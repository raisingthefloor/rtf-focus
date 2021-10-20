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
    /// Interaction logic for NewBlocklist.xaml
    /// </summary>
    public partial class NewBlocklistModal : Window
    {
        #region AppEngine and Constructor
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public NewBlocklistModal()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            this.DataContext = this;
        }
        #endregion

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
            try
            {
                LoggingService.WriteAppLog("btnCreateBlockList_Click");

                if (String.IsNullOrWhiteSpace(txtBlockList.Text.Trim()))
                {
                    MessageBox.Show("Please enter blocklist name");
                    return;
                }
                Engine.UserPreferences.BlockLists.Add(new Data.Models.Blocklist() { Name = txtBlockList.Text.Trim() });
                this.Close();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        public string BlockListName
        {
            get { return txtBlockList.Text; }
        }
    }
}
