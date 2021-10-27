using Morphic.Data.Models;
using Morphic.Data.Services;
using Morphic.Focus.Screens;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Morphic.Focus
{
    public class AppEngine : BaseClass
    {
        #region AppEnding Instance
        private static readonly AppEngine _instance = new AppEngine();
        public static AppEngine Instance { get { return _instance; } }

        AppEngine()
        {
            System.IO.File.Copy(Path.Combine(AppContext.BaseDirectory, Common.CATEGORIES_FILE_NAME), Common.MakeFilePath(Common.CATEGORIES_FILE_NAME), true);
            GetFocusSettings();

            GetCategoryies();

            CheckIsFocusRunning();

            UserPreferences.PropertyChanged += UserPreferences_PropertyChanged;
            UserPreferences.Schedules.PropertyChanged += Schedules_PropertyChanged;
            SetTodaysSchedule();
        }

        private void Schedules_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SetTodaysSchedule(true); //Since original schedules are altered, force reset today's schedules
        }

        private void UserPreferences_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SetFocusSettings();
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
            //Get Focus Preferences from the Settings.json file
            //If the file is not found, a new settings file is created
            //Settings are persisted in memomy as long as the Focus Tool is running

            //1. Get Focus Preferences from the Settings.json file
            JSONHelper jSONHelper = new JSONHelper(Common.CATEGORIES_FILE_NAME);
            CategoryCollection = jSONHelper.Get<CategoryCollection>();

            //2. If the file is not found, a new settings file is created
            if (CategoryCollection == null)
            {
                CategoryCollection = new CategoryCollection();
                jSONHelper.Save<CategoryCollection>(CategoryCollection);
            }
        }

        #endregion

        #region Selected Blocklist in Settings
        private Blocklist _selectedBlockList;
        public Blocklist SelectedBlockList
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
        #endregion

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

    }
}
