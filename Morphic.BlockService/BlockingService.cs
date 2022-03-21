﻿using CitadelCore.IO;
using CitadelCore.Logging;
using CitadelCore.Net.Http;
using CitadelCore.Net.Proxy;
using CitadelCore.Windows.Net.Proxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Win32;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFirewallHelper;

namespace Morphic.BlockService
{
    class BlockingService : ServiceBase
    {
        private static byte[] s_blockPageBytes;

        private static ManagementEventWatcher processWatcher = new ManagementEventWatcher((EventQuery)new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
        private static Timer installedAppsTimer;
        private static Process uwpProcess;

        private static readonly ushort s_standardHttpPortNetworkOrder = (ushort)IPAddress.HostToNetworkOrder((short)80);
        private static readonly ushort s_standardHttpsPortNetworkOrder = (ushort)IPAddress.HostToNetworkOrder((short)443);
        private static readonly ushort s_altHttpPortNetworkOrder = (ushort)IPAddress.HostToNetworkOrder((short)8080);
        private static readonly ushort s_altHttpsPortNetworkOrder = (ushort)IPAddress.HostToNetworkOrder((short)8443);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr hObject);

        public delegate bool WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        /// <summary>
        /// We pass this in to stream copy operations whenever the user has asked us to pull a
        /// payload from the net into memory. We set a hard limit of ~128 megs simply to avoid being
        /// vulnerable to an attack that would balloon memory consumption.
        /// </summary>
        private static readonly long s_maxInMemoryData = 128000000;

        static AppEngine _engine;
        public static AppEngine Engine { get { return _engine; } }
        //Set Session file watcher
        public BlockingService()
        {
            try
            {
                _engine = AppEngine.Instance;

                //Setup Service
                this.ServiceName = "FocusService";
                this.CanStop = true;
                this.CanPauseAndContinue = true;

                //Setup logging
                this.AutoLog = false;

                this.EventLog.Source = this.ServiceName;
                this.EventLog.Log = "Application";

                //EventLog.WriteEntry("Log Start");

                LoggingService.WriteServiceLog("Service Launched");

                //Kill Running Apps
                BlockApps();
            }
            catch (Exception ex)
            {
                //EventLog.WriteEntry(ex.Message + ex.StackTrace);
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private void StartBlock()
        {
            try
            {
                GrantSelfFirewallAccess();

                //s_blockPageBytes = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BlockedPage.html"));

                // Hooking into these properties gives us an abstract interface where we may use
                // informational, warning and error messages generated by the internals of the proxy in
                // whatsoever way we see fit, though the design was to allow users to choose logging mechanisms.
                LoggerProxy.Default.OnInfo += (msg) =>
                {
                    //Console.WriteLine("INFO: {0}", msg);
                };

                LoggerProxy.Default.OnWarning += (msg) =>
                {
                    //Console.WriteLine("WARN: {0}", msg);
                    //Log("WARN: "+ msg);
                };

                LoggerProxy.Default.OnError += (msg) =>
                {
                    //Console.WriteLine("ERRO: {0}", msg);
                    //LoggingService.WriteServiceLog("ERRO: " + msg);
                    //Log("ERRO: "+ msg);
                };

                var cfg = new ProxyServerConfiguration
                {
                    AuthorityName = "Morphic Focus Authority",
                    FirewallCheckCallback = OnFirewallCheck,
                    HttpMessageReplayInspectionCallback = OnReplayInspection,
                    NewHttpMessageHandler = OnNewMessage,
                    HttpMessageWholeBodyInspectionHandler = OnWholeBodyContentInspection,
                    HttpMessageStreamedInspectionHandler = OnStreamedContentInspection,
                    HttpExternalRequestHandlerCallback = OnManualFulfillmentCallback,
                    BlockExternalProxies = true
                };

                // Just create the server.
                proxyServer = new WindowsProxyServer(cfg);

                // Give it a kick.
                proxyServer.Start(0);

                LoggingService.WriteServiceLog("Service Started");

                // And you're up and running.
                Console.WriteLine("Proxy Running");

                Console.WriteLine("Listening for IPv4 HTTP/HTTPS connections on port {0}.", proxyServer.V4HttpEndpoint.Port);
                Console.WriteLine("Listening for IPv6 HTTP/HTTPS connections on port {0}.", proxyServer.V6HttpEndpoint.Port);
            }
            catch (Exception ex)
            {
                //EventLog.WriteEntry(ex.Message + ex.StackTrace);
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }
        private void ProcessWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            BlockApps();
        }

        public static void BlockApps()
        {
            try
            {
                //If Focus is not running, do not filter anything
                //LoggingService.WriteServiceLog("Engine.IsFocusRunning : " + Engine.IsFocusRunning);
                if (!Engine.IsFocusRunning) return;

                //LoggingService.WriteServiceLog("Engine.IsBreakRunning : " + Engine.IsBreakRunning);
                if (Engine.IsBreakRunning)
                {
                    if (Engine.Session1Blocklist != null)
                    {
                        if (Engine.Session1Blocklist.BreakBehavior != Data.Models.BreakBehavior.Blocked) //TODO
                            return;
                    }
                    else
                    {
                        return;
                    }
                }

                //Console.WriteLine("Engine.BlockApps : ");
                //Engine.BlockApps.ForEach(p => Console.WriteLine(p));
                //Console.WriteLine("Engine.ExceptionApps : ");
                //Engine.ExceptionApps.ForEach(p => Console.WriteLine(p));

                //Get the list of apps to be blocked
                if (Engine.BlockApps.Count == 0) return; //Return if no apps are to be blocked

                //Loop through running processes
                Process[] procs = Process.GetProcesses();
                IEnumerator<Process> enumprocs;
                try
                {
                    enumprocs = ((IEnumerable<Process>)procs).Reverse<Process>().GetEnumerator();

                    while (enumprocs.MoveNext())
                    {

                        Process currentProc = enumprocs.Current;
                        //Console.WriteLine("Process Name : " + currentProc.ProcessName);
                        try
                        {
                            if ((uint)currentProc.Id > 0U & currentProc.Id != 4)
                            {
                                Process proc = currentProc; //To get the real process of UWP Apps
                                if (currentProc.ProcessName == "ApplicationFrameHost")
                                {
                                    try
                                    {
                                        EnumChildWindows(currentProc.MainWindowHandle, new WindowEnumProc(UWPCallback), IntPtr.Zero);
                                        proc = uwpProcess;
                                    }
                                    catch (Exception ex)
                                    {
                                        //LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                                    }
                                }

                                if (proc == null) return;
                                string exeName;
                                try
                                {
                                    exeName = proc.MainModule.FileName.Replace("\\", "/");
                                }
                                catch (Exception ex)
                                {
                                    //LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                                    try
                                    {
                                        exeName = (GetMainModuleFilepath(proc.Id) ?? string.Empty).Replace("\\", "/");
                                    }
                                    catch (Exception ex2)
                                    {
                                        //LoggingService.WriteServiceLog("Exception" + ex2.Message + ex2.StackTrace);
                                        exeName = proc.ProcessName + ".exe";
                                    }
                                }

                                try
                                {
                                    exeName = Path.GetFileName(exeName);
                                }
                                catch (Exception ex)
                                {
                                    //LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                                }
                                //LoggingService.WriteServiceLog("exeName : " + exeName);
                                if (!String.IsNullOrWhiteSpace(exeName))
                                {
                                    exeName = exeName.ToLowerInvariant();
                                    
                                    if (Engine.BlockApps.Contains(exeName) && !Engine.ExceptionApps.Contains(exeName))
                                    {
                                        try
                                        {
                                            //bool IsProcOwnerAdmin = IsProcessOwnerAdmin(currentProc.ProcessName); todo

                                            //if (exeName.Contains("notepad++"))
                                            //{
                                            //    Console.WriteLine("GetProcessOwner(currentProc.Id) : " + GetProcessOwner(currentProc.Id));
                                            //    Console.WriteLine("GetLastUserLoggedOn : " + GetLastUserLoggedOn());
                                            //}

                                            //if (!GetLastUserLoggedOn().Contains(GetProcessOwner(currentProc.Id)))
                                            //{
                                            //    return;
                                            //}
                                            currentProc.Kill();
                                            LaunchFocusApp(exeName);
                                        }
                                        catch (Exception ex)
                                        {
                                            //LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                //LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private static void LaunchFocusApp(string exeName)
        {
            try
            {
                string filepath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Focus.exe");
                //string filepath = Path.Combine(@"E:\Focus\Focus.NET\rtf - focus - windows\Morphic.Focus\bin\Debug\net6.0 - windows", "Focus.exe");
                LoggingService.WriteServiceLog("Process : " + filepath + " " + exeName);
                
                ProcessHandler.CreateProcessAsUser(filepath, exeName).Start();
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }
        private static string GetLastUserLoggedOn()
        {
            try
            {
                string location = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI";
                var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (var hive = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, Environment.MachineName, registryView))
                {
                    using (var key = hive.OpenSubKey(location))
                    {
                        var item = key.GetValue("LastLoggedOnUser");
                        string itemValue = item == null ? "NA" : item.ToString();
                        return itemValue;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }

            return "NA";
        }

        public static string GetProcessOwner(int processId)
        {
            try
            {
                ManagementObjectCollection objectCollection = new ManagementObjectSearcher("Select * From Win32_Process Where ProcessID = " + processId.ToString()).Get();
                try
                {
                    foreach (ManagementObject managementObject in objectCollection)
                    {
                        string[] strArray = new string[2] { string.Empty, string.Empty };
                        if (Convert.ToInt32(RuntimeHelpers.GetObjectValue(managementObject.InvokeMethod("GetOwner", (object[])strArray))) == 0)
                        {
                            return strArray[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }

            return "NA";
        }

        public static bool IsProcessOwnerAdmin(string processName)
        {
            bool IsProcOwnerAdmin = false;
            try
            {
                Process[] processesByName = Process.GetProcessesByName(processName);
                int index = 0;
                while (index < processesByName.Length)
                {
                    Process process = processesByName[index];
                    IntPtr TokenHandle = IntPtr.Zero;
                    OpenProcessToken(process.Handle, 983551U, out TokenHandle);
                    WindowsIdentity windowsIdentity = new WindowsIdentity(TokenHandle);
                    try
                    {
                        foreach (IdentityReference group in windowsIdentity.Groups)
                        {
                            if (group.IsValidTargetType(typeof(SecurityIdentifier)))
                            {
                                SecurityIdentifier securityIdentifier = group as SecurityIdentifier;
                                if (securityIdentifier.IsWellKnown(WellKnownSidType.AccountAdministratorSid) || securityIdentifier.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
                                {
                                    IsProcOwnerAdmin = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                    }
                    CloseHandle(TokenHandle);
                    checked { ++index; }
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                return false;
            }

            return IsProcOwnerAdmin;
        }

        public static string GetMainModuleFilepath(int processId)
        {
            string strFileName = (string)null;
            try
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_Process").GetInstances();
                try
                {
                    foreach (ManagementObject managementObject in instances)
                    {
                        if (((uint)managementObject["ProcessId"]) == processId)
                        {
                            if (managementObject["ExecutablePath"] != null)
                            {
                                strFileName = managementObject["ExecutablePath"].ToString();
                            }
                            else
                            {
                                strFileName = Process.GetProcessById(processId).ProcessName + ".exe";
                            }

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
            return strFileName;
        }

        private static bool UWPCallback(IntPtr hwnd, IntPtr lparam)
        {
            try
            {
                Process processId = Process.GetProcessById(GetWindowProcessId(hwnd));
                if (processId.ProcessName != "ApplicationFrameHost")
                    uwpProcess = processId;
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
            return true;
        }

        public static int GetWindowProcessId(IntPtr hwnd)
        {
            int lpdwProcessId = 0;
            try
            {
                GetWindowThreadProcessId(hwnd, ref lpdwProcessId);
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
            return lpdwProcessId;
        }

        private void writeInstalledApps(object state)
        {
            try
            {
                List<string> lstApps = new List<string>();
                bool flag = false;

                //Gather installed apps
                try
                {
                    //Check if we have Windows Apps
                    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\WindowsApps"))
                    {
                        //Populate Windows Apps
                        int dirCount = 0;
                        try
                        {
                            dirCount = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\WindowsApps", "*", SearchOption.TopDirectoryOnly).Length;
                        }
                        catch (Exception ex)
                        {
                            LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                        }

                        if (dirCount > 0)
                        {
                            try
                            {
                                foreach (string exeFile in Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\WindowsApps", "*.exe", SearchOption.AllDirectories))
                                    lstApps.Add(exeFile);
                            }
                            catch (Exception ex)
                            {
                                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                            }
                        }
                    }

                    //Check if we have System Apps
                    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\SystemApps"))
                    {
                        //Populate Windows Apps
                        int dirCount = 0;
                        try
                        {
                            dirCount = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\SystemApps", "*", SearchOption.TopDirectoryOnly).Length;
                        }
                        catch (Exception ex)
                        {
                            LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                        }

                        if (dirCount > 0)
                        {
                            try
                            {
                                foreach (string exeFile in Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\SystemApps", "*.exe", SearchOption.AllDirectories))
                                    lstApps.Add(exeFile);
                            }
                            catch (Exception ex)
                            {
                                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                //EventLog.WriteEntry(ex.Message + ex.StackTrace);
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private static FirewallResponse OnFirewallCheck(FirewallRequest request)
        {
            try
            {
                //If Focus is not running, do not filter anything
                if (!Engine.IsFocusRunning) return new FirewallResponse(CitadelCore.Net.Proxy.FirewallAction.DontFilterApplication);
                
                if (Engine.IsBreakRunning)
                {
                    if (Engine.Session1Blocklist != null)
                    {
                        if (Engine.Session1Blocklist.BreakBehavior != Data.Models.BreakBehavior.Blocked) //TODO
                            return new FirewallResponse(CitadelCore.Net.Proxy.FirewallAction.DontFilterApplication);
                    }
                    else
                    {
                        return new FirewallResponse(CitadelCore.Net.Proxy.FirewallAction.DontFilterApplication);
                    }
                }

                // Only filter chrome, msedge and firefox
                var filtering = request.BinaryAbsolutePath.IndexOf("chrome", StringComparison.OrdinalIgnoreCase) != -1 ||
                    request.BinaryAbsolutePath.IndexOf("msedge", StringComparison.OrdinalIgnoreCase) != -1 ||
                    request.BinaryAbsolutePath.IndexOf("firefox", StringComparison.OrdinalIgnoreCase) != -1 ||
                    request.BinaryAbsolutePath.IndexOf("opera", StringComparison.OrdinalIgnoreCase) != -1 ||
                    request.BinaryAbsolutePath.IndexOf("brave", StringComparison.OrdinalIgnoreCase) != -1 ||
                    request.BinaryAbsolutePath.IndexOf("microsoftedge", StringComparison.OrdinalIgnoreCase) != -1 ||
                    request.BinaryAbsolutePath.IndexOf("vivaldi", StringComparison.OrdinalIgnoreCase) != -1 ||
                    request.BinaryAbsolutePath.IndexOf("iexplore", StringComparison.OrdinalIgnoreCase) != -1;

                //var filtering = true;

                if (filtering)
                {
                    if (
                        request.RemotePort == s_standardHttpPortNetworkOrder ||
                        request.RemotePort == s_standardHttpsPortNetworkOrder ||
                        request.RemotePort == s_altHttpPortNetworkOrder ||
                        request.RemotePort == s_altHttpsPortNetworkOrder
                        )
                    {
                        // Let's allow browser to access TCP 80 and 443, but block all other ports.
                        //Console.WriteLine("Filtering application {0} destined for {1}", request.BinaryAbsolutePath, (ushort)IPAddress.HostToNetworkOrder((short)request.RemotePort));
                        return new FirewallResponse(CitadelCore.Net.Proxy.FirewallAction.FilterApplication);
                    }
                    else
                    {
                        // Let's allow browser to access TCP 80 and 443, but ignore all other
                        // ports. We want to allow non 80/443 requests to go through because
                        // this example now demonstrates the replay API, which will cause
                        // a bunch of browser tabs to open whenever you visit my website.
                        //
                        // If we filtered the replays back through the proxy, who knows
                        // what would happen! Actually that's not true, you'd invoke an infinite
                        // loopback, spawn a ton of browser tabs and then call me a bad programmer.
                        //Console.WriteLine("Ignoring internet for application {0} destined for {1}", request.BinaryAbsolutePath, (ushort)IPAddress.HostToNetworkOrder((short)request.RemotePort));
                        return new FirewallResponse(CitadelCore.Net.Proxy.FirewallAction.DontFilterApplication);
                    }
                }

                // For all other applications, just let them access the internet without filtering.
                //Console.WriteLine("Not filtering application {0} destined for {1}", request.BinaryAbsolutePath, (ushort)IPAddress.HostToNetworkOrder((short)request.RemotePort));
                return new FirewallResponse(CitadelCore.Net.Proxy.FirewallAction.DontFilterApplication);
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception :" + ex.Message + ex.StackTrace);
            }
            return new FirewallResponse(CitadelCore.Net.Proxy.FirewallAction.DontFilterApplication);
        }

        /// <summary>
        /// Called whenever a new request or response message is intercepted.
        /// </summary>
        /// <param name="messageInfo">
        /// The message info.
        /// </param>
        /// <remarks>
        /// In this callback we can do all kinds of crazy things, including fully modify the HTTP
        /// headers, the request target, etc etc.
        /// </remarks>
        private static void OnNewMessage(HttpMessageInfo messageInfo)
        {
            //If the website is part of the exception list, ignore and continue
            foreach (Uri exceptionSite in Engine.ExceptionSites)
            {
                try
                {
                    if (exceptionSite.Host.AreTheSameUrls(messageInfo.Url.Host))
                    {
                        messageInfo.ProxyNextAction = ProxyNextAction.AllowAndIgnoreContentAndResponse;
                        return;
                    }
                }
                catch
                {
                }
            }

            //If the website is part of the block list, block it and redirect to morphic
            foreach (Uri blockSite in Engine.BlockSites)
            {
                try
                {
                    if (blockSite.Host.AreTheSameUrls(messageInfo.Url.Host))
                    {
                        RedirectToMorphic(messageInfo);
                        return;
                    }
                }
                catch
                {
                }
            }

            // By default, allow and ignore content
            messageInfo.ProxyNextAction = ProxyNextAction.AllowAndIgnoreContentAndResponse;
        }

        private static bool RedirectToMorphic(HttpMessageInfo messageInfo)
        {
            if (messageInfo.MessageType == MessageType.Request)
            {
                messageInfo.MakeTemporaryRedirect("https://morphic.org/websiteblocked/");
                messageInfo.ProxyNextAction = ProxyNextAction.DropConnection;
                return true;
            }

            return false;
        }

        private static void OnStreamedContentInspection(HttpMessageInfo messageInfo, StreamOperation operation, Memory<byte> buffer, out bool dropConnection)
        {
            dropConnection = false;
        }

        private static void OnWholeBodyContentInspection(HttpMessageInfo messageInfo)
        {

        }

        private static void OnReplayInspection(HttpMessageInfo messageInfo, string replayUrl, HttpReplayTerminationCallback cancellationCallback)
        {

        }

        private static Task OnManualFulfillmentCallback(HttpMessageInfo messageInfo, HttpContext context)
        {
            return Task.CompletedTask;
        }
        private static void GrantSelfFirewallAccess()
        {
            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            var hostAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var exePath = Path.Combine(Path.GetDirectoryName(hostAssembly.Location), processName + ".exe");

            // We want to delete all rules that match our process name, so we can create new ones
            // that we know will work.
            var myRules = FirewallManager.Instance.Rules.Where(r => r.Name.Equals(processName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (myRules != null)
            {
                foreach (var rule in myRules)
                {
                    FirewallManager.Instance.Rules.Remove(rule);
                }
            }

            // Allow all inbound and outbound communications from our process.
            var inboundRule = FirewallManager.Instance.CreateApplicationRule(
                FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public,
                processName,
                WindowsFirewallHelper.FirewallAction.Allow, exePath //hostAssembly.Location
            );
            inboundRule.Direction = FirewallDirection.Inbound;

            FirewallManager.Instance.Rules.Add(inboundRule);

            var outboundRule = FirewallManager.Instance.CreateApplicationRule(
                FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public,
                processName,
                WindowsFirewallHelper.FirewallAction.Allow, exePath //hostAssembly.Location
            );
            outboundRule.Direction = FirewallDirection.Outbound;

            // Add the rules to the manager, which will commit them to Windows.
            FirewallManager.Instance.Rules.Add(outboundRule);
        }

        WindowsProxyServer proxyServer = null;


        protected override void OnStart(string[] args)
        {
            try
            {
                LoggingService.WriteServiceLog("On Start");

                //Writes installed apps name in json file
                installedAppsTimer = new Timer(new TimerCallback(writeInstalledApps), (object)null, 0, 180000);

                //Watches for start of any new process
                processWatcher.EventArrived += ProcessWatcher_EventArrived;
                processWatcher.Start();

                //Starts Website Blocking
                StartBlock();
                //this.EventLog.WriteEntry("Started");

                LaunchFocusApp(string.Empty); //Launch focus app on pc start
            }
            catch (Exception ex)
            {
                //EventLog.WriteEntry(ex.Message + ex.StackTrace);
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }

            base.OnStart(args);
        }



        protected override void OnStop()
        {
            try
            {
                LoggingService.WriteServiceLog("On Stop");

                // Stop if you must.
                proxyServer.Stop();
                processWatcher.Stop();
                processWatcher.Dispose();
                installedAppsTimer.Dispose();
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
            base.OnStop();
        }

        protected override void OnPause()
        {
            try
            {
                LoggingService.WriteServiceLog("On Pause");
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
            base.OnPause();
        }

        protected override void OnShutdown()
        {
            try
            {
                LoggingService.WriteServiceLog("On Shutdown");

                // Stop if you must.
                proxyServer.Stop();
                processWatcher.Stop();
                processWatcher.Dispose();
                installedAppsTimer.Dispose();
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
            base.OnShutdown();
        }
        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
    }
}