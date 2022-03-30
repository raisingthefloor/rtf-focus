using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public delegate void EventHandler(Session? Session1);

    /// <summary>
    /// Interaction logic for FocusMain.xaml
    /// </summary>
    public partial class MainMenuNonModal : Window, INotifyPropertyChanged
    {
        #region Members and Constructor 
        
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public MainMenuNonModal()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            DataContext = this;

            //Engine.PropertyChanged += _engine_PropertyChanged;
        }
        #endregion

        #region Events
        /// <summary>
        /// Allow user to drag the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Navigate to Settings Screen with Edit Blocklists Menu Item Open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewEditBlockList_Click(object sender, RoutedEventArgs e)
        {
            //Set Edit Blocklists Tab open
            Engine.Settings.OpenBlocklist = true;

            //Hide current window and show Settings Window
            this.Hide();
            Engine.Settings.Show();
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
            //Donot set Edit Blocklists Tab open
            Engine.Settings.OpenBlocklist = false;

            //Hide current window and show Settings Window
            this.Hide();
            Engine.Settings.Show();
        }

        private void FocusStart(object sender, RoutedEventArgs e)
        {
            try
            {
                var blocklistName = chkBlockProgram.IsChecked ?? false ?
                        (cmbBlockList.SelectedValue == null ? "" : cmbBlockList.SelectedValue.ToString()) :
                        "";

                var turnOnDnd = chkDND.IsChecked ?? false;
                if (Engine.BlocklistnameIncludesNotificationCategory(blocklistName) == true)
                {
                    turnOnDnd = true;
                }

                Engine.StartFocusSession(new Session()
                {
                    ProvideBreak = chkProvide.IsChecked ?? false,
                    BreakDuration = int.Parse(((ComboBoxItem)cmbBreakTIme.SelectedItem).Tag.ToString()),
                    BreakGap = int.Parse(((ComboBoxItem)cmbEvery.SelectedItem).Tag.ToString()),

                    BlockListName = blocklistName,

                    ActualStartTime = DateTime.Now,
                    
                    SessionDuration = int.Parse(((Button)sender).Tag.ToString()),

                    TurnONDND = turnOnDnd
                });

                //Hide this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }

        }

        //private void _engine_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    NotifyPropertyChanged();
        //}
        #endregion

        #region Info Text Visibility
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility();
        }

        private void FocusChange(object sender, RoutedEventArgs e)
        {
            var control = (dynamic)sender;

            switch (control.Name.ToString())
            {
                case "chkDND":
                    //Show DND Info if either i button is clicked or focus is on checkbos
                    if (chkDND.IsFocused || (btnInfo.IsChecked ?? false))
                        txtInfo1.Visibility = Visibility.Visible;
                    else
                        txtInfo1.Visibility = Visibility.Hidden;
                    break;

                case "sPnlBreakSetting":
                case "chkProvide":
                case "cmbBreakTIme":
                case "cmbEvery":
                    //Show DND Info if either i button is clicked or focus is on checkbos
                    if (sPnlBreakSetting.IsKeyboardFocusWithin || (btnInfo.IsChecked ?? false))
                        txtInfo2.Visibility = Visibility.Visible;
                    else
                        txtInfo2.Visibility = Visibility.Hidden;
                    break;

                case "sPnlStartSession":
                case "btnSessionTillStop":
                case "btn30Min":
                case "btn1Hr":
                case "btn2Hrs":
                    //Show DND Info if either i button is clicked or focus is on checkbos
                    if (sPnlStartSession.IsKeyboardFocusWithin || btnSessionTillStop.IsFocused || (btnInfo.IsChecked ?? false))
                        txtInfo3.Visibility = Visibility.Visible;
                    else
                        txtInfo3.Visibility = Visibility.Hidden;
                    break;
                case "btnSettings":
                    //Show DND Info if either i button is clicked or focus is on checkbos
                    if (btnSettings.IsFocused || (btnInfo.IsChecked ?? false))
                        txtInfo4.Visibility = Visibility.Visible;
                    else
                        txtInfo4.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void SetVisibility()
        {
            bool showInfo = btnInfo.IsChecked ?? false;

            txtInfo1.Visibility = txtInfo2.Visibility = txtInfo3.Visibility = txtInfo4.Visibility = showInfo ? Visibility.Visible : Visibility.Hidden;
        }

        private void MouseOver(object sender, MouseEventArgs e)
        {
            var control = (dynamic)sender;

            switch (control.Name.ToString())
            {
                case "chkDND":
                    //Show DND Info if either i button is clicked or focus is on checkbos
                    if (chkDND.IsMouseOver || (btnInfo.IsChecked ?? false))
                        txtInfo1.Visibility = Visibility.Visible;
                    else
                        txtInfo1.Visibility = Visibility.Hidden;
                    break;

                case "sPnlBreakSetting":
                    //Show DND Info if either i button is clicked or focus is on checkbos
                    if (sPnlBreakSetting.IsMouseOver || (btnInfo.IsChecked ?? false))
                        txtInfo2.Visibility = Visibility.Visible;
                    else
                        txtInfo2.Visibility = Visibility.Hidden;
                    break;

                case "sPnlStartSession":
                case "btnSessionTillStop":
                    //Show DND Info if either i button is clicked or focus is on checkbos
                    if (sPnlStartSession.IsMouseOver || btnSessionTillStop.IsMouseOver || (btnInfo.IsChecked ?? false))
                        txtInfo3.Visibility = Visibility.Visible;
                    else
                        txtInfo3.Visibility = Visibility.Hidden;
                    break;
                case "btnSettings":
                    //Show DND Info if either i button is clicked or focus/hover is on checkbos
                    if (btnSettings.IsMouseOver || (btnInfo.IsChecked ?? false))
                        txtInfo4.Visibility = Visibility.Visible;
                    else
                        txtInfo4.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void chkBlockProgram_Click(object sender, RoutedEventArgs e)
        {
            bool showBlocklist = chkBlockProgram.IsChecked ?? false;

            sPnlBlockList.Visibility = showBlocklist ? Visibility.Visible : Visibility.Hidden;
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
            this.Hide();
        }
    }
}
