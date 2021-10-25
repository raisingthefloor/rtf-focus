using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
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
        private List<Penalty> penalties;

        private ObservableCollection<BlockList> blockLists;


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

            penalties = new List<Penalty>();
            penalties.Add(new Penalty { Id = 1, Type = "No, let me stop the focus session at any time", HasValue = false });
            penalties.Add(new Penalty { Id = 2, Type = "Yes, make me type to stop the focus session: ", HasValue = true, Value = 30 });
            penalties.Add(new Penalty { Id = 3, Type = "Yes, make me restart my computer to stop the focus session", HasValue = false });

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
        #endregion

        #region To be deleted

        
        public List<Penalty> Penalties
        {
            get
            {
                return penalties;
            }
        }



        #endregion

        #region User Events
        private void btnBlockAddApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("SettingsBlockLists -> btnBlockAddApp_Click");
                AllowUnblockingModal allowUnblockingModal = new AllowUnblockingModal(Engine.SelectedBlockList.AlsoBlock.ActiveAppsAndWebsites);
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
                AllowUnblockingModal allowUnblockingModal = new AllowUnblockingModal(Engine.SelectedBlockList.Exceptions.ActiveAppsAndWebsites);
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

        #endregion
    }

    

    public class Penalty
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
        public bool HasValue { get; set; }
    }

   
}
