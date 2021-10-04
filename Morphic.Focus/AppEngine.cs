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
using Morphic.Data.JSONService;

namespace Morphic.Focus
{
    public class AppEngine : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Singleton
        private static readonly AppEngine _instance = new AppEngine();
        public static AppEngine Instance { get { return _instance; } }

        AppEngine()
        {
            CheckIsFocusRunning();
        }

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
            LoggingService.WriteToLog("Session Closing : " + jsonString);

            File.Delete(Common.MakeFilePath(Common.SESSION_FILE_NAME));

            CurrSession1 = null;
            IsFocusRunning = false;
        }

        private Session? _currSession1 = null;
        public Session? CurrSession1 { get => _currSession1; set => _currSession1 = value; }

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
