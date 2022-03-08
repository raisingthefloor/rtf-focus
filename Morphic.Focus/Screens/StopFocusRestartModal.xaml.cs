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
    /// Interaction logic for StopFocusRestartModal.xaml
    /// </summary>
    public partial class StopFocusRestartModal : Window, INotifyPropertyChanged
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public StopFocusRestartModal()
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
                if (e.OriginalSource is Hyperlink)
                {
                    try
                    {
                        LoggingService.WriteAppLog("Stop Focus Restart -> Show me the blocklist");

                        //Set Edit Blocklists Tab open
                        Engine.Settings.OpenBlocklist = true;
                        Engine.SelectedBlockList = ApplicableBlocklist;

                        //Hide current window and show Settings Window
                        Engine.Settings.Show();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                    }
                }
                else
                {
                    //try
                    //{
                    //    this.DragMove();
                    //}
                    //catch (Exception ex)
                    //{
                    //    LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                    //}
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
            this.Close();
        }
        #endregion

        private Blocklist? _applicableBlocklist = null;
        public Blocklist ApplicableBlocklist
        {
            get
            {
                return _applicableBlocklist;
            }
            set
            {
                if (_applicableBlocklist != value)
                {
                    _applicableBlocklist = value;
                }
            }
        }

        private Session _session;
        public Session Session 
        { 
            get
            {
                return _session;
            }
            set
            {
                if (value != _session)
                {
                    _session = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void RestartComputer(object sender, RoutedEventArgs e)
        {
            //Opens up Win Menu
            KeyboardSend.OpenPowerBar();
            Session.IsMarkedForClose = true;
            this.Close();
        }

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
