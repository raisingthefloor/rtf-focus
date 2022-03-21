using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for SettingsBlockLists.xaml
    /// </summary>
    public partial class SettingsBlockLists : UserControl
    {
        #region AppEngine and Constructor
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public SettingsBlockLists()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            this.DataContext = this;
        }
        #endregion

        #region User Events
        private void AddBlockList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("AddBlockList_Click");

                //Open the the Add New Blocklist Diaog
                NewBlocklistModal newBlocklist = new NewBlocklistModal();

                //If user has added a blocklist name, add it to json database and make the new item selected
                if (newBlocklist.ShowDialog() == true)
                {
                    if (String.IsNullOrWhiteSpace(newBlocklist.BlockListName)) //Ideally, not needed. Just an additional check
                    {
                        MessageBox.Show("Blocklist Name cannot be an empty string");
                    }
                    else
                    {
                        cmbBlockList.SelectedItem = Engine.UserPreferences.BlockLists.Where(p => p.Name == newBlocklist.BlockListName).First();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnBlockAddApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsBlockLists -> btnBlockAddApp_Click");

                //While a focus session is running with the selected blocklist, do not allow to edit the blocklist
                if (Engine.IsFocusRunning)
                {
                    if (Engine.Session1Blocklist == Engine.SelectedBlockList || Engine.Session2Blocklist == Engine.SelectedBlockList)
                    {
                        InvokeBlocklistErrorDialog();
                        return;
                    }
                }

                AllowUnblockingModal allowUnblockingModal = new AllowUnblockingModal(Engine.SelectedBlockList.AlsoBlock.ActiveAppsAndWebsites, "Select apps to block.");
                allowUnblockingModal.ShowDialog();

            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnBlockAddWebsite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsBlockLists -> btnBlockAddWebsite_Click");

                //While a focus session is running with the selected blocklist, do not allow to edit the blocklist
                if (Engine.IsFocusRunning)
                {
                    if (Engine.Session1Blocklist == Engine.SelectedBlockList || Engine.Session2Blocklist == Engine.SelectedBlockList)
                    {
                        InvokeBlocklistErrorDialog();
                        return;
                    }
                }

                AddWebsiteModal addWebsiteModal = new AddWebsiteModal(Engine.SelectedBlockList.AlsoBlock.ActiveAppsAndWebsites);
                addWebsiteModal.ShowDialog();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnAlsoBlockRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsBlockLists -> btnAlsoBlockRemove_Click");

                //While a focus session is running with the selected blocklist, do not allow to edit the blocklist
                if (Engine.IsFocusRunning)
                {
                    if (Engine.Session1Blocklist == Engine.SelectedBlockList || Engine.Session2Blocklist == Engine.SelectedBlockList)
                    {
                        InvokeBlocklistErrorDialog();
                        return;
                    }
                }

                Button btn = sender as Button;
                var dataObject = btn.DataContext as ActiveAppsAndWebsites;

                Engine.SelectedBlockList.AlsoBlock.ActiveAppsAndWebsites.Remove(dataObject);
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnExceptionsAddApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsBlockLists -> btnExceptionsAddApp_Click");

                //While a focus session is running with the selected blocklist, do not allow to edit the blocklist
                if (Engine.IsFocusRunning)
                {
                    if (Engine.Session1Blocklist == Engine.SelectedBlockList || Engine.Session2Blocklist == Engine.SelectedBlockList)
                    {
                        InvokeBlocklistErrorDialog();
                        return;
                    }
                }

                AllowUnblockingModal allowUnblockingModal = new AllowUnblockingModal(Engine.SelectedBlockList.Exceptions.ActiveAppsAndWebsites, "Select apps to keep unblocked.");
                allowUnblockingModal.ShowDialog();

            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnExceptionsAddWebsite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsBlockLists -> btnExceptionsAddWebsite_Click");

                //While a focus session is running with the selected blocklist, do not allow to edit the blocklist
                if (Engine.IsFocusRunning)
                {
                    if (Engine.Session1Blocklist == Engine.SelectedBlockList || Engine.Session2Blocklist == Engine.SelectedBlockList)
                    {
                        InvokeBlocklistErrorDialog();
                        return;
                    }
                }

                AddWebsiteModal addWebsiteModal = new AddWebsiteModal(Engine.SelectedBlockList.Exceptions.ActiveAppsAndWebsites);
                addWebsiteModal.ShowDialog();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnExceptionsRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsBlockLists -> btnExceptionsRemove_Click");

                //While a focus session is running with the selected blocklist, do not allow to edit the blocklist
                if (Engine.IsFocusRunning)
                {
                    if (Engine.Session1Blocklist == Engine.SelectedBlockList || Engine.Session2Blocklist == Engine.SelectedBlockList)
                    {
                        InvokeBlocklistErrorDialog();
                        return;
                    }
                }

                Button btn = sender as Button;
                var dataObject = btn.DataContext as ActiveAppsAndWebsites;

                Engine.SelectedBlockList.Exceptions.ActiveAppsAndWebsites.Remove(dataObject);
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region Error Dialogs
        private void EditBlocklistError(object sender, ValidationErrorEventArgs e)
        {
            try
            {
                if (e.Action == ValidationErrorEventAction.Added && e.Error.ErrorContent == "Blocklist in use")
                {
                    InvokeBlocklistErrorDialog();

                    ((BindingExpressionBase)e.Error.BindingInError).UpdateTarget();
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void InvokeBlocklistErrorDialog()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ErrorMessageModal errorMessageModal = new ErrorMessageModal()
                    {
                        TitleText = "Changes cannot be made to this Blocklist",
                        ContentText = $"A Blocklist cannot be changed while it is being used during a Focus session.{Environment.NewLine}You must stop this session to make changes."
                    };

                    if (Engine.SelectedBlockList.Penalty == Penalty.Restart)
                        errorMessageModal.ContentSmallText = $"NOTE : You set the blocklist {Engine.SelectedBlockList.Name} to require you{Environment.NewLine}to restart your computer in order to stop the focus session.";

                    if (Engine.SelectedBlockList.Penalty == Penalty.Type)
                        errorMessageModal.ContentSmallText = $"NOTE : You set the blocklist {Engine.SelectedBlockList.Name} to require you{Environment.NewLine}to type random characters in order to stop the focus session.";


                    errorMessageModal.ShowDialog();

                });
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #endregion
    }

    public class EditBlocklistErrorValidationRule : ValidationRule
    {
        #region AppEngine and Constructor
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public EditBlocklistErrorValidationRule()
        {
            _engine = AppEngine.Instance;
        }
        #endregion

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Engine.IsFocusRunning)
            {
                if (Engine.Session1Blocklist == Engine.SelectedBlockList || Engine.Session2Blocklist == Engine.SelectedBlockList)
                    return new ValidationResult(false, "Blocklist in use");
                else
                    return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(true, null);
            };
        }
    }
}
