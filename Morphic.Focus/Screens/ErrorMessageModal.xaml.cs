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
    /// Interaction logic for FocusSchErroractiveSessionModal.xaml
    /// </summary>
    public partial class ErrorMessageModal : Window
    {
        public ErrorMessageModal()
        {
            InitializeComponent();

            DataContext = this;
        }

        #region Properties
        public string TitleText { get; set; }
        public string ContentText { get; set; }
        public string ContentSmallText { get; set; }
        #endregion

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    this.DragMove();
            //}
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
        #endregion
    }
}
