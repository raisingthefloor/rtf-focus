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
    /// Interaction logic for winFocusMain.xaml
    /// </summary>
    public partial class FocusMain : Window
    {
        public FocusMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Allow user to drag the window
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

        #region Temporary - Will be removed
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FocusStatus focusStatus = new FocusStatus();
            focusStatus.Show();
        }

        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FocusBreakSequence focusBreakSequence = new FocusBreakSequence();
            focusBreakSequence.Show();
        }

        
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FocusSessionComplete focusSessionComplete = new FocusSessionComplete();
            focusSessionComplete.Show();

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Focus2HourBreak focus2HourBreak = new Focus2HourBreak();
            focus2HourBreak.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Focus2HourBreakUntilStop focus2HourBreakUntilStop = new Focus2HourBreakUntilStop();
            focus2HourBreakUntilStop.Show();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            FocusBreakEnd focusBreakEnd = new FocusBreakEnd();
            focusBreakEnd.Show();

            FocusReminder focusReminder = new FocusReminder();
            focusReminder.Show();
        }

        #endregion
    }
}
