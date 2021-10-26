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
    /// Interaction logic for SettingsSchedule.xaml
    /// </summary>
    public partial class SettingsSchedule : UserControl
    {
        #region AppEngine and Constructor
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public SettingsSchedule()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            Schedule1.Schedule = Engine.UserPreferences.Schedules.Schedule1;
            Schedule2.Schedule = Engine.UserPreferences.Schedules.Schedule2;
            Schedule3.Schedule = Engine.UserPreferences.Schedules.Schedule3;
            Schedule4.Schedule = Engine.UserPreferences.Schedules.Schedule4;
            Schedule5.Schedule = Engine.UserPreferences.Schedules.Schedule5;

            if (!string.IsNullOrWhiteSpace(Schedule1.Schedule.BlockListName) &&
                Engine.UserPreferences.BlockLists.Any(p => p.Name == Schedule1.Schedule.BlockListName))
            {
                Schedule1.Blocklist = Engine.UserPreferences.BlockLists.Where(p => p.Name == Schedule1.Schedule.BlockListName).First();
            }

            if (!string.IsNullOrWhiteSpace(Schedule2.Schedule.BlockListName) &&
                Engine.UserPreferences.BlockLists.Any(p => p.Name == Schedule2.Schedule.BlockListName))
            {
                Schedule2.Blocklist = Engine.UserPreferences.BlockLists.Where(p => p.Name == Schedule2.Schedule.BlockListName).First();
            }

            if (!string.IsNullOrWhiteSpace(Schedule3.Schedule.BlockListName) &&
                Engine.UserPreferences.BlockLists.Any(p => p.Name == Schedule3.Schedule.BlockListName))
            {
                Schedule3.Blocklist = Engine.UserPreferences.BlockLists.Where(p => p.Name == Schedule3.Schedule.BlockListName).First();
            }

            if (!string.IsNullOrWhiteSpace(Schedule4.Schedule.BlockListName) &&
                Engine.UserPreferences.BlockLists.Any(p => p.Name == Schedule4.Schedule.BlockListName))
            {
                Schedule4.Blocklist = Engine.UserPreferences.BlockLists.Where(p => p.Name == Schedule4.Schedule.BlockListName).First();
            }

            if (!string.IsNullOrWhiteSpace(Schedule5.Schedule.BlockListName) &&
                Engine.UserPreferences.BlockLists.Any(p => p.Name == Schedule5.Schedule.BlockListName))
            {
                Schedule5.Blocklist = Engine.UserPreferences.BlockLists.Where(p => p.Name == Schedule5.Schedule.BlockListName).First();
            }

            var bc = new BrushConverter();
            Schedule1.scheduleColor.Background = (Brush)bc.ConvertFrom("#662261");
            Schedule2.scheduleColor.Background = (Brush)bc.ConvertFrom("#0080A8");
            Schedule3.scheduleColor.Background = Brushes.Orange; //bc.ConvertFrom(  (Brush)bc.ConvertFrom("#002957");
            Schedule4.scheduleColor.Background = (Brush)bc.ConvertFrom("#008145");
            Schedule5.scheduleColor.Background = (Brush)bc.ConvertFrom("#ff0000");

            focusSchedules = new List<FocusSchedule>();
            focusSchedules.Add(new FocusSchedule { Id = 1, StartAt= DateTime.Now });
            focusSchedules.Add(new FocusSchedule { Id = 2, StartAt = DateTime.Now });
            focusSchedules.Add(new FocusSchedule { Id = 3, StartAt = DateTime.Now });
            focusSchedules.Add(new FocusSchedule { Id = 4, StartAt = DateTime.Now });
            focusSchedules.Add(new FocusSchedule { Id = 5, StartAt = DateTime.Now });

            InitializeCalendarData();

            this.DataContext = this;
        }
        #endregion

        private List<FocusSchedule> focusSchedules;
        private List<CalendarData> calendarDataSource;
        

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

        public DateTime StartAt { get; set; }
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
