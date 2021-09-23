using Morphic.Focus.Models;
using Morphic.Focus.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class SettingsBlockLists : UserControl, INotifyPropertyChanged
    {
        private List<BlockCategory> blockCategories;
        private List<BlockItem> blockItems;
        private List<BlockItem> blockExceptions;
        private List<Penalty> penalties;

        private ObservableCollection<BlockList> blockLists;
        private BlockList BlockList;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsBlockLists()
        {
            InitializeComponent();

            blockCategories = new List<BlockCategory>();
            blockCategories.Add(new BlockCategory { Id = 1, Name = "Calls" });
            blockCategories.Add(new BlockCategory { Id = 2, Name = "Notifications" });
            blockCategories.Add(new BlockCategory { Id = 3, Name = "Social Media" });
            blockCategories.Add(new BlockCategory { Id = 4, Name = "Games" });
            blockCategories.Add(new BlockCategory { Id = 5, Name = "News" });
            blockCategories.Add(new BlockCategory { Id = 6, Name = "Shopping" });

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

            GetBlockLists();

            this.DataContext = this;
        }

        private void AddBlockList_Click(object sender, RoutedEventArgs e)
        {
            NewBlocklist newBlocklist = new NewBlocklist();

            if (newBlocklist.ShowDialog() == true)
            {
                if (String.IsNullOrWhiteSpace(newBlocklist.BlockListName))
                {
                    MessageBox.Show("Blocklist Name cannot be an empty string");
                }
                else
                {
                    IDataService<BlockList> dataService = new GenericDataService<BlockList>(new FocusDbContextFactory());
                    dataService.Create(new BlockList() { Name = newBlocklist.BlockListName });
                    BlockLists = new ObservableCollection<BlockList>(dataService.GetAll().Result);
                    if (BlockLists.Count > 0) BlockList = BlockLists[0];
                }
            }
                
        }

        #region Data

        private void GetBlockLists()
        {
            IDataService<BlockList> dataService = new GenericDataService<BlockList>(new FocusDbContextFactory());
            BlockLists = new ObservableCollection<BlockList>(dataService.GetAll().Result);

            if (BlockLists.Count > 0) BlockList = BlockLists[0];
        }

        public ObservableCollection<BlockList> BlockLists
        {
            get
            {
                return blockLists;
            }
            set
            {
                blockLists = value;
                NotifyPropertyChanged("BlockLists"); // method implemented below
            }
        }
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public List<BlockCategory> BlockCategories
        {
            get
            {
                return blockCategories;
            }
        }

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
            BlockAddApp blockAddApp = new BlockAddApp();

            if (blockAddApp.ShowDialog() == true)
            {
                
            }
        }

        private void btnBlockAddWebsite_Click(object sender, RoutedEventArgs e)
        {
            BlockAddWebsite blockAddWebsite = new BlockAddWebsite();

            if (blockAddWebsite.ShowDialog() == true)
            {

            }
        }
    }

    public class BlockCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class Penalty
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
        public bool HasValue { get; set; }
    }
}
