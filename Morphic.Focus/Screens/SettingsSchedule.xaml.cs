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
    /// Interaction logic for SettingsSchedule.xaml
    /// </summary>
    public partial class SettingsSchedule : UserControl, INotifyPropertyChanged
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

            //Set Schedule Property of the Schedule UserControl
            Schedule1.Schedule = Engine.UserPreferences.Schedules.Schedule1;
            Schedule2.Schedule = Engine.UserPreferences.Schedules.Schedule2;
            Schedule3.Schedule = Engine.UserPreferences.Schedules.Schedule3;
            Schedule4.Schedule = Engine.UserPreferences.Schedules.Schedule4;
            Schedule5.Schedule = Engine.UserPreferences.Schedules.Schedule5;

            //If Today's Schedule gets active/inactive then notify so that below calendar view can be refreshed
            Schedule1.Schedule.PropertyChanged += Schedule_PropertyChanged;
            Schedule2.Schedule.PropertyChanged += Schedule_PropertyChanged;
            Schedule3.Schedule.PropertyChanged += Schedule_PropertyChanged;
            Schedule4.Schedule.PropertyChanged += Schedule_PropertyChanged;
            Schedule5.Schedule.PropertyChanged += Schedule_PropertyChanged;

            //Set the Bloacklists property of the Schedule UserControl
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

            //Set the Color of the Schedules
            Schedule1.ScheduleColor = AppEngine.Schedule1Brush;
            Schedule2.ScheduleColor = AppEngine.Schedule2Brush;
            Schedule3.ScheduleColor = AppEngine.Schedule3Brush;
            Schedule4.ScheduleColor = AppEngine.Schedule4Brush;
            Schedule5.ScheduleColor = AppEngine.Schedule5Brush;

            //Set the Calendar View
            InitializeCalendarData();

            this.DataContext = this;
        }

        /// <summary>
        /// Reset Calendar View Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Schedule_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            InitializeCalendarData();
        }
        #endregion

        private static readonly KeyValuePair<int, string>[] _breakDuration = {
            new KeyValuePair<int, string>(1, "1 min"),
            new KeyValuePair<int, string>(3, "3 min"),
            new KeyValuePair<int, string>(5, "5 min"),
            new KeyValuePair<int, string>(10, "10 min"),
        };

        public KeyValuePair<int, string>[] BreakDuration
        {
            get
            {
                return _breakDuration;
            }
        }

        private static readonly KeyValuePair<int, string>[] _breakGap = {
            new KeyValuePair<int, string>(15, "15 min"),
            new KeyValuePair<int, string>(20, "20 min"),
            new KeyValuePair<int, string>(25, "25 min"),
            new KeyValuePair<int, string>(30, "30 min"),
            new KeyValuePair<int, string>(45, "45 min"),
            new KeyValuePair<int, string>(60, "60 min"),
        };

        public KeyValuePair<int, string>[] BreakGap
        {
            get
            {
                return _breakGap;
            }
        }

        /// <summary>
        /// This method sends color info to calendar view
        /// TODO - Logic of this method would be written
        /// </summary>
        private void InitializeCalendarData()
        {
            CalendarDataSource.Clear();

            //Set Empty Data
            for (int i = 0; i < 24; i++)
                for (int j = 0; j < 7; j++)
                    CalendarDataSource.Add(new CalendarData
                    {
                        Color1 = Brushes.Transparent,
                        BorderColor1 = Brushes.Transparent,
                        BorderColor2 = Brushes.Transparent,
                        Color2 = Brushes.Transparent,
                        I = i,
                        J = j
                    });

            AddSchedule(Engine.UserPreferences.Schedules.Schedule1, AppEngine.Schedule1Brush);
            AddSchedule(Engine.UserPreferences.Schedules.Schedule2, AppEngine.Schedule2Brush);
            AddSchedule(Engine.UserPreferences.Schedules.Schedule3, AppEngine.Schedule3Brush);
            AddSchedule(Engine.UserPreferences.Schedules.Schedule4, AppEngine.Schedule4Brush);
            AddSchedule(Engine.UserPreferences.Schedules.Schedule5, AppEngine.Schedule5Brush);

        }

        private void AddSchedule(Schedule schedule, Brush brush)
        {
            //Process only if the schedule is Active
            AddforDay(schedule, brush, schedule.IsActiveSunday, 0);
            AddforDay(schedule, brush, schedule.IsActiveMonday, 1);
            AddforDay(schedule, brush, schedule.IsActiveTuesday, 2);
            AddforDay(schedule, brush, schedule.IsActiveWednesday, 3);
            AddforDay(schedule, brush, schedule.IsActiveThursday, 4);
            AddforDay(schedule, brush, schedule.IsActiveFriday, 5);
            AddforDay(schedule, brush, schedule.IsActiveSaturday, 6);
        }

        private void AddforDay(Schedule schedule, Brush brush, bool day, int dayValue)
        {
            //Process for Weekday (e.g. Sunday)
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
                        //if (item.Color1 != Brushes.Transparent)
                        //{
                        //    isFirstSlotAvailable = false;
                        //    break;
                        //}
                        if (item.BorderColor1 != Brushes.Transparent)
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
                            item.Color1 = schedule.IsActive ? brush : Brushes.Transparent;
                            item.BorderColor1 = brush;
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
                            //if (item.Color2 != Brushes.Transparent)
                            //{
                            //    isSecondSlotAvailable = false;
                            //    break;
                            //}
                            if (item.BorderColor2 != Brushes.Transparent)
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
                                item.Color2 = schedule.IsActive ? brush : Brushes.Transparent;
                                item.BorderColor2 = brush;
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

    public class CalendarData : BaseClass, IEquatable<CalendarData?>
    {
        public Brush Color1 { get; set; }
        public Brush Color2 { get; set; }

        public Brush BorderColor2 { get; set; }

        public Brush BorderColor1 { get; set; }

        public int I { get; set; }
        public int J { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CalendarData);
        }

        public bool Equals(CalendarData? other)
        {
            return other != null &&
                   EqualityComparer<Brush>.Default.Equals(Color1, other.Color1) &&
                   EqualityComparer<Brush>.Default.Equals(Color2, other.Color2) &&
                   EqualityComparer<Brush>.Default.Equals(BorderColor2, other.BorderColor2) &&
                   EqualityComparer<Brush>.Default.Equals(BorderColor1, other.BorderColor1) &&
                   I == other.I &&
                   J == other.J;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color1, Color2, BorderColor1, BorderColor2, I, J);
        }

        public static bool operator ==(CalendarData? left, CalendarData? right)
        {
            return EqualityComparer<CalendarData>.Default.Equals(left, right);
        }

        public static bool operator !=(CalendarData? left, CalendarData? right)
        {
            return !(left == right);
        }
    }
}
