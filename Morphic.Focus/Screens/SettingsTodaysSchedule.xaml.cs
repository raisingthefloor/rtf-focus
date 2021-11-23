using Morphic.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class SettingsTodaysSchedule : UserControl, INotifyPropertyChanged
    {
        public string todaysWeekday { get; set; }

        #region AppEngine and Constructor
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public SettingsTodaysSchedule()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            Schedule1.Schedule = Engine.UserPreferences.TodaysSchedule.Schedule1;
            Schedule2.Schedule = Engine.UserPreferences.TodaysSchedule.Schedule2;
            Schedule3.Schedule = Engine.UserPreferences.TodaysSchedule.Schedule3;
            Schedule4.Schedule = Engine.UserPreferences.TodaysSchedule.Schedule4;
            Schedule5.Schedule = Engine.UserPreferences.TodaysSchedule.Schedule5;

            Schedule1.Schedule.PropertyChanged += Schedule_PropertyChanged;
            Schedule2.Schedule.PropertyChanged += Schedule_PropertyChanged;
            Schedule3.Schedule.PropertyChanged += Schedule_PropertyChanged;
            Schedule4.Schedule.PropertyChanged += Schedule_PropertyChanged;
            Schedule5.Schedule.PropertyChanged += Schedule_PropertyChanged;

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

            //TODO - Move to Resources
            var bc = new BrushConverter();
            Schedule1.scheduleColor.Background = AppEngine.Schedule1Brush;
            Schedule2.scheduleColor.Background = AppEngine.Schedule2Brush;
            Schedule3.scheduleColor.Background = AppEngine.Schedule3Brush; //bc.ConvertFrom(  (Brush)bc.ConvertFrom("#002957");
            Schedule4.scheduleColor.Background = AppEngine.Schedule4Brush;
            Schedule5.scheduleColor.Background = AppEngine.Schedule5Brush;

            ResetBlockListVisibility();
            InitializeCalendarData();

            todaysWeekday = DateTime.Today.ToString("ddd").ToUpper();
            this.DataContext = this;
        }

        private void Schedule_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            InitializeCalendarData();
            ResetBlockListVisibility();
        }

        private void ResetBlockListVisibility()
        {
            Schedule1.ResetVisibility();
            Schedule2.ResetVisibility();
            Schedule3.ResetVisibility();
            Schedule4.ResetVisibility();
            Schedule5.ResetVisibility();
        }
        #endregion

        /// <summary>
        /// This method sends color info to calendar view
        /// TODO - Logic of this method would be written
        /// </summary>
        private void InitializeCalendarData()
        {
            CalendarDataSource.Clear();

            for (int i = 0; i < 24; i++)
                    CalendarDataSource.Add(new CalendarData
                    {
                        Color1 = Brushes.Transparent,
                        Color2 = Brushes.Transparent,
                        I = i,
                        J = (int)DateTime.Today.DayOfWeek
                    });

            AddSchedule(Engine.UserPreferences.TodaysSchedule.Schedule1, AppEngine.Schedule1Brush);
            AddSchedule(Engine.UserPreferences.TodaysSchedule.Schedule2, AppEngine.Schedule2Brush);
            AddSchedule(Engine.UserPreferences.TodaysSchedule.Schedule3, AppEngine.Schedule3Brush);
            AddSchedule(Engine.UserPreferences.TodaysSchedule.Schedule4, AppEngine.Schedule4Brush);
            AddSchedule(Engine.UserPreferences.TodaysSchedule.Schedule5, AppEngine.Schedule5Brush);

        }

        private void AddSchedule(Schedule schedule, Brush brush)
        {
            //Process only if the schedule is Active
            if (schedule.IsActive)
            {
                AddforDay(schedule, brush, schedule.IsActiveSunday, 0);
                AddforDay(schedule, brush, schedule.IsActiveMonday, 1);
                AddforDay(schedule, brush, schedule.IsActiveTuesday, 2);
                AddforDay(schedule, brush, schedule.IsActiveWednesday, 3);
                AddforDay(schedule, brush, schedule.IsActiveThursday, 4);
                AddforDay(schedule, brush, schedule.IsActiveFriday, 5);
                AddforDay(schedule, brush, schedule.IsActiveSaturday, 6);
            }
        }

        private void AddforDay(Schedule schedule, Brush brush, bool day, int dayValue)
        {
            //Process for Sunday
            if (day)
            {
                //Get the start and end time
                TimeSpan startAt = schedule.StartAt.TimeOfDay;
                TimeSpan endAt = schedule.EndAt.TimeOfDay;
                int startHour = startAt.Hours; //12 AM = 0, 1 AM = 1, and so on
                int endHour = endAt.Minutes == 0 ? endAt.Hours - 1 : endAt.Hours; //12:00 AM = 0, 12:10 AM = 1, 1:00 AM = 0, 1:15 AM = 1, and so on

                //Check if first color slot is available
                bool isFirstSlotAvailable = true; //Assume it is available
                for (int i = startHour; i <= endHour; i++)
                {
                    //Make the check

                    if (CalendarDataSource.Any(x => x.I == i && x.J == dayValue))
                    {
                        CalendarData? item = CalendarDataSource.Where(x => x.I == i && x.J == dayValue).First();
                        if (item.Color1 != Brushes.Transparent)
                        {
                            isFirstSlotAvailable = false;
                            break;
                        }
                    }
                }

                //If First Color Slot is available, assign the color to the slot
                if (isFirstSlotAvailable)
                {
                    for (int i = startHour; i <= endHour; i++)
                    {
                        //Make the check
                        if (CalendarDataSource.Any(x => x.I == i && x.J == dayValue))
                        {
                            CalendarData item = CalendarDataSource.Where(x => x.I == i && x.J == dayValue).First();
                            item.Color1 = brush;
                        }
                    }
                }
                else
                {
                    //Check if second color slot is available
                    bool isSecondSlotAvailable = true; //Assume it is available
                    for (int i = startHour; i <= endHour; i++)
                    {
                        //Make the check
                        if (CalendarDataSource.Any(x => x.I == i && x.J == dayValue))
                        {
                            CalendarData? item = CalendarDataSource.Where(x => x.I == i && x.J == dayValue).First();
                            if (item.Color2 != Brushes.Transparent)
                            {
                                isSecondSlotAvailable = false;
                                break;
                            }
                        }
                    }

                    //If Second Color Slot is available, assign the color to the slot
                    if (isSecondSlotAvailable)
                    {
                        for (int i = startHour; i <= endHour; i++)
                        {
                            //Make the check
                            if (CalendarDataSource.Any(x => x.I == i && x.J == dayValue))
                            {
                                CalendarData item = CalendarDataSource.Where(x => x.I == i && x.J == dayValue).First();
                                item.Color2 = brush;
                            }
                        }
                    }
                }
            }
        }

        private ObservableCollection<CalendarData> calendarDataSource;

        public ObservableCollection<CalendarData> CalendarDataSource
        {
            get
            {
                if (calendarDataSource == null)
                {
                    calendarDataSource = new ObservableCollection<CalendarData>();
                    calendarDataSource.CollectionChanged += CalendarDataSource_CollectionChanged;
                }
                return calendarDataSource;
            }
            set
            {
                if (value != calendarDataSource)
                {
                    calendarDataSource = value;
                    calendarDataSource.CollectionChanged += CalendarDataSource_CollectionChanged; ;
                    foreach (CalendarData item in calendarDataSource)
                        item.PropertyChanged += Item_PropertyChanged;

                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        private void CalendarDataSource_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (CalendarData item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (CalendarData item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            NotifyPropertyChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
