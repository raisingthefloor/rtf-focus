using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for BlockAddWebsite.xaml
    /// </summary>
    public partial class AddWebsiteModal : Window
    {
        public ObservableCollection<ActiveAppsAndWebsites> AppsAppEngineList { get; set; }

        public AddWebsiteModal(ObservableCollection<ActiveAppsAndWebsites> appEngineList)
        {
            InitializeComponent();
            AppsAppEngineList = appEngineList;
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

        private void btnTestURL_Click(object sender, RoutedEventArgs e)
        {
            LoggingService.WriteAppLog("AddWebsiteModel -> btnTestURL_Click");

            if (!string.IsNullOrWhiteSpace(txtWebsiteURL.Text.Trim()))
            {
                try
                {
                    string url = new UriBuilder(txtWebsiteURL.Text.Trim()).Uri.ToString();
                    txtWebsiteURL.Text = url;
                    Process.Start("explorer", url);
                }
                catch (Exception ex)
                {
                    LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                }
            }
            else MessageBox.Show("Please enter Website URL");
        }

        private void btnCreateBlockList_Click(object sender, RoutedEventArgs e)
        {
            LoggingService.WriteAppLog("AddWebsiteModel -> btnCreateBlockList_Click");

            if (!string.IsNullOrWhiteSpace(txtWebsiteURL.Text.Trim()))
            {
                try
                {
                    Uri url = new UriBuilder(txtWebsiteURL.Text.Trim()).Uri;

                    AppsAppEngineList.Add(
                        new ActiveAppsAndWebsites()
                        {
                            IsActive = true,
                            IsApp = false,
                            Name = url.Host,
                            Path = string.Empty
                        });
                }
                catch (Exception ex)
                {
                    LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Please enter Website URL");
                return;
            }

            this.Close();
        }
    }
}
