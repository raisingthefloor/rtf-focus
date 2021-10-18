using Microsoft.Win32;
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
    /// Interaction logic for UnblockAddAppWebsite.xaml
    /// </summary>
    public partial class AllowUnblockingModal : Window
    {
        private List<AppsAndWebsites> _apps;
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }
        public AllowUnblockingModal()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            GetInstalledApps();
            this.DataContext = this;
        }

        public List<AppsAndWebsites> Apps
        {
            get
            {
                return _apps;
            }
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

        public void GetInstalledApps()
        {
            //List<string> installs = new List<string>();
            List<string> keys = new List<string>() {
              @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths",
              @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths"
            };

            _apps = new List<AppsAndWebsites>();
            // The RegistryView.Registry64 forces the application to open the registry as x64 even if the application is compiled as x86 
            FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), keys, _apps);
            FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), keys, _apps);

            _apps = _apps.Distinct().OrderBy(p => p.Name).ToList();
        }

        private void FindInstalls(RegistryKey regKey, List<string> keys, List<AppsAndWebsites> installed)
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
                                    installed.Add(new AppsAndWebsites() { IsActive = false, IsApp = true, Name = skName.Trim(), Path = Convert.ToString(defvalue).Trim() });
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
            foreach (var item in _apps.Where(p => p.IsActive))
            {
                Engine.UserPreferences.General.TemporarilyUnblock.AppsAndWebsites.Add(item);
            }

            this.Close();
        }
    }
}
