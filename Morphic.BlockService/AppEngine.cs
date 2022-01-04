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

        public bool IsBreakRunning
        {
            get
            {
                if (Session1 != null)
                    return Session1.IsBreakRunning;
                else
                    return true;
            }
        }

        private List<Uri> _blockSites = new List<Uri>();
        public List<Uri> BlockSites
        {
            get
            {
                return _blockSites;
            }
        }

        private List<Uri> _exceptionSites = new List<Uri>();
        public List<Uri> ExceptionSites
        {
            get
            {
                return _exceptionSites;
            }
        }

        private List<Session> _lstSession = new List<Session>();
        public List<Session> LstSession { get => _lstSession; set => _lstSession = value; }

        private UserPreferences? _userPreferences = null;
        public UserPreferences? UserPreferences
        {
            get
            {
                return _userPreferences;
            }
            set
            {
                if (_userPreferences != value)
                {
                    _userPreferences = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Session? Session1
        {
            get
            {
                //lock (locker)
                //{
                if (LstSession.Count >= 1)
                    return LstSession[0];
                else
                    return null;
                //}
            }
        }
        public Session? Session2
        {
            get
            {
                //lock (locker)
                //{
                if (LstSession.Count == 2)
                    return LstSession[1];
                else
                    return null;
                //}
            }
        }

        public Blocklist Session1Blocklist
        {
            get
            {
                if (Session1 == null) return null;

                string blocklistName = Session1.Schedule != null ?
                            (Session1.Schedule.BlockListName != null ? Session1.Schedule.BlockListName : string.Empty)
                            : Session1.BlockListName;
                
                if (!string.IsNullOrWhiteSpace(blocklistName))
                {
                    if (UserPreferences.BlockLists.Any(p => p.Name.ToLowerInvariant() == blocklistName.ToLowerInvariant()))
                    {
                        Blocklist blockList = UserPreferences.BlockLists.Where(p => p.Name.ToLowerInvariant() == blocklistName.ToLowerInvariant()).First();
                        return blockList;
                    }
                }

                return null;
            }
        }
        AppEngine()
        {
            try
            {
                GetFocusAndSessionSettings();

                watcher = new FileSystemWatcher(Path.GetDirectoryName(Common.GetWinRootFolder()));
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
                GetFocusAndSessionSettings();
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
                GetFocusAndSessionSettings();
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
                GetFocusAndSessionSettings();
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
                GetFocusAndSessionSettings();
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }

        public void CheckIsFocusRunning()
        {
            try
            {
                var http = new UriBuilder("instagram.com").Uri;
                var https = new UriBuilder("www.instagram.com").Uri;

                var result = Uri.Compare(http,
                    https,
                    UriComponents.Host | UriComponents.PathAndQuery,
                    UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase);

                string[] sessionFiles = Common.GetSessionFiles();
                LstSession.Clear();
                BlockSites.Clear();
                ExceptionSites.Clear();

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
                        LstSession.Add(session);

                        string blocklistName = session.Schedule != null ?
                            (session.Schedule.BlockListName != null ? session.Schedule.BlockListName : string.Empty)
                            : session.BlockListName;

                        //Update BlockSites & ExceptionSites
                        if (!string.IsNullOrWhiteSpace(blocklistName))
                        {
                            if (UserPreferences.BlockLists.Any(p => p.Name.ToLowerInvariant() == blocklistName.ToLowerInvariant()))
                            {
                                Blocklist blockList = UserPreferences.BlockLists.Where(p => p.Name.ToLowerInvariant() == blocklistName.ToLowerInvariant()).First();
                                if (blockList != null)
                                {
                                    blockList.AlsoBlock.ActiveAppsAndWebsites.Where(p => p.IsActive && !p.IsApp).ToList().ForEach(p => BlockSites.Add(new UriBuilder(p.Name).Uri));
                                    blockList.Exceptions.ActiveAppsAndWebsites.Where(p => p.IsActive && !p.IsApp).ToList().ForEach(p => ExceptionSites.Add(new UriBuilder(p.Name).Uri));
                                }
                            }
                        }

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
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }

        }

        internal void GetFocusAndSessionSettings()
        {
            try
            {
                //Get Focus Preferences from the Settings.json file
                //If the file is not found, a new settings file is created
                //Settings are persisted in memomy as long as the Focus Tool is running

                //1. Get Focus Preferences from the Settings.json file
                JSONHelper jSONHelper = new JSONHelper(Common.SETTINGS_FILE_NAME);
                UserPreferences = jSONHelper.Get<UserPreferences>();

                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }
    }
}
