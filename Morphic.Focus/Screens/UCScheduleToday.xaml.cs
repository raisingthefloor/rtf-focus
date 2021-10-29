using Morphic.Data.Models;
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
    /// Interaction logic for UCScheduleToday.xaml
    /// </summary>
    public partial class UCScheduleToday : UserControl
    {
        #region AppEngine and Constructor
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }
        public UCScheduleToday()
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

        internal void ResetVisibility()
        {
            this.Visibility = SetVisibility();
        }

        private Visibility SetVisibility()
        {
            //Hide is Schedule not available (Will likely never happen)
            if (Schedule == null)
            {
                return Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(Schedule.BlockListName)) //Hide if no blocklists
            {
                return Visibility.Collapsed;
            }
            else if (!IsActiveToday(Schedule)) //Hide if not active for this weekday
            { 
                return Visibility.Collapsed;
            }
            else
                return Visibility.Visible; //Visible
        }

        private bool IsActiveToday(Schedule schedule)
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return schedule.IsActiveSunday;
                case DayOfWeek.Monday:
                    return schedule.IsActiveMonday;
                case DayOfWeek.Tuesday:
                    return schedule.IsActiveTuesday;
                case DayOfWeek.Wednesday:
                    return schedule.IsActiveWednesday;
                case DayOfWeek.Thursday:
                    return schedule.IsActiveThursday;
                case DayOfWeek.Friday:
                    return schedule.IsActiveFriday;
                case DayOfWeek.Saturday:
                    return schedule.IsActiveSaturday;
            }
            return false;
        }
    }
}
