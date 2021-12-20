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

        //public Session? Session1 { get => _engine.Session1; set => _engine.Session1 = value; }
        //public Session? Session2 { get => _engine.Session2; set => _engine.Session2 = value; }

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

        private void Engine_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();

            //if (e.PropertyName == "IsFocusRunning" || e.PropertyName == "Session1" || e.PropertyName == "Session2")
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
                    ButtonText = "Focus" + Environment.NewLine +  Engine.TimeTillNextBreakHHMM; //Math.Ceiling(Engine.TimeTillNextBreak.TotalMinutes);
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
