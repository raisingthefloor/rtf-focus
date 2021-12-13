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
using System.Runtime.CompilerServices;

namespace Morphic.Focus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Members and Constructor

        private string _buttonText = "Focus";
        DispatcherTimer _timer;
        TimeSpan _time;
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public MainWindow()
        {
            try
            {
                //App Engine
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    _engine = AppEngine.Instance;
                }

                InitializeComponent();

                DataContext = this;

                //TODO - Review
                //Engine.FocusMain.SessionUpdate += FocusMain_SessionUpdate;
                //Engine.FocusStatus.SessionUpdate += FocusMain_SessionUpdate;

                Engine.PropertyChanged += Engine_PropertyChanged;
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }


        //private PropertyChangedEventHandler _engine_PropertyChanged(object sender, object e)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Properties

        //public Session? CurrSession1 { get => _engine.CurrSession1; set => _engine.CurrSession1 = value; }
        //public Session? CurrSession2 { get => _engine.CurrSession2; set => _engine.CurrSession2 = value; }

        //public bool IsFocusRunning
        //{
        //    get
        //    {
        //        return _engine.IsFocusRunning;
        //    }
        //    set
        //    {
        //        _engine.IsFocusRunning = value;
        //        NotifyPropertyChanged("IsFocusRunning"); // method implemented below
        //    }
        //}

        public string ButtonText
        {
            get
            {
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch (Exception ex)
                {
                    LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                }
            }
        }

        private void btnMainFocus_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
                Engine.ShowFocusWindow();
            }
            catch (Exception ex)
            {

                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnMainFocus_Click(object sender, RoutedEventArgs e)
        {
            //If Focus Session is running then show Active Sessions Window, else show Main Menu Window
            Engine.ShowFocusWindow();

            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void FocusMain_SessionUpdate()
        {
            ////Reset Button Text / Countdown Timer
            //_time = TimeSpan.Zero;
            //if (_timer != null) _timer.Stop();

            //if (Engine.IsFocusRunning)
            //{
            //    double nextBreakGap = -1;
            //    //Set nextBreakGap
            //    if (Engine.CurrSession1 != null) 
            //    {
            //        if (Engine.CurrSession1.SessionDuration == 0) 
            //        {
            //            nextBreakGap = 0;
            //        }
            //        else
            //        {
            //            nextBreakGap = Engine.CurrSession1.BreakGap;
            //        }
            //    }

            //    if (Engine.CurrSession2 != null)
            //    {
            //        if (Engine.CurrSession2.SessionDuration == 0)
            //        {
            //            if (nextBreakGap < 0)
            //                nextBreakGap = 0;
            //        }
            //        else
            //        {
            //            if (nextBreakGap <= 0 || Engine.CurrSession1.BreakGap < nextBreakGap) 
            //                nextBreakGap = Engine.CurrSession1.BreakGap;
            //        }
            //    }

            //    if (nextBreakGap == -1)
            //        ButtonText = "Focus";
            //    else if (nextBreakGap == 0)
            //        ButtonText = "Focus till Stop";
            //    else
            //    {
            //        ButtonText = "Focus" + Environment.NewLine + nextBreakGap;

            //        _time = TimeSpan.FromMinutes(nextBreakGap);

            //        _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            //        {
            //            ButtonText = "Focus" + Environment.NewLine + Math.Ceiling(_time.TotalMinutes);

            //            if (_time <= TimeSpan.Zero)
            //            {
            //                if (_timer != null) _timer.Stop();

            //                new ShortBreakModal().ShowDialog();
            //            }
            //            _time = _time.Add(TimeSpan.FromSeconds(-1));
            //        }, Application.Current.Dispatcher);

            //        _timer.Start();
            //    }
            //}
            //else
            //{
            //    ButtonText = "Focus";
            //}
        }

        private void Engine_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();

            //if (e.PropertyName == "IsFocusRunning" || e.PropertyName == "CurrSession1" || e.PropertyName == "CurrSession2")
            //    FocusMain_SessionUpdate();

            if (e.PropertyName == "TimeTillNextBreak")
                UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            if (Engine.IsFocusRunning)
            {
                if (Engine.IsFocusTillStop)
                {
                    ButtonText = "Focus till Stop";
                }
                else
                {
                    ButtonText = "Focus" + Environment.NewLine +  new TimeSpan(0, (int)Math.Ceiling(Engine.TimeTillNextBreak.TotalMinutes), 0).ToString("hh':'mm"); //Math.Ceiling(Engine.TimeTillNextBreak.TotalMinutes);
                }
            }
            else
            {
                ButtonText = "Focus";
            }
        }
        #endregion

        #region INotifyPropertyChanged implement
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
        #endregion
    }
}
