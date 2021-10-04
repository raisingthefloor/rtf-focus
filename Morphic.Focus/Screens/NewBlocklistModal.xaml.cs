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
using System.Windows.Shapes;

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for NewBlocklist.xaml
    /// </summary>
    public partial class NewBlocklistModal : Window
    {
        public NewBlocklistModal()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Let the Window be dragged using mouse-press
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

        private void btnCreateBlockList_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string BlockListName
        {
            get { return txtBlockList.Text; }
        }
    }
}
