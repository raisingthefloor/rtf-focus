using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.BlockService
{
    public class AppEngine : BaseClass
    {
        private static readonly AppEngine _instance = new AppEngine();
        public static AppEngine Instance { get { return _instance; } }

        private FileSystemWatcher watcher = null;

        public bool IsFocusRunning { get; set; }

        AppEngine()
        {
            try
            {
                CheckIsFocusRunning();

                watcher = new FileSystemWatcher(Path.GetDirectoryName(Common.MakeFilePath(Common.SESSION_FILE_NAME)));
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

                watcher.Filter = Common.SESSION_SEARCH;
                watcher.EnableRaisingEvents = true;
                LoggingService.WriteServiceLog("Watching started");
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog(ex.Message + ex.StackTrace);
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {

                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            try
            {
                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {

                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {

                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        public void CheckIsFocusRunning()
        {
            string[] sessionFiles = Common.GetSessionFiles();

            if (sessionFiles.Length == 0)
            {
                IsFocusRunning = false;
                return;
            }

            foreach (string sessionFile in sessionFiles)
            {
                Session? session = null;

                //Get the Session data from the file
                //Get the Focus Session Object
                JSONHelper jSONHelper = new JSONHelper(sessionFile);
                session = jSONHelper.Get<Session>();

                if (session != null)
                {
                    if (DateTime.Now <= session.ActualStartTime.AddMinutes(session.SessionDuration))
                    {
                        IsFocusRunning = true;
                        LoggingService.WriteServiceLog("Focus Running");
                    }
                    else
                    {
                        IsFocusRunning = false;
                        LoggingService.WriteServiceLog("Focus not Running");
                    }
                }
            }
        }
    }
}
