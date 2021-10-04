using Microsoft.Win32;
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
    /// Interaction logic for UnblockAddAppWebsite.xaml
    /// </summary>
    public partial class AllowUnblockingModal : Window
    {
        private List<BlockCategory> blockCategories;
        public AllowUnblockingModal()
        {
            InitializeComponent();

            GetInstalledApps();
            this.DataContext = this;
        }

        public List<BlockCategory> BlockCategories
        {
            get
            {
                return blockCategories;
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
            List<string> installs = new List<string>();
            List<string> keys = new List<string>() {
              @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
              @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            // The RegistryView.Registry64 forces the application to open the registry as x64 even if the application is compiled as x86 
            FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), keys, installs);
            FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), keys, installs);

            installs = installs.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            installs.Sort(); // The list of ALL installed applications

            blockCategories = new List<BlockCategory>();
            foreach (string str in installs)
            {
                int i = 0;
                blockCategories.Add(new BlockCategory() { Name = str, Id = i++ });
            }
        }

        private void FindInstalls(RegistryKey regKey, List<string> keys, List<string> installed)
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
                                installed.Add(Convert.ToString(sk.GetValue("DisplayName")).Trim());
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
            }
        }
    }
}
