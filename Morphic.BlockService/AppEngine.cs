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

        private List<string> _blockApps = new List<string>();
        public List<string> BlockApps
        {
            get
            {
                return _blockApps;
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

        private List<string> _exceptionApps = new List<string>();
        public List<string> ExceptionApps
        {
            get
            {
                return _exceptionApps;
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

        private CategoryCollection? _categoryCollection = null;
        public CategoryCollection? CategoryCollection
        {
            get
            {
                return _categoryCollection;
            }
            set
            {
                if (_categoryCollection != value)
                {
                    _categoryCollection = value;
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

        public Blocklist Session2Blocklist
        {
            get
            {
                if (Session2 == null) return null;

                string blocklistName = Session2.Schedule != null ?
                            (Session2.Schedule.BlockListName != null ? Session2.Schedule.BlockListName : string.Empty)
                            : Session2.BlockListName;

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
                string[] sessionFiles = Common.GetSessionFiles();
                LstSession.Clear();
                BlockSites.Clear();
                ExceptionSites.Clear();
                BlockApps.Clear();
                ExceptionApps.Clear();

                if (sessionFiles.Length == 0)
                {
                    IsFocusRunning = false;
                    LoggingService.WriteServiceLog("Focus not Running");
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

                        //Update BlockSitesApps & ExceptionSitesApps
                        if (!string.IsNullOrWhiteSpace(blocklistName))
                        {
                            if (UserPreferences.BlockLists.Any(p => p.Name.ToLowerInvariant() == blocklistName.ToLowerInvariant()))
                            {
                                Blocklist blockList = UserPreferences.BlockLists.Where(p => p.Name.ToLowerInvariant() == blocklistName.ToLowerInvariant()).First();
                                if (blockList != null)
                                {
                                    blockList.AlsoBlock.ActiveAppsAndWebsites.Where(p => p.IsActive && !p.IsApp).ToList().ForEach(p => BlockSites.Add(new UriBuilder(p.Name).Uri));
                                    blockList.Exceptions.ActiveAppsAndWebsites.Where(p => p.IsActive && !p.IsApp).ToList().ForEach(p => ExceptionSites.Add(new UriBuilder(p.Name).Uri));

                                    blockList.AlsoBlock.ActiveAppsAndWebsites.Where(p => p.IsActive && p.IsApp).ToList().ForEach(p => BlockApps.Add(p.Name.ToLowerInvariant().Trim()));
                                    blockList.Exceptions.ActiveAppsAndWebsites.Where(p => p.IsActive && p.IsApp).ToList().ForEach(p => ExceptionApps.Add(p.Name.ToLowerInvariant().Trim()));

                                    foreach(Blockcategory blockCategory in blockList.Blockcategories.Where(p => p.IsActive))
                                    {
                                        Category category = CategoryCollection.Categories
                                            .Where(p => p.Name.ToLowerInvariant().Trim() == blockCategory.Name.ToLowerInvariant().Trim()).First();

                                        category.CollAppsAndWebsites.ActiveAppsAndWebsites
                                            .Where(p => p.IsActive && !p.IsApp).ToList()
                                            .ForEach(p => BlockSites.Add(new UriBuilder(p.Name.Trim()).Uri));

                                        category.CollAppsAndWebsites.ActiveAppsAndWebsites
                                            .Where(p => p.IsActive && p.IsApp).ToList()
                                            .ForEach(p => BlockApps.Add(p.Name.ToLowerInvariant().Trim()));
                                    }
                                }
                            }
                        }


                        IsFocusRunning = true;
                        LoggingService.WriteServiceLog("Focus Running");
                    }
                }

                //Add Temporary Unblock
                UserPreferences.General.TemporarilyUnblock.ActiveAppsAndWebsites.Where(p => p.IsActive && !p.IsApp).ToList().ForEach(p => ExceptionSites.Add(new UriBuilder(p.Name).Uri));
                UserPreferences.General.TemporarilyUnblock.ActiveAppsAndWebsites.Where(p => p.IsActive && p.IsApp).ToList().ForEach(p => ExceptionApps.Add(p.Name.ToLowerInvariant().Trim()));
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
                JSONHelper settingHelper = new JSONHelper(Common.SETTINGS_FILE_NAME);
                UserPreferences = settingHelper.Get<UserPreferences>();

                //2. Get Categories
                JSONHelper categoryHelper = new JSONHelper(Common.CATEGORIES_FILE_NAME);
                CategoryCollection = categoryHelper.Get<CategoryCollection>();

                CheckIsFocusRunning();
            }
            catch (Exception ex)
            {
                LoggingService.WriteServiceLog("Exception" + ex.Message + ex.StackTrace);
            }
        }
    }
}
