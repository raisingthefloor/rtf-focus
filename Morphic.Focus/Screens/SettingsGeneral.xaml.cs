using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for SettingsGeneral.xaml
    /// </summary>
    public partial class SettingsGeneral : UserControl
    {
        #region AppEngine and Constructor
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public SettingsGeneral()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            this.DataContext = this;
        }
        #endregion

        private void btnAddAppUnblock_Click(object sender, RoutedEventArgs e)
        {
            LoggingService.WriteAppLog("btnAddAppUnblock_Click");
            AllowUnblockingModal unblockAddAppWebsite = new AllowUnblockingModal(Engine.UserPreferences.General.TemporarilyUnblock.ActiveAppsAndWebsites, "Select apps to allow yourself to temporarily unblock.");
            unblockAddAppWebsite.ShowDialog();
        }

        private void btnAddWebsiteUnblock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsGeneral -> btnAddWebsiteUnblock_Click");
                AddWebsiteModal addWebsiteModal = new AddWebsiteModal(Engine.UserPreferences.General.TemporarilyUnblock.ActiveAppsAndWebsites);
                addWebsiteModal.ShowDialog();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnRemoveTempUnblock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsGeneral -> btnRemoveTempUnblock_Click");
                Button btn = sender as Button;
                var dataObject = btn.DataContext as ActiveAppsAndWebsites;

                Engine.UserPreferences.General.TemporarilyUnblock.ActiveAppsAndWebsites.Remove(dataObject);
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
    }
}

