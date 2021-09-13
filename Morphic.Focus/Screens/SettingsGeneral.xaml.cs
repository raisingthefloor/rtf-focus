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
    /// Interaction logic for SettingsGeneral.xaml
    /// </summary>
    public partial class SettingsGeneral : UserControl
    {
        private List<BlockItem> blockItems;
        public SettingsGeneral()
        {
            InitializeComponent();

            blockItems = new List<BlockItem>();
            blockItems.Add(new BlockItem { Id = 1, Name = "Mail"});
            blockItems.Add(new BlockItem { Id = 2, Name = "Skype"});
            blockItems.Add(new BlockItem { Id = 3, Name = "Linkedin.com"});

            this.DataContext = this;
        }

        public List<BlockItem> BlockItems
        {
            get
            {
                return blockItems;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class BlockItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
    }

}

