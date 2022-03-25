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

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for FocusBreakEnd.xaml
    /// </summary>
    public partial class EndofBreakModal : Window, INotifyPropertyChanged
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public EndofBreakModal()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            DataContext = this;
        }

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    try
            //    {
            //        this.DragMove();
            //    }
            //    catch (Exception ex)
            //    {
            //        LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            //    }
            //}
        }

        /// <summary>
        /// Hide this window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();
        }
        private void btnStopFocus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Log Closing Session
                LoggingService.WriteAppLog("Session Closing");

                if (Engine.ActiveSessions.Count > 0)
                {
                    Engine.EndSession(Engine.ActiveSessions[0], true);
                }

                //Hide this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void StartFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Engine.StartFocusSequence());

                ShowButtonVisibility();

                //Closes this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
        private void btn1Min_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideButtonVisibility();
                Task.Factory.StartNew(() => Engine.EndBreakRemindInMins(Common.Min1));
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btn5Min_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideButtonVisibility();
                Task.Factory.StartNew(() => Engine.EndBreakRemindInMins(Common.Min5));
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btn15Min_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideButtonVisibility();
                Task.Factory.StartNew(() => Engine.EndBreakRemindInMins(Common.Min15));
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region Properties

        private Visibility _show1min = Visibility.Visible;
        private Visibility _show5min = Visibility.Visible;
        private Visibility _show15min = Visibility.Visible;

        public Visibility Show1min
        {
            get => _show1min;
            set
            {
                if (_show1min != value)
                {
                    _show1min = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Visibility Show5min
        {
            get => _show5min;
            set
            {
                if (_show5min != value)
                {
                    _show5min = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Visibility Show15min
        {
            get => _show15min;
            set
            {
                if (_show15min != value)
                {
                    _show15min = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void HideButtonVisibility()
        {
            if (Show15min == Visibility.Visible)
            {
                Show15min = Visibility.Collapsed;
                return;
            }
            else if (Show5min == Visibility.Visible)
            {
                Show5min = Visibility.Collapsed;
                return;
            }
            else if (Show1min == Visibility.Visible)
            {
                Show1min = Visibility.Collapsed;
                return;
            }
        }

        private void ShowButtonVisibility()
        {
            Show1min = Show5min = Show15min = Visibility.Visible;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true; //Do not allow the window to close
        }
    }
}
