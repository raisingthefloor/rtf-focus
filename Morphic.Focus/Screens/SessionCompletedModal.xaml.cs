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

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for SessionCompletedModal.xaml
    /// </summary>
    public partial class SessionCompletedModal : Window, INotifyPropertyChanged
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        private Session session;
        private bool _hasUserInteracted = false;

        public Session Session
        {
            get => session;
            set => session = value;
        }

        public SessionCompletedModal(Session session)
        {
            try
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    _engine = AppEngine.Instance;
                }

                Session = session;

                InitializeComponent();

                DataContext = this;
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #region Events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_hasUserInteracted)
                e.Cancel = true; //Do not allow the window to close
        }

        private void EndSession(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Engine.EndSession(Session));

                ShowButtonVisibility();

                //Closes this dialog
                HideWindow();
                this.Close();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

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

        #endregion

        private void btn5Min_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideButtonVisibility();
                Task.Factory.StartNew(() => Engine.EndSessionRemindInMins(Common.Min5));
                HideWindow();
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
                Task.Factory.StartNew(() => Engine.EndSessionRemindInMins(Common.Min10));
                HideWindow();
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
                Task.Factory.StartNew(() => Engine.EndSessionRemindInMins(Common.Min15));
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void HideWindow()
        {
            _hasUserInteracted = true;
            this.Hide();
        }

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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
                _hasUserInteracted = false;
        }
    }
}
