using Morphic.Data.Models;
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

        #region Events
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

                //Validation 1 : Blocklist name cannot be empty
                if (String.IsNullOrWhiteSpace(txtBlockList.Text.Trim()))
                {
                    MessageBox.Show("Please enter blocklist name");
                    return;
                }

                //Validation 2 : We should not have a blocklist created with the same name earlier
                if (Engine.UserPreferences.BlockLists.Any(p => p.Name.ToLowerInvariant() == txtBlockList.Text.Trim().ToLowerInvariant()))
                {
                    MessageBox.Show("A Blocklist with the name " + txtBlockList.Text.Trim() + " already exists.");
                    return;
                }

                Blocklist blocklist = new Blocklist() { Name = txtBlockList.Text.Trim() };

                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Notifications", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Email", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Communication (Not Email)", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Games", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Proxies", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Videos", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Social Media", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Shopping", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Porn", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "News", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Gambling", IsActive = false });
                blocklist.Blockcategories.Add(new Blockcategory() { Name = "Dating", IsActive = false });

                Engine.UserPreferences.BlockLists.Add(blocklist);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
        #endregion
        public string BlockListName
        {
            get { return txtBlockList.Text.Trim(); }
        }
    }
}
