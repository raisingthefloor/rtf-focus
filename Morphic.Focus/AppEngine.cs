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
        private static readonly object schedulelocker = new object();
        private static readonly AppEngine _instance = new AppEngine();
        public static AppEngine Instance { get { return _instance; } }

        public const int NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED = 1; // valid range: 1...2

        AppEngine()
        {
            try
            {
                #region Categories
                //TODO - Add versioning to json to prevent copy of the file each time program loads
                //Copy Categories JSON file to user folder
                //if (!File.Exists(Common.MakeFilePath(Common.CATEGORIES_FILE_NAME)))
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

                //Set Default Blocklists
                SetDefaultBlocklists();
                #endregion

                #region Ongoing Sessions
                CheckIsFocusRunning();
                //foreach (var session in this.LstSession)
                //{
                //    session.PropertyChanged += Session_PropertyChanged;
                //}
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

        private void SetDefaultBlocklists()
        {
            try
            {
                if (UserPreferences != null)
                {
                    //Add No Entertain Blocklist
                    if (!UserPreferences.BlockLists.Any(p => p.IsDefault && p.Name == "No Entertain"))
                    {
                        AddNoEntertain();
                    }

                    //Add No Entertain or Social Blocklist
                    if (!UserPreferences.BlockLists.Any(p => p.IsDefault && p.Name == "No Entertain or Social"))
                    {
                        AddNoEntertainorSocial();
                    }

                    //Add Only Work, Comm OK
                    if (!UserPreferences.BlockLists.Any(p => p.IsDefault && p.Name == "Only Work, Comm OK"))
                    {
                        AddOnlyWorkCommOK();
                    }

                    //Add Only Work, No Comm Blocklist
                    if (!UserPreferences.BlockLists.Any(p => p.IsDefault && p.Name == "Only Work, No Comm"))
                    {
                        AddOnlyWorkNoComm();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void AddOnlyWorkNoComm()
        {
            Blocklist blocklist = new Blocklist() { Name = "Only Work, No Comm", IsDefault = true };

            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Notifications", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Email", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Communication (Not Email)", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Games", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Proxies", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Videos", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Social Media", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Shopping", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Porn", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "News", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Gambling", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Dating", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Productivity (exceptions)", IsActive = false });

            UserPreferences.BlockLists.Add(blocklist);
        }

        private void AddOnlyWorkCommOK()
        {
            Blocklist blocklist = new Blocklist() { Name = "Only Work, Comm OK", IsDefault = true };

            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Notifications", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Email", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Communication (Not Email)", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Games", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Proxies", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Videos", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Social Media", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Shopping", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Porn", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "News", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Gambling", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Dating", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Productivity (exceptions)", IsActive = true });

            UserPreferences.BlockLists.Add(blocklist);
        }

        private void AddNoEntertainorSocial()
        {
            Blocklist blocklist = new Blocklist() { Name = "No Entertain or Social", IsDefault = true };

            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Notifications", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Email", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Communication (Not Email)", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Games", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Proxies", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Videos", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Social Media", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Shopping", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Porn", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "News", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Gambling", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Dating", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Productivity (exceptions)", IsActive = false });

            UserPreferences.BlockLists.Add(blocklist);
        }

        private void AddNoEntertain()
        {
            Blocklist blocklist = new Blocklist() { Name = "No Entertain", IsDefault = true };

            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Notifications", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Email", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Communication (Not Email)", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Games", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Proxies", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Videos", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Social Media", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Shopping", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Porn", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "News", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Gambling", IsActive = true });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Dating", IsActive = false });
            blocklist.Blockcategories.Add(new Blockcategory() { Name = "Productivity (exceptions)", IsActive = false });

            UserPreferences.BlockLists.Add(blocklist);
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

        private DailyTrigger? schTrigger1 = null;
        private DailyTrigger? schTrigger2 = null;
        private DailyTrigger? schTrigger3 = null;
        private DailyTrigger? schTrigger4 = null;
        private DailyTrigger? schTrigger5 = null;

        public DailyTrigger? SchTrigger1 { get => schTrigger1; set => schTrigger1 = value; }
        public DailyTrigger? SchTrigger2 { get => schTrigger2; set => schTrigger2 = value; }
        public DailyTrigger? SchTrigger3 { get => schTrigger3; set => schTrigger3 = value; }
        public DailyTrigger? SchTrigger4 { get => schTrigger4; set => schTrigger4 = value; }
        public DailyTrigger? SchTrigger5 { get => schTrigger5; set => schTrigger5 = value; }

        public void ResetSchedule(int scheduleNumber)
        {
            try
            {
                lock (schedulelocker)
                {
                    switch (scheduleNumber)
                    {
                        case 1:
                            if (SchTrigger1 != null) SchTrigger1.Dispose();
                            SchTrigger1 = null;
                            SchTrigger1 = SetTrigger(UserPreferences.TodaysSchedule.Schedule1);
                            break;
                        case 2:
                            if (SchTrigger2 != null) SchTrigger2.Dispose();
                            SchTrigger2 = null;
                            SchTrigger2 = SetTrigger(UserPreferences.TodaysSchedule.Schedule2);
                            break;
                        case 3:
                            if (SchTrigger3 != null) SchTrigger3.Dispose();
                            SchTrigger3 = null;
                            SchTrigger3 = SetTrigger(UserPreferences.TodaysSchedule.Schedule3);
                            break;
                        case 4:
                            if (SchTrigger4 != null) SchTrigger4.Dispose();
                            SchTrigger4 = null;
                            SchTrigger4 = SetTrigger(UserPreferences.TodaysSchedule.Schedule4);
                            break;
                        case 5:
                            if (SchTrigger5 != null) SchTrigger5.Dispose();
                            SchTrigger5 = null;
                            SchTrigger5 = SetTrigger(UserPreferences.TodaysSchedule.Schedule5);
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        public void ResetSchedules()
        {
            try
            {
                lock (schedulelocker)
                {
                    //Reset Triggers
                    if (SchTrigger1 != null) SchTrigger1.Dispose();
                    if (schTrigger2 != null) schTrigger2.Dispose();
                    if (schTrigger3 != null) schTrigger3.Dispose();
                    if (schTrigger4 != null) schTrigger4.Dispose();
                    if (schTrigger5 != null) schTrigger5.Dispose();
                    SchTrigger1 = null;
                    schTrigger2 = null;
                    schTrigger3 = null;
                    schTrigger4 = null;
                    schTrigger5 = null;

                    //Set Triggers
                    //SetTrigger(schTrigger1, UserPreferences.TodaysSchedule.Schedule1);
                    //SetTrigger(schTrigger2, UserPreferences.TodaysSchedule.Schedule2);
                    //SetTrigger(schTrigger3, UserPreferences.TodaysSchedule.Schedule3);
                    //SetTrigger(schTrigger4, UserPreferences.TodaysSchedule.Schedule4);
                    //SetTrigger(schTrigger5, UserPreferences.TodaysSchedule.Schedule5);
                    SchTrigger1 = SetTrigger(UserPreferences.TodaysSchedule.Schedule1);
                    schTrigger2 = SetTrigger(UserPreferences.TodaysSchedule.Schedule2);
                    schTrigger3 = SetTrigger(UserPreferences.TodaysSchedule.Schedule3);
                    schTrigger4 = SetTrigger(UserPreferences.TodaysSchedule.Schedule4);
                    schTrigger5 = SetTrigger(UserPreferences.TodaysSchedule.Schedule5);
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private static bool StartTimeIsInNextFiveMinutes(DateTime startTime)
        {
            var startTimeTimeSpan = new TimeSpan(startTime.Hour, startTime.Minute, startTime.Second);
            var fiveMinuteEarlyTimeSpan = startTimeTimeSpan.Subtract(new TimeSpan(0, 5, 0));

            var today = DateTime.Today;
            var now = DateTime.Now;

            var adjustedStartTimeSpan = today + startTimeTimeSpan - now;
            var adjustedFiveMinuteEarlyTimeSpan = today + fiveMinuteEarlyTimeSpan - now;

            if (adjustedFiveMinuteEarlyTimeSpan < TimeSpan.Zero && adjustedStartTimeSpan >= TimeSpan.Zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private DailyTrigger? SetTrigger(Schedule schedule)
        {
            DailyTrigger? trigger = null;
            try
            {
                if (schedule != null) //Schedule should be non-null
                {
                    if (schedule.IsActive) //Schedule should be active
                    {
                        if (Helper.IsActiveToday(schedule)) //Schedule should be active today
                        {
                            //New Logic
                            DateTime triggerTime;
                            if (AppEngine.StartTimeIsInNextFiveMinutes(schedule.StartAt) == true)
                            {
                                triggerTime = schedule.StartAt;
                            }
                            else
                            {
                                triggerTime = schedule.StartAt.AddMinutes(-5); //Modal dialog to be shown 5 mins before start time
                            }

                            //Create trigger
                            trigger = new DailyTrigger(triggerTime.Hour, triggerTime.Minute, triggerTime.Second);

                            // NOTE: the time will be triggered 5 minutes ahead of the scheduled time (if possible) or right at the scheduled time (if the session needs to start less than five minutes from now)
                            trigger.OnTimeTriggered += () =>
                            {
                                //TODO - Review restart computer
                                //Case - If recently scheduled and it's start time is less than 5 mins from now then on scheduled time
                                if (DateTime.Now >= schedule.StartAt)
                                {
                                    if (ShallStartSession(schedule))
                                    {
                                        //Start the session
                                        double totalMinutes = (schedule.EndAt - DateTime.Now).TotalMinutes;

                                        StartFocusSession(new Session()
                                        {
                                            ActualStartTime = DateTime.Now,
                                            ActualEndTime = schedule.EndAt,
                                            BlockListName = schedule.BlockListName,

                                            //Break
                                            ProvideBreak = UserPreferences.Schedules.Schedulebreak.IsActive,
                                            BreakDuration = UserPreferences.Schedules.Schedulebreak.BreakDuration,
                                            BreakGap = UserPreferences.Schedules.Schedulebreak.BreakGap,

                                            //User & Log
                                            FocusType = "ScheduledSession",
                                            Schedule = schedule,
                                            SessionDuration = Convert.ToInt32(totalMinutes)
                                        });
                                    };
                                    return;
                                }

                                //Case decision to be taken 5 mins prior to start time
                                if (UserPreferences.General.dontGive5MinWarning) //Dialog not to be shown if disabled in General Setting
                                {
                                    //Schedule to start a session in 5 mins
                                    DateTime scheduleStartTime = schedule.StartAt; //Schedule start time

                                    FocusDispatchTimer focusDispatchTimer = new FocusDispatchTimer();
                                    focusDispatchTimer.Time = TimeSpan.FromMinutes(5);

                                    focusDispatchTimer.Timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                                    {
                                        //At schedule start time
                                        if (focusDispatchTimer.Time <= TimeSpan.Zero)
                                        {
                                            if (focusDispatchTimer.Timer != null) focusDispatchTimer.Timer.Stop();

                                            if (ShallStartSession(schedule))
                                            {
                                                //Start the session
                                                double totalMinutes = (schedule.EndAt - DateTime.Now).TotalMinutes;

                                                StartFocusSession(new Session()
                                                {
                                                    ActualStartTime = DateTime.Now,
                                                    ActualEndTime = schedule.EndAt,
                                                    BlockListName = schedule.BlockListName,

                                                    //Break
                                                    ProvideBreak = UserPreferences.Schedules.Schedulebreak.IsActive,
                                                    BreakDuration = UserPreferences.Schedules.Schedulebreak.BreakDuration,
                                                    BreakGap = UserPreferences.Schedules.Schedulebreak.BreakGap,

                                                    //User & Log
                                                    FocusType = "ScheduledSession",
                                                    Schedule = schedule,
                                                    SessionDuration = Convert.ToInt32(totalMinutes)
                                                });
                                            };

                                            ActiveFocusDispatchTimers.Remove(focusDispatchTimer);
                                        }
                                        focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                                    }, Application.Current.Dispatcher);

                                    focusDispatchTimer.Timer.Start();
                                    ActiveFocusDispatchTimers.Add(focusDispatchTimer);
                                }
                                else //Show dialog of focus about to start
                                {
                                    if (ShallStartSession(schedule))
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            ScheduledSessionModal scrScheduledSessionModal = new ScheduledSessionModal(schedule);
                                            scrScheduledSessionModal.Show();
                                            LoggingService.WriteAppLog("Scheduled Session Dialog Open");
                                        });
                                    }
                                }
                            };
                            #region Old Logic
                            ////Previous Logic - to be commented
                            //if (!UserPreferences.General.dontGive5MinWarning) //Dialog not to be shown if disabled in General Setting
                            //{
                            //    DateTime scheduleStartTime = schedule.StartAt.AddMinutes(-5); //Modal dialog to be shown 5 mins before start time

                            //    trigger = new DailyTrigger(scheduleStartTime.Hour, scheduleStartTime.Minute, scheduleStartTime.Second); // today at scheduled time

                            //    trigger.OnTimeTriggered += () =>
                            //    {
                            //        if (Session1 != null && Session1.BlockListName == schedule.BlockListName) return; //Dialog not to be shown if scheduled blocklist is already active

                            //        if (Session2 != null && Session2.BlockListName == schedule.BlockListName) return; //Dialog not to be shown if scheduled blocklist is already active

                            //        if (Session1 != null && Session2 != null) return; //Dialog not to be shown if two sessions are already active

                            //        Application.Current.Dispatcher.Invoke(() =>
                            //        {
                            //            ScheduledSessionModal scrScheduledSessionModal = new ScheduledSessionModal(schedule);
                            //            scrScheduledSessionModal.Show();
                            //            LoggingService.WriteAppLog("Scheduled Session Dialog Open");
                            //        });

                            //    };
                            //}
                            //else //Start schedule session at scheduled time
                            //{
                            //    DateTime scheduleStartTime = schedule.StartAt; //Schedule start time

                            //    trigger = new DailyTrigger(scheduleStartTime.Hour, scheduleStartTime.Minute, scheduleStartTime.Second); // today at scheduled time

                            //    trigger.OnTimeTriggered += () =>
                            //    {
                            //        double totalMinutes = (schedule.EndAt - DateTime.Now).TotalMinutes;

                            //        StartFocusSession(new Session()
                            //        {
                            //            ActualStartTime = DateTime.Now,
                            //            ActualEndTime = schedule.EndAt,
                            //            BlockListName = schedule.BlockListName,

                            //            //Break
                            //            ProvideBreak = UserPreferences.Schedules.Schedulebreak.IsActive,
                            //            BreakDuration = UserPreferences.Schedules.Schedulebreak.BreakDuration,
                            //            BreakGap = UserPreferences.Schedules.Schedulebreak.BreakGap,

                            //            //User & Log
                            //            FocusType = "ScheduledSession",
                            //            Schedule = schedule,
                            //            SessionDuration = Convert.ToInt32(totalMinutes)
                            //        });
                            //    };
                            //}
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
            return trigger;
        }

        private bool ShallStartSession(Schedule schedule)
        {
            try
            {
                //Dialog not to be shown if scheduled blocklist is already active
                foreach(var session in this.ActiveSessions)
                {
                    if (session.BlockListName == schedule.BlockListName)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            schedule.IsActive = false;
                        });
                        return false;
                    }
                }

                //Dialog not to be shown if the maximum number of sessions is/are already active
                if (this.ActiveSessions.Count >= AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        schedule.IsActive = false;
                    });
                    return false;
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
            return true;
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
        private List<FocusDispatchTimer> _activeFocusDispatchTimers = new List<FocusDispatchTimer>();
        public List<FocusDispatchTimer> ActiveFocusDispatchTimers { get => _activeFocusDispatchTimers; set => _activeFocusDispatchTimers = value; }

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

            //Migration Code - Keep it commented
            //foreach (Category category in CategoryCollection.Categories)
            //{
            //    foreach (string name in category.AppsAndWebsites)
            //    {
            //        if (name.Contains("(desktop app)"))
            //        {
            //            category.CollAppsAndWebsites.ActiveAppsAndWebsites.Add(new ActiveAppsAndWebsites()
            //            {
            //                IsActive = true,
            //                IsApp = true,
            //                Name = name.Replace("(desktop app)", "").Trim()
            //            });
            //        }
            //        else
            //        {
            //            category.CollAppsAndWebsites.ActiveAppsAndWebsites.Add(new ActiveAppsAndWebsites()
            //            {
            //                IsActive = true,
            //                IsApp = false,
            //                Name = "http://" + name
            //            });
            //        }
            //    }
            //}
            //jSONHelper.Save<CategoryCollection>(CategoryCollection);
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

        private List<Session> _activeSessions = new List<Session>();
        //public List<Session> ActiveSessions = new List<Session>();
        public List<Session> ActiveSessions { get => _activeSessions; set => _activeSessions = value; }

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

        // NOTE: these two properties are helpers to enable easy binding; in the future, we should remove these and safely address the sessions by index in UI binding
        public Session? ActiveSessions_NullableItem0 // first session (i.e. at index 0)
        {
            get
            {
                if (this.ActiveSessions.Count >= 1)
                {
                    return this.ActiveSessions[0];
                }
                else
                {
                    return null;
                }
            }
        }
        //
        public Session? ActiveSessions_NullableItem1 // second session (i.e. at index 1)
        {
            get
            {
                if (this.ActiveSessions.Count >= 2)
                {
                    return this.ActiveSessions[1];
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsFocusWithBreaks
        {
            get
            {
                if (this.ActiveSessions.Count == 0)
                {
                    return false;
                }
                else
                {
                    var provideBreak = false;
                    foreach (var session in this.ActiveSessions)
                    {
                        if (session.ProvideBreak == true)
                        {
                            provideBreak = true;
                        }
                    }
                    return provideBreak;
                }
            }
        }

        public DateTime NextBreakTime
        {
            get
            {
                if (this.ActiveSessions.Count == 0)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    var earliestNextBreakTime = DateTime.MaxValue;
                    var foundEarlierNextBreakTime = false;
                    foreach (var session in this.ActiveSessions)
                    {
                        if (session.NextBreakTime < earliestNextBreakTime && session.NextBreakTime != DateTime.MinValue)
                        {
                            earliestNextBreakTime = session.NextBreakTime;
                            foundEarlierNextBreakTime = true;
                        }
                    }
                    
                    if (foundEarlierNextBreakTime == true)
                    {
                        return earliestNextBreakTime;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
            }
        }

        public DateTime NextBreakEndsTime
        {
            get
            {
                if (this.ActiveSessions.Count == 0)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    var earliestLastBreakStartTime = DateTime.MaxValue;
                    var foundEarlierLastBreakStartTime = false;
                    foreach (var session in this.ActiveSessions)
                    {
                        if (session.LastBreakStartTime < earliestLastBreakStartTime && session.LastBreakStartTime != DateTime.MinValue)
                        {
                            earliestLastBreakStartTime = session.LastBreakStartTime;
                            foundEarlierLastBreakStartTime = true;
                        }
                    }

                    if (foundEarlierLastBreakStartTime == true)
                    {
                        return earliestLastBreakStartTime;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
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
                    NotifyPropertyChanged("ActiveSessions");
                    NotifyPropertyChanged("ActiveSessions_NullableItem0");
                    NotifyPropertyChanged("ActiveSessions_NullableItem1");
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

        public string TimeTillNextBreakMM
        {
            get
            {
                return ((int)Math.Ceiling(TimeTillNextBreak.TotalMinutes)).ToString("0");
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
                    NotifyPropertyChanged("ActiveSessions");
                    NotifyPropertyChanged("ActiveSessions_NullableItem0");
                    NotifyPropertyChanged("ActiveSessions_NullableItem1");
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

        public string TimeTillNextBreakEndsMM
        {
            get
            {
                return new TimeSpan(0, (int)Math.Ceiling(TimeTillNextBreakEnds.TotalMinutes), 0).ToString("%m");
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
                    if (this.ActiveSessions.Count == 0)
                    {
                        // default
                        return false;
                    }
                    else
                    {
                        // if any session is not "focus till stop" then return false
                        foreach (var session in this.ActiveSessions)
                        {
                            if (session.SessionDuration != Int32.MaxValue) 
                            {
                                return false;
                            }
                        }

                        // if all sessions were "focus till stop" then return true
                        return true;
                    }
                }
            }
        }

        public Blocklist? GetSessionBlocklist(int index)
        {
            if (this.ActiveSessions.Count < index + 1)
            {
                return null;
            }

            var session = this.ActiveSessions[index];

            string blocklistName = session.Schedule != null ?
                        (session.Schedule.BlockListName != null ? session.Schedule.BlockListName : string.Empty)
                        : session.BlockListName;

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
                    else if (session.IsMarkedForClose)
                    {
                        //Log Closing Session
                        LoggingService.WriteAppLog("Session Closing - Restart marked for Close");

                        File.Delete(Common.MakeFilePath(sessionFile));
                        session = null;
                    }
                    else
                    {
                        InvokeStartSessionErrorDialog();
                        StartFocusSession(session);
                    }
                }
            }
        }

        private void InvokeStartSessionErrorDialog()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ErrorMessageModal errorMessageModal = new ErrorMessageModal()
                    {
                        TitleText = "A Focus session is active",
                        ContentText = $"You started a Focus session before you last logged out or restarted this computer."
                    };

                    errorMessageModal.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
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
                    //If it is not a scheduled session, start
                    //If it is a scheduled session and same blocklist is not already running, start
                    //If it is a scheduled session and same blocklist is already running, start and deactive today's schedule
                    if (session.Schedule == null || ShallStartSession(session.Schedule))
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

                        //If it is not a scheduled session, start
                        //If it is a scheduled session and same blocklist is not already running, start
                        //If it is a scheduled session and same blocklist is already running, start and deactive today's schedule
                        if (session.Schedule == null || ShallStartSession(session.Schedule))
                            StartFocusSession(session);

                        ActiveFocusDispatchTimers.Remove(focusDispatchTimer);
                    }
                    focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                focusDispatchTimer.Timer.Start();
                ActiveFocusDispatchTimers.Add(focusDispatchTimer);
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
                    if (this.ActiveSessions.Count >= AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED)
                    {
                        LoggingService.WriteAppLog("Session cannot be started. Maximum number of sessions already running.");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ErrorMessageModal errorMessageModal = new ErrorMessageModal()
                            {
                                TitleText = AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED + " Focus Session" + (AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED != 1 ? "s" : "") + " already running",
                                ContentText = "More than " + AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED + "Focus Sessions cannot run for the same time. Try closing one of the running Focus Sessions."
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

                    if (session.TurnONDND == true)
                    {
                        var targetFocusAssistState = FocusAssist.FocusAssistState.AlarmsOnly;

                        var numAttemptsToSetState = 3;
                        for (var iAttempt = 0; iAttempt < numAttemptsToSetState; iAttempt += 1)
                        {
                            var setStateResult = FocusAssist.SetState(targetFocusAssistState);
                            if (setStateResult.IsError == true)
                            {
                                LoggingService.WriteAppLog("Error: could not turn on Windows Focus Assist");
                            }

                            var getStateResult = FocusAssist.GetState();
                            if (getStateResult.IsError == true)
                            {
                                LoggingService.WriteAppLog("Error: could not determine if Windows Focus Assist was turned on successfully");
                            }
                            var focusAssistState = getStateResult.Value!;

                            // if we successfully turned on focus assist, break out of this loop
                            // NOTE: this is necessary because _sometimes_ Windows doesn't turn in focus assist when we ask it to (via the undocumented API call)
                            if (focusAssistState == targetFocusAssistState)
                            {
                                break;
                            }

                            // wait 100ms before the next attempt
                            if (iAttempt < numAttemptsToSetState - 1)
                            {
                                Thread.Sleep(100);
                            }
                            else
                            {
                                LoggingService.WriteAppLog("Error: could not turn on Windows Focus Assist successfully");
                            }
                        }
                    }

                    this.ActiveSessions.Add(session);
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
                    if (this.ActiveSessions.Count == 1) StartFocusToBreakTimer();
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
                foreach (Session session in this.ActiveSessions.ToList())
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
                if (session.TurnONDND == true)
                {
                    var numAttemptsToSetState = 3;
                    for (var iAttempt = 0; iAttempt < numAttemptsToSetState; iAttempt += 1)
                    {
                        var setStateResult = FocusAssist.SetState(FocusAssist.FocusAssistState.Off);
                        if (setStateResult.IsError == true)
                        {
                            LoggingService.WriteAppLog("Error: could not turn off Windows Focus Assist");
                        }

                        var getStateResult = FocusAssist.GetState();
                        if (getStateResult.IsError == true)
                        {
                            LoggingService.WriteAppLog("Error: could not determine if Windows Focus Assist was turned off successfully");
                        }
                        var focusAssistState = getStateResult.Value!;

                        // if we successfully turned off focus assist, break out of this loop
                        // NOTE: this is necessary because _sometimes_ Windows doesn't turn off focus assist when we ask it to (via the undocumented API call)
                        if (focusAssistState == FocusAssist.FocusAssistState.Off)
                        {
                            break;
                        }

                        // wait 100ms before the next attempt
                        if (iAttempt < numAttemptsToSetState - 1)
                        {
                            Thread.Sleep(100);
                        }
                        else
                        {
                            LoggingService.WriteAppLog("Error: could not turn off Windows Focus Assist successfully");
                        }
                    }
                }

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
                                    new StopFocusRestartModal()
                                    {
                                        ApplicableBlocklist = blockList,
                                        Session = session
                                    }.ShowDialog(); //Show Stop Focus Restart Modal
                                });
                            }
                            else if (blockList.Penalty == Penalty.Type)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    LoggingService.WriteAppLog("Show Stop Focus Random Char Modal");
                                    new StopFocusRandomCharModal()
                                    {
                                        PenaltyValue = blockList.PenaltyValue,
                                        ApplicableBlocklist = blockList,
                                        Session = session
                                    }.ShowDialog(); //Show Stop Focus Restart Modal
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
                this.ActiveSessions.Remove(session);
                session = null;
                IsFocusRunning = this.ActiveSessions.Count > 0;

                if (this.ActiveSessions.Count == 0) StopFocusTimer();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (FocusStatus.Visibility == Visibility.Visible)
                    {
                        FocusStatus.Hide();
                        FocusMain.Show();
                    }
                });
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
                    foreach (Session session in this.ActiveSessions.ToList())
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
                foreach (Session session in this.ActiveSessions.ToList())
                {
                    session.LastStartTime = datetimeNow;
                    session.IsBreakRunning = false;
                }

                if (this.ActiveSessions.Count >= 1) StartFocusToBreakTimer();
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
                if (ActiveFocusDispatchTimers.Exists(p => p.IsCountdownTimer))
                {
                    focusDispatchTimer = ActiveFocusDispatchTimers.Find(p => p.IsCountdownTimer);
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
                    {
                        TimeTillNextBreak = focusDispatchTimer.Time = new TimeSpan(0, Common.LongBreakDuration, 0); //Long Break TODO Review from specs
                    }
                    else
                    {
                        TimeTillNextBreak = focusDispatchTimer.Time = (NextBreakTime - DateTime.Now).Duration();
                    }
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

                            var sessionCompleted = false;

                            //Todo Review
                            //Stop focus session if the end time has reached
                            foreach (Session session in this.ActiveSessions.ToList())
                            {
                                if (session != null)
                                {
                                    if (session.SessionDuration > 0 && session.SessionDuration != Int32.MaxValue /* Int32.MaxValue means "no scheduled end to session" */)
                                    {
                                        if (DateTime.Now >= session.ActualStartTime.AddMinutes(session.SessionDuration))
                                        {
                                            OpenSessionCompletedModal(session); //Stop the Session if End time has reached
                                            sessionCompleted = true;
                                        }
                                    }
                                }
                            }

                            // NOTE: we should record the time that a break should start (either a short break or a long break) and use THAT time logic here to determine
                            //       if a short/long break dialog should be shown
                            //       [instead, we are using the legacy solution...but we're basing it on whether or not a session just ended]

                            //Open Short/Long Break Modal
                            if (sessionCompleted == false && this.ActiveSessions.Count > 0)
                            {
                                var showLongBreakModal = false;
                                foreach (var session in this.ActiveSessions)
                                {
                                    if ((DateTime.Now - session.LastStartTime).TotalMinutes >= Common.LongBreakDuration) //If focusing for more than 120 mins
                                    {
                                        showLongBreakModal = true;
                                    }
                                }

                                if (showLongBreakModal == true)
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

                            ActiveFocusDispatchTimers.Remove(focusDispatchTimer);
                        }
                    }
                    TimeTillNextBreak = focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                focusDispatchTimer.Timer.Start(); //Start Countdown timer
                ActiveFocusDispatchTimers.Add(focusDispatchTimer);
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
                if (ActiveFocusDispatchTimers.Exists(p => p.IsCountdownTimer))
                {
                    focusDispatchTimer = ActiveFocusDispatchTimers.Find(p => p.IsCountdownTimer);
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
                            foreach (Session session in this.ActiveSessions.ToList())
                            {
                                if (session != null)
                                {
                                    if (session.SessionDuration > 0 && session.SessionDuration != Int32.MaxValue /* Int32.MaxValue means "no scheduled end to session" */)
                                    {
                                        if (DateTime.Now >= session.ActualStartTime.AddMinutes(session.SessionDuration))
                                        {
                                            OpenSessionCompletedModal(session); //Stop the Session if End time has reached
                                        }
                                    }
                                }
                            }

                            //Open Break End Modal
                            if (ActiveSessions.Count > 0)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    LoggingService.WriteAppLog("Break end");
                                    EndofBreakModal.ShowDialog(); //Show long break modal
                                });
                            }

                            ActiveFocusDispatchTimers.Remove(focusDispatchTimer);
                        }
                    }
                    TimeTillNextBreakEnds = focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                focusDispatchTimer.Timer.Start(); //Start Countdown timer
                ActiveFocusDispatchTimers.Add(focusDispatchTimer);
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
                if (ActiveFocusDispatchTimers.Exists(p => p.IsCountdownTimer))
                {
                    focusDispatchTimer = ActiveFocusDispatchTimers.Find(p => p.IsCountdownTimer);

                    //Reset existing time & timer
                    if (focusDispatchTimer.Timer != null) focusDispatchTimer.Timer.Stop();
                    TimeTillNextBreak = focusDispatchTimer.Time = TimeSpan.Zero;

                    ActiveFocusDispatchTimers.Remove(focusDispatchTimer);
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

                        ActiveFocusDispatchTimers.Remove(focusDispatchTimer);
                    }
                    focusDispatchTimer.Time = focusDispatchTimer.Time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                focusDispatchTimer.Timer.Start();
                ActiveFocusDispatchTimers.Add(focusDispatchTimer);
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
                // OBSERVATION: this code appears to run in a loop so that it will trigger every day (until the application stops running)
                while (true)
                {
                    try
                    {
                        //Example we need to schedule for 5pm and current time is 6pm
                        var triggerTime = DateTime.Today + TriggerHour - DateTime.Now; //triggerTime = 00 hr + 17 hr - 18 hr = -1 hr
                        // if the trigger time has passed within the last 5 seconds, re-schedule it to start right now
                        // NOTE: this is a patch, primarily designed to deal with scenarios where we tried to schedule "5 minutes ahead of time" but barely missed that window
                        if (triggerTime.Add(new TimeSpan(0, 0, 5)) > TimeSpan.Zero && triggerTime < TimeSpan.Zero)
                        {
                            triggerTime = TimeSpan.Zero;
                        }
                        else if (triggerTime < TimeSpan.Zero) //if triggerTime < 0
                        {
                            triggerTime = triggerTime.Add(new TimeSpan(24, 0, 0)); //triggerTime = -1 hr + 24 hr = 23 hr
                        }

                        await Task.Delay(triggerTime, CancellationToken.Token); //wait for 23 hr. Currenttime = 6pm + 23 hr = 5pm next day
                        OnTimeTriggered?.Invoke(); //trigger at 5 pm next day

                        await Task.Delay(new TimeSpan(0, 0, 10), CancellationToken.Token); //wait for 10 seconds before attempting next trigger time calculation
                    }
                    catch
                    {
                        // OBSERVATION: this "break" seems to exist, in theory, to break out of this thread if the cancellation token is cancelled; note that
                        //              this will also presumably exit if the invoked routine throws an exception (resulting in tomorrow's scheduled session _not_ being triggered)
                        break;
                    }
                }
            }, CancellationToken.Token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            try
            {
                CancellationToken?.Cancel();
                CancellationToken?.Dispose();
                CancellationToken = null;

                RunningTask?.Dispose();
                RunningTask = null;
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
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

