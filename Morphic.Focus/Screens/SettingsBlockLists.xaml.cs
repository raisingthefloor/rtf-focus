using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }

    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
