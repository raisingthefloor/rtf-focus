
using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for FocusStatus.xaml
    /// </summary>
    public partial class ActiveSessionNonModal : Window, INotifyPropertyChanged
    {
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

                Engine.PropertyChanged += Engine_PropertyChanged;
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
                if (e.OriginalSource is Hyperlink)
                {
                    try
                    {
                        LoggingService.WriteAppLog("Active Session -> Why do this?");

                        Hyperlink link = (Hyperlink)e.OriginalSource;

                        if (((Hyperlink)e.OriginalSource).NavigateUri != null)
                            Process.Start("explorer", ((Hyperlink)e.OriginalSource).NavigateUri.ToString());
                    }
                    catch (Exception ex)
                    {
                        LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                    }
                }
                else
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

                Engine.EndSession(Engine.Session1, true);
                
                //Hide this dialog
                this.Hide();
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
                if (Engine.IsFocusWithBreaks)
                    BreakStatusText = string.Format("Your next break is in {0} minutes", Engine.TimeTillNextBreakMM);
                else
                    BreakStatusText = String.Empty;

            if (e.PropertyName == "TimeTillNextBreakEnds")
                BreakStatusText = string.Format("Your break ends in {0} minutes", Engine.TimeTillNextBreakEndsMM);
        }
        private void btnStopFocus2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Log Closing Session
                LoggingService.WriteAppLog("Session Closing");

                Engine.EndSession(Engine.Session2, true);

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

        #endregion

        #region Properties
        private string _breakStatusText = string.Empty;
        public string BreakStatusText
        {
            get
            {
                return _breakStatusText;
            }
            set
            {
                if (_breakStatusText != value)
                {
                    _breakStatusText = value;
                    NotifyPropertyChanged();
                }
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
