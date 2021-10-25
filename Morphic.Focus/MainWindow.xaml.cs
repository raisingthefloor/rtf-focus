using Morphic.Data.Models;
using Morphic.Focus.Screens;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;

namespace Morphic.Focus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _buttonText = "Focus";

        DispatcherTimer _timer;
        TimeSpan _time;

        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public MainWindow()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            DataContext = this;

            _engine.FocusMain.SessionUpdate += FocusMain_SessionUpdate;
            _engine.FocusStatus.SessionUpdate += FocusMain_SessionUpdate;
            _engine.PropertyChanged += _engine_PropertyChanged;
        }

        
        private void _engine_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("IsFocusRunning");
            //FocusMain_SessionUpdate(CurrSession1);
        }

        #region Properties

        public Session? CurrSession1 { get => _engine.CurrSession1; set => _engine.CurrSession1 = value; }

        public bool IsFocusRunning
        {
            get
            {
                return _engine.IsFocusRunning;
            }
            set
            {
                _engine.IsFocusRunning = value;
                NotifyPropertyChanged("IsFocusRunning"); // method implemented below
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
                NotifyPropertyChanged("ButtonText"); // method implemented below
            }
        }

        #endregion

        #region Events
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnMainFocus_Click(object sender, RoutedEventArgs e)
        {
            _engine.ShowFocusWindow();
        }

        private void FocusMain_SessionUpdate(Session? currSession1)
        {

            CurrSession1 = currSession1;
            IsFocusRunning = CurrSession1 != null;

            _time = TimeSpan.Zero;
            if (_timer != null) _timer.Stop();

            if (IsFocusRunning)
            {
                if (CurrSession1.SessionDuration == 0)
                    ButtonText = "Focus till Stop";
                else
                {
                    ButtonText = "Focus" + Environment.NewLine + CurrSession1.BreakGap;

                    _time = TimeSpan.FromMinutes(CurrSession1.BreakGap);

                    _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                    {
                        ButtonText = "Focus" + Environment.NewLine + Math.Ceiling(_time.TotalMinutes);

                        if (_time == TimeSpan.Zero)
                        {
                            if (_timer != null) _timer.Stop();

                            new ShortBreakModal().ShowDialog();
                        }
                        _time = _time.Add(TimeSpan.FromSeconds(-1));
                    }, Application.Current.Dispatcher);

                    _timer.Start();
                }
            }
            else
            {
                ButtonText = "Focus";
            }
        }
        #endregion

        #region INotifyPropertyChanged implement
        //Property changed
        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region To be deleted
        public async void video()
        {
            //Scr 1
            ActiveSessionNonModal scr1 = new ActiveSessionNonModal();
            scr1.Show();

            await Task.Delay(5000);
            scr1.Close();

            //Src 2
            AddAppModal scr2 = new AddAppModal();
            scr2.Show();

            await Task.Delay(5000);
            scr2.Close();

            //Src 3
            AddWebsiteModal scr3 = new AddWebsiteModal();
            scr3.Show();

            await Task.Delay(5000);
            scr3.Close();

            //Src 4
            AllowUnblockingModal scr4 = new AllowUnblockingModal();
            scr4.Show();

            await Task.Delay(5000);
            scr4.Close();

            //Src 5
            BlockedAppModal scr5 = new BlockedAppModal();
            scr5.Show();

            await Task.Delay(5000);
            scr5.Close();

            //Src 6
            //CategoryListModal scr6 = new CategoryListModal();
            //scr6.Show();

            //await Task.Delay(5000);
            //scr6.Close();

            //Src 7
            EndofBreakModal scr7 = new EndofBreakModal();
            scr7.Show();

            await Task.Delay(5000);
            scr7.Close();

            //Src 8
            ErrorMessageModal scr8 = new ErrorMessageModal();
            scr8.Show();

            await Task.Delay(5000);
            scr8.Close();

            //Src 9
            LockedScreenNonModal scr9 = new LockedScreenNonModal();
            scr9.Show();

            await Task.Delay(5000);
            scr9.Close();

            //Src 10
            LongBreakModal scr10 = new LongBreakModal();
            scr10.Show();

            await Task.Delay(5000);
            scr10.Close();

            //Src 11
            MainMenuNonModal scr11 = new MainMenuNonModal();
            scr11.Show();

            await Task.Delay(5000);
            scr11.Close();

            //Src 12
            NewBlocklistModal scr12 = new NewBlocklistModal();
            scr12.Show();

            await Task.Delay(5000);
            scr12.Close();

            //Src 13
            ScheduledReminderModal scr13 = new ScheduledReminderModal();
            scr13.Show();

            await Task.Delay(5000);
            scr13.Close();

            //Src 14
            ScheduledSessionModal scr14 = new ScheduledSessionModal();
            scr14.Show();

            await Task.Delay(5000);
            scr14.Close();

            //Src 15
            SessionCompletedModal scr15 = new SessionCompletedModal();
            scr15.Show();

            await Task.Delay(5000);
            scr15.Close();

            //Src 16
            Settings scr16 = new Settings();
            scr16.Show();

            await Task.Delay(5000);
            scr16.Close();

            //Src 17
            ShortBreakModal scr17 = new ShortBreakModal();
            scr17.Show();

            await Task.Delay(5000);
            scr17.Close();

            //Src 18
            StopFocusRandomCharModal scr18 = new StopFocusRandomCharModal();
            scr18.Show();

            await Task.Delay(5000);
            scr18.Close();

            //Src 19
            StopFocusRestartModal scr19 = new StopFocusRestartModal();
            scr19.Show();

            await Task.Delay(5000);
            scr19.Close();
        }
        #endregion

       

        
    }
}
