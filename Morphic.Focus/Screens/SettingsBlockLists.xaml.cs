using System;
using System.Collections.Generic;
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
    /// Interaction logic for SettingsBlockLists.xaml
    /// </summary>
    public partial class SettingsBlockLists : UserControl
    {
        private List<BlockCategory> blockCategories;
        private List<BlockItem> blockItems;
        private List<BlockItem> blockExceptions;
        private List<Penalty> penalties;

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
            penalties.Add(new Penalty { Id = 1, Type = "No, let me stop the focus session at any time", HasValue=false });
            penalties.Add(new Penalty { Id = 2, Type = "Yes, make me type to stop the focus session: ", HasValue = true, Value=30 });
            penalties.Add(new Penalty { Id = 3, Type = "Yes, make me restart my computer to stop the focus session", HasValue = false });

            this.DataContext = this;
        }

        public List<BlockItem> BlockItems
        {
            get
            {
                return blockItems;
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
