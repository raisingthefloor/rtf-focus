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
    /// Interaction logic for ShortBreakModal.xaml
    /// </summary>
    public partial class ShortBreakModal : Window, INotifyPropertyChanged
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }



        public ShortBreakModal()
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
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

                if (Engine.Session1 != null)
                    Engine.StopFocusSession(Engine.Session1);

                //Hide this dialog
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
                Task.Factory.StartNew(() => Engine.ShortBreakRemindInMins(1)); //TODO set 5 instead of 1
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btn10Min_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideButtonVisibility();
                Task.Factory.StartNew(() => Engine.ShortBreakRemindInMins(10));
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
                Task.Factory.StartNew(() => Engine.ShortBreakRemindInMins(15));
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }



        private void StartBreak(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Engine.StartBreakSequence());

                ShowButtonVisibility();

                //Closes this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
        #endregion

        #region Properties

        private Visibility _show5min = Visibility.Visible;
        private Visibility _show10min = Visibility.Visible;
        private Visibility _show15min = Visibility.Visible;

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
        public Visibility Show10min
        {
            get => _show10min;
            set
            {
                if (_show10min != value)
                {
                    _show10min = value;
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
            else if (Show10min == Visibility.Visible)
            {
                Show10min = Visibility.Collapsed;
                return;
            }
            else if (Show5min == Visibility.Visible)
            {
                Show5min = Visibility.Collapsed;
                return;
            }

        }

        private void ShowButtonVisibility()
        {
            Show5min = Show10min = Show15min = Visibility.Visible;
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
