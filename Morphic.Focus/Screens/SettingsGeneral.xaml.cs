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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for SettingsGeneral.xaml
    /// </summary>
    public partial class SettingsGeneral : UserControl, INotifyPropertyChanged
    {
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public SettingsGeneral()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            this.DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void btnAddAppUnblock_Click(object sender, RoutedEventArgs e)
        {
            AllowUnblockingModal unblockAddAppWebsite = new AllowUnblockingModal();
            unblockAddAppWebsite.ShowDialog();
        }

        private void btnAddWebsiteUnblock_Click(object sender, RoutedEventArgs e)
        {
            AddWebsiteModal addWebsiteModal = new AddWebsiteModal();
            addWebsiteModal.ShowDialog();
        }
    }

    #region To be deleted
    public class BlockItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    #endregion
}

