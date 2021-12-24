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
    /// Interaction logic for LockedScreenNonModal.xaml
    /// </summary>
    public partial class LockedScreenNonModal : Window
    {
        public LockedScreenNonModal()
        {
            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.5);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.5);
            InitializeComponent();

            
            grdMain.Height = this.Height;
            grdMain.Width = this.Width;
        }

        /// <summary>
        /// Hide this window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
