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
    /// Interaction logic for StopFocusRestartModal.xaml
    /// </summary>
    public partial class StopFocusRestartModal : Window
    {
        public StopFocusRestartModal()
        {
            InitializeComponent();
        }

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
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

        private void HideMe(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RestartComputer(object sender, RoutedEventArgs e)
        {
            //Opens up Win Menu
            KeyboardSend.OpenPowerBar();
            this.Close();
        }
    }
}
