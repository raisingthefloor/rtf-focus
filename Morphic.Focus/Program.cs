using Morphic.Data.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace Morphic.Focus
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var proc = Process.GetCurrentProcess();
            var processName = proc.ProcessName.Replace(".vshost", "");
            var runningProcess = Process.GetProcesses()
                .FirstOrDefault(x => (x.ProcessName == processName ||
                                x.ProcessName == proc.ProcessName ||
                                x.ProcessName == proc.ProcessName + ".vshost") && x.Id != proc.Id);

            //If this is the first instance
            if (runningProcess == null)
            {
                // Create a timer with a two second interval.
                LaunchWatcherAppTimer = new System.Timers.Timer(2000);
                // Hook up the Elapsed event for the timer. 
                LaunchWatcherAppTimer.Elapsed += LaunchWatcherAppTimer_Elapsed;
                LaunchWatcherAppTimer.AutoReset = true;
                LaunchWatcherAppTimer.Enabled = true;

                var app = new App();
                app.InitializeComponent();
                var window = new MainWindow();
                MainWindow.HandleParameter(args);
                app.Run(window);

                MainWindow.HandleParameter(args);



                return; // In this case we just proceed on loading the program
            }

            //If this is the second instance
            if (args.Length > 0)
                UnsafeNative.SendMessage(runningProcess.MainWindowHandle, string.Join(" ", args));

            LaunchWatcherAppTimer.Stop();
            LaunchWatcherAppTimer.Dispose();
        }

        private static void LaunchWatcherAppTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                LaunchWatcherApp();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private static System.Timers.Timer LaunchWatcherAppTimer;
        
        private static void LaunchWatcherApp()
        {
            try
            {
                if (!ProcessHelpers.IsRunning("Morphic.FocusWatch"))
                {
                    string appLocation = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Morphic.FocusWatch.exe");
                    LoggingService.WriteAppLog("appLocation : " + appLocation);

                    using (var managementClass = new ManagementClass("Win32_Process"))
                    {
                        var processInfo = new ManagementClass("Win32_ProcessStartup");
                        processInfo.Properties["CreateFlags"].Value = 0x00000008;

                        var inParameters = managementClass.GetMethodParameters("Create");
                        inParameters["CommandLine"] = appLocation;
                        inParameters["ProcessStartupInformation"] = processInfo;

                        var result = managementClass.InvokeMethod("Create", inParameters, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog("Exception" + ex.Message + ex.StackTrace);
            }
        }
    }

    public static class ProcessHelpers
    {
        public static bool IsRunning(string name) => Process.GetProcessesByName(name).Length > 0;
    }
}
