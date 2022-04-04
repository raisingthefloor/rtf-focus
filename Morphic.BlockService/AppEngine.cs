using Morphic.Data.Models;
using Morphic.Data.Services;
using Morphic.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.BlockService
{
    public class AppEngine : BaseClass
    {
        private static readonly AppEngine _instance = new AppEngine();
        public static AppEngine Instance { get { return _instance; } }

        private FileSystemWatcher watcher = null;

        private MorphicTelemetryClient? _telemetryClient = null;

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
                // start telemetry session
                this.StartTelemetrySession();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }

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

        #region DUPLICATE telemetry code
        // NOTE: duplicating this code here is very much _not_ ideal.  Here are the reasons:
        // 1. This is a true last-minute item, and the potential risk of adding a share library and refactoring out this code is risky for the limited trial period where this duplicate code will exist.
        // 2. The backend service shouldn't generate a telemetry UUID if one doesn't already exist; note that if we use this type of code in production we should create a wait to either wait until a telemetry ID is created before sending data or we should share a communication path with the GUI app.
        // 3. The GUI focus app doesn't have a clean, ready-to-go channel for pushing information to the main app, so we record the telemetry events here instead of bubbling them up; [notably, it's important that we queue the events here regardless...in case the foreground app isn't running.]

        private void StartTelemetrySession()
        {
            // retrieve the telemetry device ID for this device; if it doesn't exist then create a new one
            var telemetryDeviceUuid = this.GetOrCreateTelemetryDeviceUuid();

            // configure our telemetry uplink
            var mqttHostname = "focusmqtt.morphic.org";
            var mqttClientId = telemetryDeviceUuid;
            var mqttUsername = "focus-windows";
            var mqttAnonymousPassword = "wY2NeLJht6Ss4IhN";

            var mqttConfig = new MorphicTelemetryClient.WebsocketTelemetryClientConfig()
            {
                Hostname = mqttHostname,
                Port = 443,
                Path = "/ws",
                ClientId = mqttClientId,
                Username = mqttUsername,
                Password = mqttAnonymousPassword,
                UseTls = true
            };
            var telemetryClient = new MorphicTelemetryClient(mqttConfig);
            telemetryClient.SiteId = null;
            _telemetryClient = telemetryClient;

            Task.Run(async () =>
            {
                await telemetryClient.StartSessionAsync();
            });
        }

        internal string? GetTelemetryClientId()
        {
            return _telemetryClient?.GetTelemetryClientId();
        }

        internal async Task StopTelemetrySessionAsync()
        {
            try
            {
                if (_telemetryClient is not null)
                {
                    // wait up to 2.5 seconds for message queue to empty
                    await _telemetryClient.FlushMessageQueueAsync(2500);

                    await _telemetryClient.StopSessionAsync();
                }
            }
            catch { }
        }

        private string GetOrCreateTelemetryDeviceUuid()
        {
            // retrieve the telemetry device ID for this device; if it doesn't exist then create a new one
            string? telemetryDeviceUuid = null;

            // capture our existing telemetry id, if one already exists
            // NOTE: realistically, we should make a folder WITHIN %PROGRAMDATA%, but we are using that base folder since the rest of the app uses it
            var telemetryUuidFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Common.TELEMETRY_UUID_FILE_NAME);
            if (File.Exists(telemetryUuidFilePath) == true)
            {
                try
                {
                    telemetryDeviceUuid = File.ReadAllText(telemetryUuidFilePath);
                }
                catch { }
            }

            // if we could not retrieve a device uuid, create a new one now
            if (telemetryDeviceUuid is null)
            {
                // our service is NOT the one in charge of assigning UUIDs; if we don't have one, we'll have to wait until the next reboot etc. to get one
                telemetryDeviceUuid = "unassigned";
            }

            return telemetryDeviceUuid! + "-BlockService";
        }

        public void EnqueueTelemetryRecord(string eventName, string? data = null)
        {
            _telemetryClient?.EnqueueActionMessage(eventName, data);
        }

        internal void PopulateCommonEventData(ref TelemetryEventData eventData)
        {
            eventData.CountdownTimerCheckbox = this.UserPreferences?.General.showBreakCountdownTimer ?? null;
            eventData.OneMinLockScreenCheckbox = this.UserPreferences?.General.blockScreen1stMinofBreak ?? null;
            var temporaryUnblockItemCount= this.UserPreferences?.General.TemporarilyUnblock.ActiveAppsAndWebsites.Count;
            if (temporaryUnblockItemCount is not null)
            {
                int checkedItemCount = 0;
                foreach (var element in this.UserPreferences!.General.TemporarilyUnblock.ActiveAppsAndWebsites)
                {
                    if (element.IsActive == true)
                    {
                        checkedItemCount += 1;
                    }
                }
                eventData.AllowUnblockingItemCount = checkedItemCount;
            }
            else
            {
                eventData.AllowUnblockingItemCount = null;
            }

            //

            var blocklistCount = this.UserPreferences?.BlockLists.Count ?? 0;
            var blocklistList = new List<TelemetryEventDataBlocklistData>();
            if (blocklistCount > 0)
            {
                foreach (var blocklist in this.UserPreferences!.BlockLists)
                {
                    var blocklistData = new TelemetryEventDataBlocklistData();

                    foreach (var blockcategory in blocklist.Blockcategories)
                    {
                        switch (blockcategory.Name)
                        {
                            case "Notifications":
                            case "Notifcations":
                                blocklistData.NotificationsCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Communication (Not Email)":
                                blocklistData.CommunicationCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Dating":
                                blocklistData.DatingCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Email":
                                blocklistData.EmailCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Gambling":
                                blocklistData.GamblingCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Games":
                                blocklistData.GamesCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "News":
                                blocklistData.NewsCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Porn":
                                blocklistData.PornCategoryCheckbox = blockcategory.IsActive;
                                break;
                            //case "Productivity (exceptions)":
                            //    blocklistData.ProductivityCategoryCheckbox = blockcategory.IsActive;
                            //    break;
                            case "Proxies":
                                blocklistData.ProxiesCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Shopping":
                                blocklistData.ShoppingCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Social Media":
                                blocklistData.SocialmediaCategoryCheckbox = blockcategory.IsActive;
                                break;
                            case "Videos":
                                blocklistData.VideosCategoryCheckbox = blockcategory.IsActive;
                                break;
                            default:
                                // unknown list
                                break;
                        }
                    }

                    blocklistData.BlocklistNameHash = this.HashUsingSHA1(blocklist.Name);
                    blocklistData.AlsoBlockItemsCount = blocklist.AlsoBlock.ActiveAppsAndWebsites.Count();
                    blocklistData.ExceptionItemsCount = blocklist.Exceptions.ActiveAppsAndWebsites.Count();
                    switch (blocklist.Penalty)
                    {
                        case Penalty.None:
                            blocklistData.Penalty = "none";
                            break;
                        case Penalty.Type:
                            blocklistData.Penalty = blocklist.PenaltyValue.ToString();
                            break;
                        case Penalty.Restart:
                            blocklistData.Penalty = "restart";
                            break;
                        default:
                            Debug.Assert(false, "Invalid blocklist penalty");
                            blocklistData.Penalty = null;
                            break;
                    }
                    switch (blocklist.BreakBehavior)
                    {
                        case BreakBehavior.Blocked:
                            blocklistData.BreakBehavior = "block";
                            break;
                        case BreakBehavior.UnblockLongOnly:
                            blocklistData.BreakBehavior = "unblock-Long";
                            break;
                        case BreakBehavior.UnblockFull:
                            blocklistData.BreakBehavior = "unblock-ALL";
                            break;
                        default:
                            Debug.Assert(false, "Invalid blocklist break behavior");
                            blocklistData.BreakBehavior = null;
                            break;
                    }

                    blocklistList.Add(blocklistData);
                }
            }
            eventData.BlocklistList = blocklistList;
        }

        internal string HashUsingSHA1(string value)
        {
            var valueAsBytes = System.Text.Encoding.UTF8.GetBytes(value);

            using (SHA1 sha256 = SHA1.Create())
            {
                var hashAsBytes = sha256.ComputeHash(valueAsBytes);

                var resultBuilder = new StringBuilder();
                for (int i = 0; i < hashAsBytes.Length; i += 1)
                {
                    resultBuilder.Append(hashAsBytes[i].ToString("X2"));
                }
                return resultBuilder.ToString();
            }
        }

        internal int? ConvertToInt32OrNull(string? value)
        {
            int? result;

            if (value is not null)
            {
                if (value.Trim() == String.Empty)
                {
                    return null;
                }

                try
                {
                    result = Convert.ToInt32(value);
                }
                catch
                {
                    Debug.Assert(false, "Cannot convert the value to an int; value: " + value);
                    result = null;
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        #endregion DUPLICATE telemetry code

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

                                    //Category Blocking
                                    //Check for each active category
                                    foreach (Blockcategory blockCategory in blockList.Blockcategories.Where(p => p.IsActive))
                                    {
                                        //Get the Category
                                        Category category = CategoryCollection.Categories
                                            .Where(p => p.Name.ToLowerInvariant().Trim() == blockCategory.Name.ToLowerInvariant().Trim()).First();

                                        //Get Category websites
                                        //category.CollAppsAndWebsites.ActiveAppsAndWebsites
                                        //    .Where(p => p.IsActive && !p.IsApp).ToList()
                                        //    .ForEach(p => BlockSites.Add(new UriBuilder(p.Name.Trim()).Uri));
                                        List<ActiveAppsAndWebsites> appWebsites = category.CollAppsAndWebsites.ActiveAppsAndWebsites.Where(p => p.IsActive && !p.IsApp).ToList();
                                        foreach (ActiveAppsAndWebsites appWebsite in appWebsites)
                                        {
                                            try
                                            {
                                                Uri uri = new UriBuilder(appWebsite.Name.ToLowerInvariant().Trim()).Uri;
                                                BlockSites.Add(uri);
                                            }
                                            catch //If the category item is not well constructed, ignore them for now
                                            {
                                                
                                            }
                                        }

                                        //Get Category apps
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
