using Morphic.Data.Models;
using Morphic.Data.Services;
using Morphic.Focus.Screens;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Morphic.Focus
{
    public class AppEngine : BaseClass
    {
        #region AppEngine Instance
        private static readonly AppEngine _instance = new AppEngine();
        public static AppEngine Instance { get { return _instance; } }

        AppEngine()
        {
            try
            {
                #region Categories
                //Copy Categories JSON file to user folder
                if (!File.Exists(Common.MakeFilePath(Common.CATEGORIES_FILE_NAME)))
                    File.Copy(Path.Combine(AppContext.BaseDirectory, Common.CATEGORIES_FILE_NAME), Common.MakeFilePath(Common.CATEGORIES_FILE_NAME), true);

                //Get Categories from the Categories JSON File
                GetCategoryies();
                #endregion

                #region User Preferences
                //Load User Preference's from User's Json File
                //If User's Json file do not exist, set the defaults and save it to the file
                GetFocusSettings();

                //Notify if User Preferences or Schedules are updated
                UserPreferences.PropertyChanged += UserPreferences_PropertyChanged;
                UserPreferences.Schedules.PropertyChanged += Schedules_PropertyChanged;

                //Set User Preferences -> Today's schedule
                SetTodaysSchedule();
                #endregion

                #region Ongoing Sessions
                CheckIsFocusRunning();
                #endregion

                #region Set Timers to trigger scheduled focus sessions
                ResetSchedules();
                #endregion
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region PropertyChanged
        private void Schedules_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SetTodaysSchedule(true); //Since original schedules are altered, force reset today's schedules
        }

        private void UserPreferences_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SetFocusSettings();
        }
        #endregion

        #region Schedule-Trigger

        DailyTrigger? schTrigger1 = null;
        DailyTrigger? schTrigger2 = null;
        DailyTrigger? schTrigger3 = null;
        DailyTrigger? schTrigger4 = null;
        DailyTrigger? schTrigger5 = null;
        private void ResetSchedules()
        {
            try
            {
                //Reset Triggers
                schTrigger1 = null;
                schTrigger2 = null;
                schTrigger3 = null;
                schTrigger4 = null;
                schTrigger5 = null;

                //Set Triggers
                SetTrigger(schTrigger1, UserPreferences.TodaysSchedule.Schedule1);
                SetTrigger(schTrigger2, UserPreferences.TodaysSchedule.Schedule2);
                SetTrigger(schTrigger3, UserPreferences.TodaysSchedule.Schedule3);
                SetTrigger(schTrigger4, UserPreferences.TodaysSchedule.Schedule4);
                SetTrigger(schTrigger5, UserPreferences.TodaysSchedule.Schedule5);


            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void SetTrigger(DailyTrigger? trigger, Schedule schedule)
        {
            if (schedule != null) //Schedule should be non-null
            {
                if (schedule.IsActive) //Schedule should be active
                {
                    if (Helper.IsActiveToday(schedule)) //Schedule should be active today
                    {
                        if (!UserPreferences.General.dontGive5MinWarning) //Dialog not to be shown if disabled in General Setting
                        {
                            DateTime scheduleStartTime = schedule.StartAt.AddMinutes(-5); //Modal dialog to be shown 5 mins before start time

                            trigger = new DailyTrigger(scheduleStartTime.Hour, scheduleStartTime.Minute, scheduleStartTime.Second); // today at scheduled time

                            trigger.OnTimeTriggered += () =>
                            {
                                if (CurrSession1 != null && CurrSession1.BlockListName == schedule.BlockListName) return; //Dialog not to be shown if scheduled blocklist is already active

                                if (CurrSession2 != null && CurrSession2.BlockListName == schedule.BlockListName) return; //Dialog not to be shown if scheduled blocklist is already active

                                if (CurrSession1 != null && CurrSession2 != null) return; //Dialog not to be shown if two sessions are already active

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    ScheduledSessionModal scrScheduledSessionModal = new ScheduledSessionModal(schedule);
                                    scrScheduledSessionModal.Show();
                                    LoggingService.WriteAppLog("Scheduled Session Dialog Open");
                                });

                            };
                        }
                    }
                }
            }
        }


        #endregion

        #region UI Screens
        MainMenuNonModal _focusMain = null;
        public MainMenuNonModal FocusMain
        {
            get
            {
                if (_focusMain == null)
                {
                    _focusMain = new MainMenuNonModal();
                }

                return _focusMain;
            }
            set => _focusMain = value;
        }


        ActiveSessionNonModal _focusStatus = null;
        public ActiveSessionNonModal FocusStatus
        {
            get
            {
                if (_focusStatus == null)
                {
                    _focusStatus = new ActiveSessionNonModal();
                }

                return _focusStatus;
            }
            set => _focusStatus = value;
        }

        Settings _settings = null;
        public Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings();
                }

                return _settings;
            }
            set => _settings = value;
        }

        internal void ShowFocusWindow()
        {
            if (IsFocusRunning)
            {
                //Show Focus Status Window
                FocusStatus.Show();
            }
            else
            {
                //FocusMain.SessionUpdate += FocusMain_SessionUpdate;

                //Show the Focus Main Menu
                FocusMain.Show();
            }
        }
        #endregion

        #region Properties

        #region User Preferences
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

        internal void SetFocusSettings()
        {
            //Get Focus Preferences from the Settings.json file
            //If the file is not found, a new settings file is created
            //Settings are persisted in memomy as long as the Focus Tool is running

            //1. Get Focus Preferences from the Settings.json file
            JSONHelper jSONHelper = new JSONHelper(Common.SETTINGS_FILE_NAME);

            //2. If the file is not found, a new settings file is created
            if (UserPreferences != null)
            {
                jSONHelper.Save<UserPreferences>(UserPreferences);
            }

        }

        private void SetTodaysSchedule(bool force = false)
        {
            //force=true means original schedules are altered. This basically means that we will mandatorily reset Today's Schedule
            if (!force)
            {
                //We have already set today's scheduled and this is a restart of the Focus App
                if (UserPreferences.TodaysSchedule.DateUpdated.Date == DateTime.Today.Date)
                    return;
            }

            AssignSchedule(UserPreferences.TodaysSchedule.Schedule1, UserPreferences.Schedules.Schedule1);
            AssignSchedule(UserPreferences.TodaysSchedule.Schedule2, UserPreferences.Schedules.Schedule2);
            AssignSchedule(UserPreferences.TodaysSchedule.Schedule3, UserPreferences.Schedules.Schedule3);
            AssignSchedule(UserPreferences.TodaysSchedule.Schedule4, UserPreferences.Schedules.Schedule4);
            AssignSchedule(UserPreferences.TodaysSchedule.Schedule5, UserPreferences.Schedules.Schedule5);
            UserPreferences.TodaysSchedule.DateUpdated = DateTime.Now;
        }

        void AssignSchedule(Schedule todaySchedule, Schedule schedule)
        {

            todaySchedule.BlockListName = schedule.BlockListName;
            todaySchedule.EndAt = schedule.EndAt;
            todaySchedule.IsActive = schedule.IsActive;
            todaySchedule.IsActiveFriday = schedule.IsActiveFriday;
            todaySchedule.IsActiveMonday = schedule.IsActiveMonday;
            todaySchedule.IsActiveSaturday = schedule.IsActiveSaturday;
            todaySchedule.IsActiveSunday = schedule.IsActiveSunday;
            todaySchedule.IsActiveThursday = schedule.IsActiveThursday;
            todaySchedule.IsActiveTuesday = schedule.IsActiveTuesday;
            todaySchedule.IsActiveWednesday = schedule.IsActiveWednesday;
            todaySchedule.StartAt = schedule.StartAt;
        }

        internal void GetFocusSettings()
        {
            //Get Focus Preferences from the Settings.json file
            //If the file is not found, a new settings file is created
            //Settings are persisted in memomy as long as the Focus Tool is running

            //1. Get Focus Preferences from the Settings.json file
            JSONHelper jSONHelper = new JSONHelper(Common.SETTINGS_FILE_NAME);
            UserPreferences = jSONHelper.Get<UserPreferences>();

            //2. If the file is not found, a new settings file is created
            if (UserPreferences == null)
            {
                UserPreferences = new UserPreferences();
                jSONHelper.Save<UserPreferences>(UserPreferences);
            }
        }
        #endregion

        #region CategoryCollection

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
        private void GetCategoryies()
        {
            //Get Categories List from the categories.json file
            //If the file is not found, a new settings file is created
            //Settings are persisted in memomy as long as the Focus Tool is running

            //1. Get Focus Preferences from the Settings.json file
            JSONHelper jSONHelper = new JSONHelper(Common.CATEGORIES_FILE_NAME);
            CategoryCollection = jSONHelper.Get<CategoryCollection>();

            //2. If the file is not found, a new empty categories file is created
            if (CategoryCollection == null)
            {
                CategoryCollection = new CategoryCollection();
                jSONHelper.Save<CategoryCollection>(CategoryCollection);
            }
        }

        #endregion

        #region Selected Blocklist in Settings
        private Blocklist? _selectedBlockList;
        public Blocklist? SelectedBlockList
        {
            get
            {
                //Set first item as selected item is initial selection not done yet
                if (_selectedBlockList == null && UserPreferences.BlockLists.Count > 0)
                    SelectedBlockList = UserPreferences.BlockLists[0];

                return _selectedBlockList;
            }
            set
            {
                if (_selectedBlockList != value)
                {
                    _selectedBlockList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region To Review
        private bool _isFocusRunning = false;
        public bool IsFocusRunning
        {
            get
            {
                return _isFocusRunning;
            }
            set
            {
                _isFocusRunning = value;
                NotifyPropertyChanged("IsFocusRunning"); // method implemented below
            }
        }

        private Session? _currSession1 = null;
        public Session? CurrSession1 { get => _currSession1; set => _currSession1 = value; }

        private Session? _currSession2 = null;
        public Session? CurrSession2 { get => _currSession2; set => _currSession2 = value; }
        #endregion

        #endregion

        #region Colors
        private static readonly BrushConverter bc = new BrushConverter();
        public static readonly Brush Schedule1Brush = (Brush)bc.ConvertFrom("#662261");
        public static readonly Brush Schedule2Brush = (Brush)bc.ConvertFrom("#0080A8");
        public static readonly Brush Schedule3Brush = (Brush)bc.ConvertFrom("#002957");
        public static readonly Brush Schedule4Brush = (Brush)bc.ConvertFrom("#008145");
        public static readonly Brush Schedule5Brush = (Brush)bc.ConvertFrom("#bf2035");
        #endregion
        public void CheckIsFocusRunning()
        {
            if (File.Exists(Common.MakeFilePath(Common.SESSION_FILE_NAME)))
            {
                //Get the Focus Session Object
                JSONHelper jSONHelper = new JSONHelper(Common.SESSION_FILE_NAME);
                CurrSession1 = jSONHelper.Get<Session>();
                IsFocusRunning = true;
            }
            else
            {
                IsFocusRunning = false;
            }
        }

        internal void StopFocusSession()
        {
            //Delete file and stop focus session
            JSONHelper jSONHelper = new JSONHelper(Common.SESSION_FILE_NAME);
            string jsonString = jSONHelper.GetJson<Session>();

            //Log Closing Session
            LoggingService.WriteAppLog("Session Closing : " + jsonString);

            File.Delete(Common.MakeFilePath(Common.SESSION_FILE_NAME));

            CurrSession1 = null;
            IsFocusRunning = false;
        }

        internal void StartFocusSession(Session session)
        {
            try
            {
                LoggingService.WriteAppLog("Start Focus Session Request Received");

            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
    }

    /// <summary>
    /// Utility class for triggering an event every 24 hours at a specified time of day
    /// </summary>
    public class DailyTrigger : IDisposable
    {
        /// <summary>
        /// Time of day (from 00:00:00) to trigger
        /// </summary>
        TimeSpan TriggerHour { get; }

        /// <summary>
        /// Task cancellation token source to cancel delayed task on disposal
        /// </summary>
        CancellationTokenSource CancellationToken { get; set; }

        /// <summary>
        /// Reference to the running task
        /// </summary>
        Task RunningTask { get; set; }

        /// <summary>
        /// Initiator
        /// </summary>
        /// <param name="hour">The hour of the day to trigger</param>
        /// <param name="minute">The minute to trigger</param>
        /// <param name="second">The second to trigger</param>
        public DailyTrigger(int hour, int minute = 0, int second = 0)
        {
            TriggerHour = new TimeSpan(hour, minute, second);
            CancellationToken = new CancellationTokenSource();
            RunningTask = Task.Run(async () =>
            {
                while (true)
                {
                    var triggerTime = DateTime.Today + TriggerHour - DateTime.Now;
                    if (triggerTime < TimeSpan.Zero)
                        triggerTime = triggerTime.Add(new TimeSpan(24, 0, 0));
                    await Task.Delay(triggerTime, CancellationToken.Token);
                    OnTimeTriggered?.Invoke();
                }
            }, CancellationToken.Token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            CancellationToken?.Cancel();
            CancellationToken?.Dispose();
            CancellationToken = null;
            RunningTask?.Dispose();
            RunningTask = null;
        }

        /// <summary>
        /// Triggers once every 24 hours on the specified time
        /// </summary>
        public event Action OnTimeTriggered;

        /// <summary>
        /// Finalized to ensure Dispose is called when out of scope
        /// </summary>
        ~DailyTrigger() => Dispose();
    }
}
