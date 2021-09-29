using Morphic.Focus.JSONService;
using Morphic.Focus.Models;
using Morphic.Focus.Services;
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
    public partial class FocusStatus : Window, INotifyPropertyChanged
    {
        public event EventHandler SessionUpdate;

        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

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

                if (SessionUpdate != null)
                {
                    SessionUpdate(CurrSession1);
                }
            }
        }

        public FocusStatus()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();
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
                //Delete file and stop focus session
                JSONHelper jSONHelper = new JSONHelper(Common.SESSION_FILE_NAME);
                string jsonString = jSONHelper.GetJson<Session>();

                //Log Closing Session
                LoggingService.WriteToLog("Session Closing : " + jsonString);

                File.Delete(Common.MakeFilePath(Common.SESSION_FILE_NAME));

                CurrSession1 = null;
                IsFocusRunning = false;

                //Hide this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteToLog(ex.Message + ex.StackTrace);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature will be available soon!");
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
