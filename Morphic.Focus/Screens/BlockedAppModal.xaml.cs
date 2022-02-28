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
    /// Interaction logic for BlockedAppModal.xaml
    /// </summary>
    public partial class BlockedAppModal : Window
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public BlockedAppModal()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            DataContext = this;
        }

        public BlockedAppModal(string appName) : this()
        {
            AppName = appName;

        }

        public string AppName { get; set; }

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (e.OriginalSource is TextBox)
                    {
                        return;
                    }
                    else
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
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
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

        private void btnStopFocus(object sender, RoutedEventArgs e)
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
    }
}
