
using Morphic.Data.Models;
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
using System.Windows.Shapes;

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for FocusStatus.xaml
    /// </summary>
    public partial class ActiveSessionNonModal : Window, INotifyPropertyChanged
    {
        public event EventHandler SessionUpdate;

        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public ActiveSessionNonModal()
        {
            try
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    _engine = AppEngine.Instance;
                }

                InitializeComponent();

                DataContext = this;

                
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
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

        /// <summary>
        /// Close current window and open Settings Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            _engine.Settings.Show();
        }

        /// <summary>
        /// Stop the ongoing Focus Session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopFocus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Log Closing Session
                LoggingService.WriteAppLog("Session Closing");

                Engine.StopFocusSession(Engine.Session1);
                
                //Hide this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
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

        private void btnStopFocus2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Log Closing Session
                LoggingService.WriteAppLog("Session Closing");

                Engine.StopFocusSession(Engine.Session2);

                //Hide this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void StartSecondSession(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide();
                _engine.FocusMain.Show();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
    }
}
