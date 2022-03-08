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
    /// Interaction logic for Focus2HourBreak.xaml
    /// </summary>
    public partial class LongBreakModal : Window
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }
        public LongBreakModal()
        {
            try
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    _engine = AppEngine.Instance;
                }

                InitializeComponent();

                DataContext = this;
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    try
            //    {
            //        this.DragMove();
            //    }
            //    catch (Exception ex)
            //    {
            //        LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            //    }
            //}
        }

        /// <summary>
        /// Hide this window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();
        }
        private void btnStopFocus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Log Closing Session
                LoggingService.WriteAppLog("Session Closing");

                if (Engine.Session1 != null)
                    Engine.EndSession(Engine.Session1, true);

                //Hide this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btn15Min_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Engine.StartBreakSequence(Common.Min15));

                //Closes this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btn30Min_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Engine.StartBreakSequence(Common.Min30));

                //Closes this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btn60Min_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Engine.StartBreakSequence(Common.Min60));

                //Closes this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnRegularBreak_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => Engine.StartBreakSequence(Engine.Session1.BreakDuration));

                //Closes this dialog
                this.Hide();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true; //Do not allow the window to close
        }
    }
}
