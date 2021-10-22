using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        private List<BlockItem> blockItems;
        private List<BlockItem> blockExceptions;
        private List<Penalty> penalties;

        private ObservableCollection<BlockList> blockLists;
        private BlockList BlockList;


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

            blockItems = new List<BlockItem>();
            blockItems.Add(new BlockItem { Id = 1, Name = "Acron Editor" });
            blockItems.Add(new BlockItem { Id = 2, Name = "doppler.com" });
            blockItems.Add(new BlockItem { Id = 3, Name = "Brian Lara's Cricket" });

            blockExceptions = new List<BlockItem>();
            blockExceptions.Add(new BlockItem { Id = 1, Name = "Microsoft Word" });
            blockExceptions.Add(new BlockItem { Id = 2, Name = "wikipedia.com" });

            penalties = new List<Penalty>();
            penalties.Add(new Penalty { Id = 1, Type = "No, let me stop the focus session at any time", HasValue = false });
            penalties.Add(new Penalty { Id = 2, Type = "Yes, make me type to stop the focus session: ", HasValue = true, Value = 30 });
            penalties.Add(new Penalty { Id = 3, Type = "Yes, make me restart my computer to stop the focus session", HasValue = false });

            this.DataContext = this;
        }
        #endregion

        private void AddBlockList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("AddBlockList_Click");

                NewBlocklistModal newBlocklist = new NewBlocklistModal();

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

        #region To be deleted

        public List<BlockItem> BlockExceptions
        {
            get
            {
                return blockExceptions;
            }
        }

        public List<Penalty> Penalties
        {
            get
            {
                return penalties;
            }
        }

        public List<BlockItem> BlockItems
        {
            get
            {
                return blockItems;
            }
        }

        #endregion
        private void btnBlockAddApp_Click(object sender, RoutedEventArgs e)
        {
            AddAppModal blockAddApp = new AddAppModal();

            if (blockAddApp.ShowDialog() == true)
            {
                
            }
        }

        private void btnBlockAddWebsite_Click(object sender, RoutedEventArgs e)
        {
            AddWebsiteModal blockAddWebsite = new AddWebsiteModal();

            if (blockAddWebsite.ShowDialog() == true)
            {

            }
        }
    }

    public class Penalty
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
        public bool HasValue { get; set; }
    }

    #region To be deleted
    public class BlockItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    #endregion
}
