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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for SettingsTodaysSchedule.xaml
    /// </summary>
    public partial class SettingsTodaysSchedule : UserControl
    {
        public string todaysWeekday { get; set; }
        private List<FocusSchedule> focusSchedules;
        private List<CalendarData> calendarDataSource;
        public SettingsTodaysSchedule()
        {
            InitializeComponent();

            InitializeCalendarData();

            focusSchedules = new List<FocusSchedule>();
            focusSchedules.Add(new FocusSchedule { Id = 1, BlockListName = "Meditation Session", ListColor=Brushes.Purple });
            focusSchedules.Add(new FocusSchedule { Id = 2, BlockListName = "Weekend blocklist", ListColor = Brushes.Blue });

            todaysWeekday = DateTime.Today.ToString("ddd").ToUpper();
            this.DataContext = this;
        }

        /// <summary>
        /// This method sends color info to calendar view
        /// TODO - Logic of this method would be written
        /// </summary>
        private void InitializeCalendarData()
        {
            calendarDataSource = new List<CalendarData>();

            for (int i = 1; i <= 24; i++)
            {
                calendarDataSource.Add(new CalendarData
                {
                    Color1 = Brushes.Transparent,
                    Color2 = Brushes.Transparent,
                    I = i,
                    J = 1
                });
            }

            //For Demo
            for (int i = 10; i <= 17; i++)
            {
                CalendarData? item = calendarDataSource.Find(x => x.I == i && x.J == 1);
                if (item != null)
                    item.Color1 = Brushes.Purple;
            }
            for (int i = 12; i <= 14; i++)
            {
                CalendarData? item = calendarDataSource.Find(x => x.I == i && x.J == 1);
                if (item != null)
                    item.Color2 = Brushes.Blue;
            }

        }
        public List<CalendarData> CalendarDataSource
        {
            get
            {
                return calendarDataSource;
            }
        }

        public List<FocusSchedule> FocusSchedules
        {
            get
            {
                return focusSchedules;
            }
        }
    }
}
