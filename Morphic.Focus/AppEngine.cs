using Morphic.Data.Models;
using Morphic.Data.Services;
using Morphic.Focus.Screens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Morphic.Focus
{
    public class AppEngine : BaseClass
    {
        #region AppEngine Instance
        private static readonly object locker = new object();
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
                //Session1.PropertyChanged += Session1_PropertyChanged;
                //Session2.PropertyChanged += Session2_PropertyChanged;
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

        private void CurrSession_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                Session session = (Session)sender;
                string filePath = Common.GetSessionFilePath(session);

                //1. Get Focus Session file 
                JSONHelper jSONHelper = new JSONHelper(filePath);

                //2. If the file is not found, a new settings file is created
                if (session != null)
                {
                    jSONHelper.Save<Session>(session);
                }
                else
                {
                    File.Delete(Common.MakeFilePath(filePath));
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
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
                                if (Session1 != null && Session1.BlockListName == schedule.BlockListName) return; //Dialog not to be shown if scheduled blocklist is already active

                                if (Session2 != null && Session2.BlockListName == schedule.BlockListName) return; //Dialog not to be shown if scheduled blocklist is already active

                                if (Session1 != null && Session2 != null) return; //Dialog not to be shown if two sessions are already active

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

        LockedScreenNonModal _lockedScreenNonModal = null;

        public LockedScreenNonModal LockedScreenNonModal
        {
            get
            {
                if (_lockedScreenNonModal == null)
                {
                    _lockedScreenNonModal = new LockedScreenNonModal() { Topmost = true, ShowInTaskbar = false };
                }

                return _lockedScreenNonModal;
            }
            set => _lockedScreenNonModal = value;
        }

        LongBreakModal _longBreakModal = null;
        public LongBreakModal LongBreakModal
        {
            get
            {
                if (_longBreakModal == null)
                {
                    _longBreakModal = new LongBreakModal() { ShowInTaskbar = false };
                }

                return _longBreakModal;
            }
            set => _longBreakModal = value;
        }

        ShortBreakModal _shortBreakModal = null;
        public ShortBreakModal ShortBreakModal
        {
            get
            {
                if (_shortBreakModal == null)
                {
                    _shortBreakModal = new ShortBreakModal() { ShowInTaskbar = false };
                }

                return _shortBreakModal;
            }
            set => _shortBreakModal = value;
        }


        EndofBreakModal _endofBreakModal = null;
        public EndofBreakModal EndofBreakModal
        {
            get
            {
                if (_endofBreakModal == null)
                {
                    _endofBreakModal = new EndofBreakModal() { ShowInTaskbar = false };
                }

                return _endofBreakModal;
            }
            set => _endofBreakModal = value;
        }

        //SessionCompletedModal _sessionCompletedModal = null;
        //public SessionCompletedModal SessionCompletedModal
        //{
        //    get
        //    {
        //        if (_sessionCompletedModal == null)
        //        {
        //            _sessionCompletedModal = new SessionCompletedModal() { ShowInTaskbar = false };
        //        }

        //        return _sessionCompletedModal;
        //    }
        //    set => _sessionCompletedModal = value;
        //}
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

        #region ListFocusDispatchTimer
        private List<FocusDispatchTimer> _lstFocusDispatchTimer = new List<FocusDispatchTimer>();
        public List<FocusDispatchTimer> LstFocusDispatchTimer { get => _lstFocusDispatchTimer; set => _lstFocusDispatchTimer = value; }

        #endregion

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

        #region Session

        private List<Session> _lstSession = new List<Session>();
        public List<Session> LstSession { get => _lstSession; set => _lstSession = value; }

        private bool _isFocusRunning = false;
        public bool IsFocusRunning
        {
            get
            {
                return _isFocusRunning;
            }
            set
            {
                if (value != _isFocusRunning)
                {
                    _isFocusRunning = value;
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

        public DateTime NextBreakTime
        {
            get
            {
                if (LstSession.Count == 0)
                    return DateTime.MinValue;
                else //if (LstSession.Count == 1)
                    return Session1.NextBreakTime;
                //else if (LstSession.Count == 2)
                //    return new[] { Session1.NextBreakTime, Session2.NextBreakTime }.Min();
                //else
                //    return DateTime.MinValue;
            }
        }

        public DateTime NextBreakEndsTime
        {
            get
            {
                if (LstSession.Count == 0)
                    return DateTime.MinValue;
                else //if (LstSession.Count == 1)
                    return Session1.LastBreakStartTime.AddMinutes(Session1.BreakDuration);
                //else if (LstSession.Count == 2)
                //    return new[] { Session1.NextBreakTime, Session2.NextBreakTime }.Min();
                //else
                //    return DateTime.MinValue;
            }
        }

        private TimeSpan _timeTillNextBreak = TimeSpan.Zero;
        public TimeSpan TimeTillNextBreak
        {
            get
            {
                return _timeTillNextBreak;
            }
            set
            {
                if (value != _timeTillNextBreak)
                {
                    _timeTillNextBreak = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("Session1");
                    NotifyPropertyChanged("Session2");
                }
            }
        }

        public string TimeTillNextBreakHHMM
        {
            get
            {
                return new TimeSpan(0, (int)Math.Ceiling(TimeTillNextBreak.TotalMinutes), 0).ToString("hh':'mm");
            }
        }

        private TimeSpan _timeTillNextBreakEnds = TimeSpan.Zero;
        public TimeSpan TimeTillNextBreakEnds
        {
            get
            {
                return _timeTillNextBreakEnds;
            }
            set
            {
                if (value != _timeTillNextBreakEnds)
                {
                    _timeTillNextBreakEnds = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("Session1");
                    NotifyPropertyChanged("Session2");
                }
            }
        }

        public string TimeTillNextBreakEndsHHMM
        {
            get
            {
                return new TimeSpan(0, (int)Math.Ceiling(TimeTillNextBreakEnds.TotalMinutes), 0).ToString("hh':'mm");
            }
        }
        public bool IsFocusTillStop
        {
            get
            {
                if (!IsFocusRunning)
                {
                    return false;
                }
                else
                {

                    if (Session1 != null && Session2 == null) //If only session 1 is running
                    {
                        return Session1.SessionDuration == 0; //Is the session duration set to zero
                    }
                    else if (Session2 != null && Session1 == null) //If only session 2 is running
                    {
                        return Session2.SessionDuration == 0; //Is the session duration set to zero
                    }
                    else if (Session1 != null && Session2 != null) //Both Sessions are running
                    {
                        //Return true only when both sessions are of type Focus Till Stop
                        return Session1.SessionDuration == 0 && Session2.SessionDuration == 0;
                    }
                    return false; //default
                }
            }
        }

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

        #region Action Methods

        /// <summary>
        /// Upon Application Start
        /// </summary>
        public void CheckIsFocusRunning()
        {
            string[] sessionFiles = Common.GetSessionFiles();
            foreach (string sessionFile in sessionFiles)
            {
                Session? session = null;

                //Get the Session data from the file
                //Get the Focus Session Object
                JSONHelper jSONHelper = new JSONHelper(sessionFile);
                session = jSONHelper.Get<Session>();

                if (session != null)
                {
                    if (DateTime.Now > session.ActualStartTime.AddMinutes(session.SessionDuration))
                    {
                        //Log Closing Session
                        LoggingService.WriteAppLog("Session Closing");

                        File.Delete(Common.MakeFilePath(sessionFile));
                        session = null;
                    }
                    else
                    {
                        StartFocusSession(session);
                    }
                }
            }
        }

        /// <summary>
        /// Delayed Session Start
        /// </summary>
        /// <param name="session"></param>
        /// <param name="minutesLeft"></param>
        internal void StartFocusSession(Session session, int minutesLeft)
        {
            try
            {
                //Start Session immediately
                if (minutesLeft == 0)
                {
                    StartFocusSession(session);
                    return;
                }

                //Start Session after x mins
                FocusDispatchTimer focusDispatchTimer = new FocusDispatchTimer();

                //Countdown - Start Session after x min
                focusDispatchTimer.Time = TimeSpan.FromMinutes(minutesLeft);

                focusDispatchTimer.Timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    //Start Session after x mins
                    if (focusDispatchTimer.Time <= TimeSpan.Zero)
                    {
                        if (focusDispatchTimer.Timer != null) focusDispatchTimer.Timer.Stop();

                        StartFocusSession(session);

                        LstFocusDispatchTimer.Remove(focusDispatchTimer);
                    }
                    focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                focusDispatchTimer.Timer.Start();
                LstFocusDispatchTimer.Add(focusDispatchTimer);
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        internal void StartFocusSession(Session session)
        {
            lock (locker)
            {
                try
                {
                    session.LastStartTime = DateTime.Now;

                    LoggingService.WriteAppLog("Start Focus Session Request Received");

                    //Do not start a session, if two sessions are already running
                    if (LstSession.Count >= 2)
                    {
                        LoggingService.WriteAppLog("Session cannot be started. Two Sessions already running.");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ErrorMessageModal errorMessageModal = new ErrorMessageModal()
                            {
                                TitleText = "Two Focus sessions already running",
                                ContentText = "More than two Focus Sessions cannot run for the same time. Try closing one of the running Focus Session."
                            };
                            errorMessageModal.ShowDialog();

                        });

                        return;
                    }

                    string jsonString = string.Empty;

                    //Fill Session variables
                    //save session data
                    //Add to json file
                    JSONHelper jSONHelper = new JSONHelper(Common.GetSessionFilePath(session));
                    jsonString = jSONHelper.Save(session);

                    LstSession.Add(session);
                    session.PropertyChanged += CurrSession_PropertyChanged;
                    session.IsBreakRunning = false;
                    IsFocusRunning = true;

                    ////TODO - Review this
                    //if (IsFocusRunning)
                    //{
                    //    //Log Session
                    //    LoggingService.WriteAppLog("Session Restarted : " + jsonString);
                    //}
                    //else
                    //{
                    //    //Log Session
                    //    LoggingService.WriteAppLog("Session Started : " + jsonString);
                    //}

                    //Reset Break Timer if this is the first session that has started
                    if (LstSession.Count == 1) StartFocusToBreakTimer();
                }
                catch (Exception ex)
                {
                    LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                }
            }
        }

        internal void StopAllFocusSession()
        {
            try
            {
                foreach (Session session in LstSession.ToList())
                {
                    OpenSessionCompletedModal(session);
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        internal void OpenSessionCompletedModal(Session session)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoggingService.WriteAppLog("Open Session Completed Modal");
                    new SessionCompletedModal(session).ShowDialog(); //Show session completed modal
                });
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        internal void EndSession(Session session, bool isEarlyEnding = false)
        {
            try
            {
                if (isEarlyEnding)
                {
                    if (string.IsNullOrWhiteSpace(session.BlockListName))
                    {
                        EndSession(session);
                    }
                    else
                    {
                        if (UserPreferences.BlockLists.Any(p => p.Name.ToLowerInvariant() == session.BlockListName.ToLowerInvariant()))
                        {
                            Blocklist blockList = UserPreferences.BlockLists.Where(p => p.Name.ToLowerInvariant() == session.BlockListName.ToLowerInvariant()).First();

                            if (blockList.Penalty == Penalty.None)
                            { 
                                EndSession(session);
                            }
                            else if (blockList.Penalty == Penalty.Restart)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    LoggingService.WriteAppLog("Show Stop Focus Restart Modal");
                                    new StopFocusRestartModal().ShowDialog(); //Show Stop Focus Restart Modal
                                });
                            }
                            else if (blockList.Penalty == Penalty.Type)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    LoggingService.WriteAppLog("Show Stop Focus Random Char Modal");
                                    new StopFocusRandomCharModal().ShowDialog(); //Show Stop Focus Restart Modal
                                });
                            }
                        }
                    }
                }
                else
                {
                    EndSession(session);
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        internal void EndSession(Session session)
        {
            try
            {
                //Delete file and stop focus session
                JSONHelper jSONHelper = new JSONHelper(Common.GetSessionFilePath(session));
                string jsonString = jSONHelper.GetJson<Session>();

                //Log Closing Session
                LoggingService.WriteAppLog("Session Closing : " + jsonString);

                File.Delete(Common.GetSessionFilePath(session));

                session.PropertyChanged -= CurrSession_PropertyChanged;
                LstSession.Remove(session);
                session = null;
                IsFocusRunning = LstSession.Count > 0;

                if (LstSession.Count == 0) StopFocusTimer();

                FocusStatus.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        internal void EndSessionRemindInMins(int mins)
        {
            lock (locker)
            {
                StartFocusToBreakTimer(mins);
            }
        }

        internal void StartBreakSequence(int longBreak = 0)
        {
            try
            {
                LoggingService.WriteAppLog("Start Break Session Request Received");

                lock (locker)
                {
                    DateTime datetimeNow = DateTime.Now; //Start of Break time for one or both sessions

                    //1 - Set Last Break Start time for all running sessions
                    foreach (Session session in LstSession.ToList())
                    {
                        session.LastBreakStartTime = datetimeNow;
                        session.IsBreakRunning = true;
                    }

                    //For first minute start
                    if (UserPreferences.General.blockScreen1stMinofBreak)
                    {
                        OpenLockComputerNonModalScreen();
                    }

                    StartBreakToFocusTimer(longBreak);
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        internal void ShortBreakRemindInMins(int mins)
        {
            lock (locker)
            {
                StartFocusToBreakTimer(mins);
            }
        }

        internal void StartFocusSequence()
        {
            LoggingService.WriteAppLog("Start Focus Session Request Received");

            lock (locker)
            {
                DateTime datetimeNow = DateTime.Now; //Start of Break time for one or both sessions

                //1 - Set Last Break Start time for all running sessions
                foreach (Session session in LstSession.ToList())
                {
                    session.LastStartTime = datetimeNow;
                    session.IsBreakRunning = false;
                }

                if (LstSession.Count >= 1) StartFocusToBreakTimer();
            }
        }

        /// <summary>
        /// breakGap = 0 (default)
        /// breakGap <> 0, a focus session is extended by clicking 5,10,15 min buttons
        /// </summary>
        /// <param name="isBreak"></param>
        /// <param name="breakGap"></param>
        private void StartFocusToBreakTimer(int breakGap = 0)
        {
            try
            {
                FocusDispatchTimer? focusDispatchTimer = null;

                //Get countdown dispatch timer
                if (LstFocusDispatchTimer.Exists(p => p.IsCountdownTimer))
                {
                    focusDispatchTimer = LstFocusDispatchTimer.Find(p => p.IsCountdownTimer);
                }
                else
                {
                    focusDispatchTimer = new FocusDispatchTimer() { IsCountdownTimer = true };
                }

                //Reset existing time & timer
                if (focusDispatchTimer.Timer != null) focusDispatchTimer.Timer.Stop();
                focusDispatchTimer.Time = TimeSpan.Zero;

                //Set countdown time
                if (breakGap == 0)
                {
                    if (NextBreakTime == DateTime.MinValue)
                        TimeTillNextBreak = focusDispatchTimer.Time = new TimeSpan(0, 60, 0); //Long Break TODO Review from specs
                    else
                        TimeTillNextBreak = focusDispatchTimer.Time = (NextBreakTime - DateTime.Now).Duration();
                }
                else //Extended Session from break dialog
                {
                    TimeTillNextBreak = focusDispatchTimer.Time = new TimeSpan(0, breakGap, 0);
                }

                //TODO Review
                //Set Break Type
                //if (isBreak)
                //{
                //    focusDispatchTimer.Type = TimeTillNextBreak.TotalMinutes == 60 ? TimerType.CountdownToLongBreak : TimerType.CountdownToShortBreak;
                //}

                focusDispatchTimer.Timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    
                    //Timer finishes
                    if (focusDispatchTimer.Time <= TimeSpan.Zero)
                    {
                        lock (locker)
                        {
                            //Stop the timer
                            if (focusDispatchTimer.Timer != null)
                                focusDispatchTimer.Timer.Stop();

                            //Todo Review
                            //Stop focus session if the end time has reached
                            foreach (Session session in LstSession.ToList())
                            {
                                if (session != null)
                                {
                                    if (session.SessionDuration > 0)
                                    {
                                        if (DateTime.Now >= session.ActualStartTime.AddMinutes(session.SessionDuration))
                                        {
                                            OpenSessionCompletedModal(session); //Stop the Session if End time has reached
                                        }
                                    }
                                }
                            }

                            //Open Short/Long Break Modal
                            if (LstSession.Count > 0)
                            {
                                
                                if ((DateTime.Now - Session1.LastStartTime).TotalMinutes >= Common.LongBreakDuration) //If focussing for more than 120 mins
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        LoggingService.WriteAppLog("Show long break modal");
                                        LongBreakModal.ShowDialog(); //Show long break modal
                                    });
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        LoggingService.WriteAppLog("Show short break modal");
                                        ShortBreakModal.ShowDialog(); //Show short break modal
                                    });
                                }
                            }

                            LstFocusDispatchTimer.Remove(focusDispatchTimer);
                        }
                    }
                    TimeTillNextBreak = focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                focusDispatchTimer.Timer.Start(); //Start Countdown timer
                LstFocusDispatchTimer.Add(focusDispatchTimer);
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }

        }

        private void StartBreakToFocusTimer(int breakDuration = 0)
        {
            try
            {
                FocusDispatchTimer? focusDispatchTimer = null;

                //Get countdown dispatch timer
                if (LstFocusDispatchTimer.Exists(p => p.IsCountdownTimer))
                {
                    focusDispatchTimer = LstFocusDispatchTimer.Find(p => p.IsCountdownTimer);
                }
                else
                {
                    focusDispatchTimer = new FocusDispatchTimer() { IsCountdownTimer = true };
                }

                //Reset existing time & timer
                if (focusDispatchTimer.Timer != null) focusDispatchTimer.Timer.Stop();
                focusDispatchTimer.Time = TimeSpan.Zero;

                //Set countdown time
                if (breakDuration == 0) //default
                {
                    if (NextBreakEndsTime == DateTime.MinValue)
                        TimeTillNextBreakEnds = focusDispatchTimer.Time = new TimeSpan(0, 0, 0); //Todo Review
                    else
                        TimeTillNextBreakEnds = focusDispatchTimer.Time = (NextBreakEndsTime - DateTime.Now).Duration();
                }
                else
                {
                    TimeTillNextBreakEnds = focusDispatchTimer.Time = new TimeSpan(0, breakDuration, 0); //Longer Break or break extension
                }
                //Set Break Type

                focusDispatchTimer.Type = TimerType.CountdownToBreakEnd;

                focusDispatchTimer.Timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    //Timer finishes
                    if (focusDispatchTimer.Time <= TimeSpan.Zero)
                    {
                        lock (locker)
                        {
                            //Stop the timer
                            if (focusDispatchTimer.Timer != null)
                                focusDispatchTimer.Timer.Stop();

                            //Stop focus session if the end time has reached
                            foreach (Session session in LstSession.ToList())
                            {
                                if (session != null)
                                {
                                    if (session.SessionDuration > 0)
                                    {
                                        if (DateTime.Now >= session.ActualStartTime.AddMinutes(session.SessionDuration))
                                        {
                                            OpenSessionCompletedModal(session); //Stop the Session if End time has reached
                                        }
                                    }
                                }
                            }

                            //Open Break End Modal
                            if (LstSession.Count > 0)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    LoggingService.WriteAppLog("Break end");
                                    EndofBreakModal.ShowDialog(); //Show long break modal
                                });
                            }

                            LstFocusDispatchTimer.Remove(focusDispatchTimer);
                        }
                    }
                    TimeTillNextBreakEnds = focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                focusDispatchTimer.Timer.Start(); //Start Countdown timer
                LstFocusDispatchTimer.Add(focusDispatchTimer);
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }

        }

        private void StopFocusTimer()
        {
            try
            {
                FocusDispatchTimer? focusDispatchTimer = null;

                //Get countdown dispatch timer
                if (LstFocusDispatchTimer.Exists(p => p.IsCountdownTimer))
                {
                    focusDispatchTimer = LstFocusDispatchTimer.Find(p => p.IsCountdownTimer);

                    //Reset existing time & timer
                    if (focusDispatchTimer.Timer != null) focusDispatchTimer.Timer.Stop();
                    TimeTillNextBreak = focusDispatchTimer.Time = TimeSpan.Zero;

                    LstFocusDispatchTimer.Remove(focusDispatchTimer);
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        internal void EndBreakRemindInMins(int mins)
        {
            lock (locker)
            {
                StartBreakToFocusTimer(mins);
            }
        }

        private void OpenLockComputerNonModalScreen()
        {
            try
            {
                //Show Locked Screen
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LockedScreenNonModal.Show();
                });

                //Show lock screen for 1 min
                FocusDispatchTimer focusDispatchTimer = new FocusDispatchTimer();
                focusDispatchTimer.Time = TimeSpan.FromMinutes(1);

                focusDispatchTimer.Timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    //Close lock screen after 1 min
                    if (focusDispatchTimer.Time <= TimeSpan.Zero)
                    {
                        if (focusDispatchTimer.Timer != null) focusDispatchTimer.Timer.Stop();

                        LockedScreenNonModal.Hide();

                        LstFocusDispatchTimer.Remove(focusDispatchTimer);
                    }
                    focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                focusDispatchTimer.Timer.Start();
                LstFocusDispatchTimer.Add(focusDispatchTimer);
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
        #endregion

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

    public class FocusDispatchTimer
    {
        public DispatcherTimer Timer { get; set; } = null;
        public TimeSpan Time { get; set; } = TimeSpan.Zero;

        public bool IsCountdownTimer { get; set; } = false;
        public TimerType Type { get; set; } = TimerType.Other;

    }

    public enum TimerType
    {
        CountdownToShortBreak,
        CountdownToLongBreak,
        CountdownToBreakEnd,
        Other
    }
}

