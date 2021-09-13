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
    /// Interaction logic for SettingsSchedule.xaml
    /// </summary>
    public partial class SettingsSchedule : UserControl
    {
        private List<FocusSchedule> focusSchedules;
        private List<CalendarData> calendarDataSource;
        public SettingsSchedule()
        {
            InitializeComponent();

            focusSchedules = new List<FocusSchedule>();
            focusSchedules.Add(new FocusSchedule { Id = 1 });
            focusSchedules.Add(new FocusSchedule { Id = 2 });
            focusSchedules.Add(new FocusSchedule { Id = 3 });
            focusSchedules.Add(new FocusSchedule { Id = 4 });
            focusSchedules.Add(new FocusSchedule { Id = 5 });

            InitializeCalendarData();

            this.DataContext = this;
        }

        /// <summary>
        /// This method sends color info to calendar view
        /// TODO - Logic of this method would be written
        /// </summary>
        private void InitializeCalendarData()
        {
            calendarDataSource = new List<CalendarData>();
            
            for (int i=1; i<=24; i++)
                for (int j = 1; j <= 7; j++)
                    calendarDataSource.Add(new CalendarData { Color1 = Brushes.Transparent,
            Color2 = Brushes.Transparent, I=i, J=j});

            //For Demo
            for (int i = 10; i <= 17; i++)
            {
                CalendarData? item = calendarDataSource.Find(x => x.I == i && x.J == 1);
                if (item!=null)
                    item.Color1 = Brushes.Purple;
            }
            for (int i = 12; i <= 14; i++)
            {
                CalendarData? item = calendarDataSource.Find(x => x.I == i && x.J == 1);
                if (item != null)
                    item.Color2 = Brushes.Blue;
            }

        }

        public List<FocusSchedule> FocusSchedules
        {
            get
            {
                return focusSchedules;
            }
        }

        public List<CalendarData> CalendarDataSource
        {
            get
            {
                return calendarDataSource;
            }
        }
    }


    public class FocusSchedule
    {
        public int Id { get; set; }
        public string BlockListName { get; set; }

        public TimeSpan StartAt { get; set; }
        public TimeSpan EndAt { get; set; }
        public List<DayOfWeek> ScheduledDays { get; set; }
        public bool IsActive { get; set; }
        public SolidColorBrush ListColor { get; set; }
    }

    public class CalendarData
    {
        public SolidColorBrush Color1 { get; set; }
        public SolidColorBrush Color2 { get; set; }

        public int I { get; set; }
        public int J { get; set; }

    }
}
