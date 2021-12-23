using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for FocusBreakEnd.xaml
    /// </summary>
    public partial class EndofBreakModal : Window
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public EndofBreakModal()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            DataContext = this;
        }

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch (Exception ex)
                {
                    LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                }
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
        private void btnStopFocus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Log Closing Session
                LoggingService.WriteAppLog("Session Closing");

                if (Engine.Session1 != null)
                    Engine.StopFocusSession(Engine.Session1);

                //Hide this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void StartFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Engine.StartFocusSequence());

                //Closes this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature will be available soon!");
        }

        
    }
}
