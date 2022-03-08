﻿using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace Morphic.FocusWatch
{
    internal class Program
    {
        private static FileSystemWatcher watcher = null;

        public static bool IsFocusRunning { get; set; }

        static void Main(string[] args)
        {
            try
            {
                SingleInstanceWatcher();

                CheckIsFocusRunning();

                watcher = new FileSystemWatcher(Common.GetAppDataAppFolder());
                watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

                watcher.Created += Watcher_Created;
                watcher.Deleted += Watcher_Deleted;
                watcher.Renamed += Watcher_Renamed;
                watcher.Changed += Watcher_Changed;

                watcher.Filter = "*.json";
                watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                LoggingService.WriteLightAppLog("Exception" + ex.Message + ex.StackTrace);
            }

            //Start watching for app close
            while (true)
            {
                try
                {
                    if (!ProcessHelpers.IsRunning("Focus"))
                    {
                        if (IsFocusRunning)
                        {
                            string appLocation = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Focus.exe");
                            Process.Start(appLocation);
                        }
                        Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.WriteLightAppLog("Exception" + ex.Message + ex.StackTrace);
                }
            }
        }

        private const string UniqueEventName = "09F57308-E5FF-4DD5-A295-7773E86151E3";
        private static EventWaitHandle eventWaitHandle;
        private static void SingleInstanceWatcher()
        {
            // check if it is already open.
            try
            {
                // try to open it - if another instance is running, it will exist , if not it will throw
                eventWaitHandle = EventWaitHandle.OpenExisting(UniqueEventName);

                // Terminate this instance.
                Environment.Exit(0);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // listen to a new event (this app instance will be the new "master")
                eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {

                LoggingService.WriteLightAppLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            try
            {
                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {

                LoggingService.WriteLightAppLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private static void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {

                LoggingService.WriteLightAppLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {
                LoggingService.WriteLightAppLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        public static void CheckIsFocusRunning()
        {
            try
            {
                string[] sessionFiles = Common.GetSessionFiles();

                if (sessionFiles.Length == 0)
                {
                    IsFocusRunning = false;
                    LoggingService.WriteLightAppLog("Focus not Running");
                    return;
                }
                else
                {
                    IsFocusRunning = true;
                    LoggingService.WriteLightAppLog("Focus Running");
                    return;
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteLightAppLog("Exception" + ex.Message + ex.StackTrace);
            }
        }
    }

    public static class ProcessHelpers
    {
        public static bool IsRunning(string name) => Process.GetProcessesByName(name).Length > 0;
    }
}