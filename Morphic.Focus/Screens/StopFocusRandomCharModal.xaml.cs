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
    /// Interaction logic for StopFocusRandomCharModal.xaml
    /// </summary>
    public partial class StopFocusRandomCharModal : Window, INotifyPropertyChanged
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public StopFocusRandomCharModal()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            DataContext = this;
        }

        #region Properties
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
                    NotifyPropertyChanged();
                }
            }
        }

        private int _penaltyValue = 0;
        public int PenaltyValue
        {
            get
            {
                return _penaltyValue;
            }
            set
            {
                if (_penaltyValue != value)
                {
                    _penaltyValue = value;
                    NotifyPropertyChanged("RandomChars");
                }
            }
        }

        private string _randomChars = string.Empty;
        public string RandomChars
        {
            get
            {
                // constrain penalty value range
                var penaltyValue = Blocklist.ConstrainPenaltyValue(this.PenaltyValue);

                if (string.IsNullOrWhiteSpace(_randomChars) || _randomChars.Length != penaltyValue)
                {
                    _randomChars = RandomString(penaltyValue);
                }

                return _randomChars;
            }
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Session Session  { get; set; }
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

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.OriginalSource is Hyperlink)
                {
                    try
                    {
                        LoggingService.WriteAppLog("Stop Focus Random Char -> Show me the blocklist");

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

        private void StopSession(object sender, RoutedEventArgs e)
        {
            try
            {
                Engine.EndSession(Session);
                this.Close();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void EnableDisableStopFocusButton(object sender, TextChangedEventArgs e)
        {
            btnStopSession.IsEnabled = txtRandomChars.Text == RandomChars;
        }
        #endregion


    }
}
