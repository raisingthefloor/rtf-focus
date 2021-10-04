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

            _engine.CheckIsFocusRunning();
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
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
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
    }
}
