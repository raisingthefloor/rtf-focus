using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private SettingsGeneral _objSettingsGeneral;
        private SettingsBlockLists _objSettingsBlockLists;
        private SettingsSchedule _objSettingsSchedule;
        private SettingsTodaysSchedule _objSettingsTodaysSchedule;

        private Control _currentSelectedSetting;
        private bool openBlocklist = false;

        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public Settings()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            //Initialize right-side User Controls
            _objSettingsGeneral = new SettingsGeneral();
            _objSettingsBlockLists = new SettingsBlockLists();
            _objSettingsSchedule = new SettingsSchedule();
            _objSettingsTodaysSchedule = new SettingsTodaysSchedule();

            //Set the General Setting as the default View of the Setting when opened up
            _currentSelectedSetting = _objSettingsGeneral;
            sPnlContent.Children.Add(_currentSelectedSetting);
            lstBoxMenu.SelectedItem = lstItemGeneral;

            this.DataContext = this;
        }

        public bool OpenBlocklist
        {
            get => openBlocklist;
            set
            {
                if (value)
                    lstBoxMenu.SelectedIndex = 1;
                else
                    lstBoxMenu.SelectedIndex = 0;
                openBlocklist = value;
            }
        }

        #region Events
        /// <summary>
        /// Let the Window be dragged using mouse-press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (e.OriginalSource is Hyperlink)
                    {
                        try
                        {
                            LoggingService.WriteAppLog("Settings -> Window_MouseMove");

                            Hyperlink link = (Hyperlink)e.OriginalSource;

                            if (link.Tag != null && link.Tag.ToString() == "Category")
                            {
                                LoggingService.WriteAppLog("Settings -> Category_Clicked");

                                var run = ((Hyperlink)e.OriginalSource).Inlines.FirstOrDefault() as Run;
                                string text = run == null ? string.Empty : run.Text;

                                CategoryListModal categoryListModal = new CategoryListModal(text);
                                categoryListModal.ShowDialog();
                                return;
                            }

                            if (((Hyperlink)e.OriginalSource).NavigateUri != null)
                                Process.Start("explorer", ((Hyperlink)e.OriginalSource).NavigateUri.ToString());
                        }
                        catch (Exception ex)
                        {
                            LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                        }
                    }
                    else if (e.OriginalSource is TextBox)
                    {
                        return;
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
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// Event used to update the usercontrol on the right of the side of the settings screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Remove all current content
            sPnlContent.Children.Clear();

            //Get the currently selected menu item and accordingly set current user control
            switch (((ListBoxItem)lstBoxMenu.SelectedItem).Name)
            {
                case "lstItemGeneral":
                    _currentSelectedSetting = _objSettingsGeneral;
                    break;
                case "lstItemBlockLists":
                    _currentSelectedSetting = _objSettingsBlockLists;
                    break;
                case "lstItemSchedule":
                    _currentSelectedSetting = _objSettingsSchedule;
                    break;
                case "lstItemToday":
                    _currentSelectedSetting = _objSettingsTodaysSchedule;
                    break;
            }

            //Add the _currentSelectedSetting as the child of the right menu stack panel
            sPnlContent.Children.Add(_currentSelectedSetting);
        }

        /// <summary>
        /// Close this window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();
            _engine.ShowFocusWindow();
        }
        #endregion


    }
}
