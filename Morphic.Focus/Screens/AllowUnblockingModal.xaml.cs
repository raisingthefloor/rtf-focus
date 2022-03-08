using Microsoft.Win32;
using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for UnblockAddAppWebsite.xaml
    /// </summary>
    public partial class AllowUnblockingModal : Window
    {
        private List<ActiveAppsAndWebsites> _apps;
        public List<ActiveAppsAndWebsites> AppsLocalList { get => _apps; set => _apps = value; }

        public ObservableCollection<ActiveAppsAndWebsites> AppsAppEngineList { get; set; }

        public string TitleDesc { get; set; }

        public AllowUnblockingModal(ObservableCollection<ActiveAppsAndWebsites> appEngineList, string headerText)
        {
            InitializeComponent();

            AppsAppEngineList = appEngineList;
            GetInstalledApps();
            TitleDesc = headerText;

            this.DataContext = this;
        }

        /// <summary>
        /// Let the Window be dragged using mouse-press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (e.OriginalSource is System.Windows.Controls.Primitives.Thumb)
                    {
                        return;
                    }
                    else
                    {
                        //try
                        //{
                        //    this.DragMove();
                        //}
                        //catch (Exception ex)
                        //{
                        //    LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        public void GetInstalledApps()
        {
            List<string> keys = new List<string>() {
              @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths",
              @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths"
            };

            AppsLocalList = new List<ActiveAppsAndWebsites>();
            // The RegistryView.Registry64 forces the application to open the registry as x64 even if the application is compiled as x86 
            FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), keys, AppsLocalList);
            FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), keys, AppsLocalList);

            AppsLocalList = AppsLocalList.Distinct().OrderBy(p => p.Name).ToList();


            foreach (ActiveAppsAndWebsites item in AppsAppEngineList)
            {
                ActiveAppsAndWebsites existingItem = AppsLocalList.Find(p => p.IsApp == item.IsApp && p.Name == item.Name && p.Path == item.Path);
                if (existingItem != null) existingItem.IsActive = item.IsActive;
            }
        }

        private void FindInstalls(RegistryKey regKey, List<string> keys, List<ActiveAppsAndWebsites> installed)
        {
            foreach (string key in keys)
            {
                using (RegistryKey rk = regKey.OpenSubKey(key))
                {
                    if (rk == null)
                    {
                        continue;
                    }
                    foreach (string skName in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(skName))
                        {
                            try
                            {
                                var defvalue = sk?.GetValue("");
                                if (defvalue != null)
                                {
                                    installed.Add(new ActiveAppsAndWebsites() { IsActive = false, IsApp = true, Name = skName.Trim(), Path = Convert.ToString(defvalue).Trim() });
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
            }
        }

        private void btnBlockAddApp_Click(object sender, RoutedEventArgs e)
        {
            LoggingService.WriteAppLog("btnBlockAddApp_Click");

            //TODO - What if user deselects an item in the dialog
            foreach (var item in AppsLocalList.Where(p => p.IsActive))
            {
                List<ActiveAppsAndWebsites> existingItem = AppsAppEngineList.Where(p => p.IsApp == item.IsApp && p.Name == item.Name && p.Path == item.Path).ToList();

                if (existingItem.Count == 0)
                {
                    AppsAppEngineList.Add(item);
                }
                else if (existingItem.Count == 1)
                {
                    existingItem[0].IsActive = item.IsActive;
                }
                else
                {
                    //TODO
                    //What if more than one item received
                }
            }

            this.Close();
        }
    }
}
