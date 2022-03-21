using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for ScheduledSessionModal.xaml
    /// </summary>
    public partial class ScheduledSessionModal : Window, INotifyPropertyChanged
    {
        #region Private members, AppEngine and Constructor
        AppEngine _engine;
        private string _titleText = string.Empty;
        private string _buttonText = string.Empty;
        
        DispatcherTimer _timer;
        TimeSpan _time = TimeSpan.Zero;

        public AppEngine Engine { get { return _engine; } }

        public ScheduledSessionModal(Schedule schedule)
        {
            try
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    _engine = AppEngine.Instance;
                }

                //The schedule for which this dialog was triggered
                Schedule = schedule;

                //Start 5 min countdown timer
                StartCountDownTimer();

                InitializeComponent();

                this.DataContext = this;
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region Methods
        private void StartCountDownTimer()
        {
            try
            {
                TitleText = "Your scheduled focus session starts in 5 min.";
                ButtonText = "OK, start in 5 min";

                _time = TimeSpan.FromMinutes(5); //5 min countdown timer

                _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    TitleText = "Your scheduled focus session starts in " + Math.Ceiling(_time.TotalMinutes) + " min.";
                    ButtonText = "OK, start in " + Math.Ceiling(_time.TotalMinutes) + " min";

                    if (_time <= TimeSpan.Zero)
                    {
                        if (_timer != null) _timer.Stop();

                        //TODO - START A FOCUS SESSION HERE
                        LoggingService.WriteAppLog("Scheduled Session Dialog Closed -> 5 min timer timed out");

                        Engine.StartFocusSession(new Session()
                        {
                            ActualStartTime = DateTime.Now,
                            ActualEndTime = Schedule.EndAt,
                            BlockListName = Schedule.BlockListName,

                            //Break
                            ProvideBreak = Engine.UserPreferences.Schedules.Schedulebreak.IsActive,
                            BreakDuration = Engine.UserPreferences.Schedules.Schedulebreak.BreakDuration,
                            BreakGap = Engine.UserPreferences.Schedules.Schedulebreak.BreakGap,

                            //User & Log
                            FocusType = "ScheduledSession",
                            Schedule = Schedule
                        });

                        this.Close();
                    }
                    _time = _time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                _timer.Start();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
        #endregion

        #region Properties
        public event PropertyChangedEventHandler? PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Schedule Schedule { get; set; }

        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                _titleText = value;
                NotifyPropertyChanged(); // method implemented below
            }
        }

        public string ButtonText
        {
            get
            {
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                NotifyPropertyChanged(); // method implemented below
            }
        }
        #endregion

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    this.DragMove();
            //}
        }

        /// <summary>
        /// Hide this window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        private void btnStopFocus_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature will be available soon!");
        }

        private void StartFocus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Default 5 mins
                int minutesLeft = 5;

                Button button = (Button)sender;

                switch(button.Name)
                {
                    case "btnOK":
                        //Get minutes from the timer
                        if (_time == TimeSpan.Zero || Math.Ceiling(_time.TotalMinutes) >= 0)
                            minutesLeft = Convert.ToInt32(Math.Ceiling(_time.TotalMinutes));
                        break;
                    case "btnNow":
                        minutesLeft = 0;
                        break;
                    case "btn10Min":
                        minutesLeft = 10;
                        break;
                    case "btn15Min":
                        minutesLeft = 15;
                        break;
                    default:
                        minutesLeft = 5;
                        break;
                }

                LoggingService.WriteAppLog(String.Format("Start session after {0} min clicked", minutesLeft));

                //Stop the minutes countdown timer
                _timer.Stop(); 
                _time = TimeSpan.Zero;

                //Timer - Start a Session after x mins
                double totalMinutes = (Schedule.EndAt - DateTime.Now).TotalMinutes;
                Engine.StartFocusSession(new Session()
                {
                    ActualStartTime = DateTime.Now,
                    ActualEndTime = Schedule.EndAt,
                    BlockListName = Schedule.BlockListName,

                    //Break
                    ProvideBreak = Engine.UserPreferences.Schedules.Schedulebreak.IsActive,
                    BreakDuration = Engine.UserPreferences.Schedules.Schedulebreak.BreakDuration,
                    BreakGap = Engine.UserPreferences.Schedules.Schedulebreak.BreakGap,

                    //User & Log
                    FocusType = "ScheduledSession",
                    Schedule = Schedule,
                    SessionDuration = Convert.ToInt32(totalMinutes)
                }, minutesLeft);

                this.Close();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
        #endregion


    }
}
