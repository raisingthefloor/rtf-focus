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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for UCSchedule.xaml
    /// </summary>
    public partial class UCSchedule : UserControl
    {
        
        #region AppEngine and Constructor
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }
        public UCSchedule()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            this.DataContext = this;
        }
        #endregion

        private Schedule _schedule;
        public Schedule Schedule 
        {
            get
            {
                return _schedule;
            }
            set
            {
                _schedule = value;
            }
        }

        private Blocklist _blocklist;
        public Blocklist Blocklist
        {
            get
            {
                return _blocklist;
            }
            set
            {
                if (value != _blocklist)
                {
                    _blocklist = value;
                    if (_blocklist != null) Schedule.BlockListName = _blocklist.Name;
                }
            }
        }

        private void btnClearSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoggingService.WriteAppLog("UCSchedule -> btnClearSchedule_Click");

                Schedule.BlockListName = string.Empty;
                Schedule.StartAt = DateTime.Today;
                Schedule.EndAt = DateTime.Today;
                Schedule.IsActiveSunday = Schedule.IsActiveMonday = Schedule.IsActiveTuesday = Schedule.IsActiveWednesday = Schedule.IsActiveThursday =
                    Schedule.IsActiveFriday = Schedule.IsActiveSaturday = false;
                Schedule.IsActive = false;
                cmbBlockList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }
    }
}
