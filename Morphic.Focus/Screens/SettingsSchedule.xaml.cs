using Morphic.Core;
using Morphic.Data.Models;
using Morphic.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

            //Set the Blocklists property of the Schedule UserControl
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
            // NOTE: we do a basic variation on validating properties here; this is not ideal, as ideally we'd have a validation that happened before the property
            //       changed; ideally this validation should be broken out in the future into validation code instead.
            switch (e.PropertyName)
            {
                case "IsActiveSunday":
                case "IsActiveMonday":
                case "IsActiveTuesday":
                case "IsActiveWednesday":
                case "IsActiveThursday":
                case "IsActiveFriday":
                case "IsActiveSaturday":
                    {
                        // NOTE: for this property type, we'll set the property to validate it (i.e. get a success/failure back from setting it); our code will also go 
                        //       ahead and re-sync the schedules afterwards (which is somewhat redundant), but that's necessary in the currently implementation to make sure
                        //       that all data binding is triggered; in the future, we will ideally move to a validation model...and discretely update values (instead of re-sync)
                        if (sender is null)
                        {
                            // if sender is null, show the user an error and then exit our special-case handler
                            this.InvokeInternalErrorDialog();
                            break;
                        }

                        var schedule = (Schedule)sender;

                        var dayOfWeek = SettingsSchedule.GetDayOfWeekFromIsActivePropertyName(e.PropertyName);
                        var isActive = SettingsSchedule.GetDayOfWeekIsActiveProperty(schedule, e.PropertyName);

                        // if we are adding a day to the schedule, handle the property change as a special case (where we make sure it's a valid change)
                        if (isActive == true)
                        {
                            var getBrushResult = this.GetBrushForSchedule(schedule);
                            if (getBrushResult.IsError == true)
                            {
                                // if we could not get a brush for the schedule, show the user an error and then exit our special-case handler
                                this.InvokeInternalErrorDialog();
                                break;
                            }
                            var brush = getBrushResult.Value!;
                            
                            var setDayToActiveResult = AddforDay(schedule, brush, isActive, dayOfWeek);
                            if (setDayToActiveResult.IsError == true)
                            {
                                // set the day back to FALSE to indicate that we can't set the day for the user
                                // NOTE: this should be safe (non-reentrant); if this causes us issues, we'll have to find another way to handle "ignoring" user schedule checkbox set-to-checked events
                                SettingsSchedule.SetDayOfWeekIsActiveProperty(schedule, e.PropertyName, false);

                                // show the user an error indicating that we couldn't add the day to the schedule
                                this.InvokeScheduleErrorDialog();
                            }
                        }
                    }
                    break;
                default:
                    // fall through; we don't validate this property
                    break;
            }

            // NOTE: even if the property change was validated (by discretely adding the data), we still re-initialize the calendar data here to make sure we raise all necessary events
            //       [ideally, we'd have validation logic simply block changes instead...and would have separate routines to add/remove/update various pieces of data]
            InitializeCalendarData();
        }

        private void InvokeInternalErrorDialog()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ErrorMessageModal errorMessageModal = new ErrorMessageModal()
                    {
                        TitleText = "Sorry, I'm having trouble doing this right now.",
                        ContentText = $"Please try again or contact support for assistance."
                    };

                    errorMessageModal.ShowDialog();

                });
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void InvokeScheduleErrorDialog()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ErrorMessageModal errorMessageModal = new ErrorMessageModal()
                    {
                        TitleText = AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED + " Focus session(s) already scheduled",
                        ContentText = "More than " + AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED + " focus session(s) cannot be scheduled for the same time.\nTry editing your schedule."
                    };

                    errorMessageModal.ShowDialog();

                });
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
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
            new KeyValuePair<int, string>(Common.Min15, "15 min"),
            new KeyValuePair<int, string>(Common.Min20, "20 min"),
            new KeyValuePair<int, string>(Common.Min25, "25 min"),
            new KeyValuePair<int, string>(Common.Min30, "30 min"),
            new KeyValuePair<int, string>(Common.Min45, "45 min"),
            new KeyValuePair<int, string>(Common.Min60, "60 min"),
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

        private MorphicResult<MorphicUnit, MorphicUnit> AddSchedule(Schedule schedule, Brush brush)
        {
            //Process only if the schedule for specific day(s) is Active
            var addSundayResult = AddforDay(schedule, brush, schedule.IsActiveSunday, 0);
            var addMondayResult = AddforDay(schedule, brush, schedule.IsActiveMonday, 1);
            var addTuesdayResult = AddforDay(schedule, brush, schedule.IsActiveTuesday, 2);
            var addWednesdayResult = AddforDay(schedule, brush, schedule.IsActiveWednesday, 3);
            var addThursdayResult = AddforDay(schedule, brush, schedule.IsActiveThursday, 4);
            var addFridayResult = AddforDay(schedule, brush, schedule.IsActiveFriday, 5);
            var addSaturdayResult = AddforDay(schedule, brush, schedule.IsActiveSaturday, 6);

            if (addSundayResult.IsSuccess == true &&
                addMondayResult.IsSuccess == true &&
                addTuesdayResult.IsSuccess == true &&
                addWednesdayResult.IsSuccess == true &&
                addThursdayResult.IsSuccess == true &&
                addFridayResult.IsSuccess == true &&
                addSaturdayResult.IsSuccess == true)
            {
                return MorphicResult.OkResult();
            } 
            else
            {
                return MorphicResult.ErrorResult();
            }
        }

        private MorphicResult<MorphicUnit, MorphicUnit> AddforDay(Schedule schedule, Brush brush, bool day, int dayValue)
        {
            //Process for Weekday (e.g. Sunday)
            if (day == true)
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

                    // change made in the first slot; return success
                    return MorphicResult.OkResult();
                }

                // if the first slot was not available, then try the second slot
                if (AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED == 2)
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

                        // change made in the second slot; return success
                        return MorphicResult.OkResult();
                    }
                }
                else if (AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED > 2)
                {
                    throw new Exception("Invalid code path; constant " + nameof(AppEngine.NUMBER_OF_SIMULTANEOUS_SESSIONS_ALLOWED) + " is out of range.");
                }

                // if no slots were available, return an error condition
                return MorphicResult.ErrorResult();
            }
            else
            {
                // if there's nothing to add to the schedule, return success
                return MorphicResult.OkResult();
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

        // helper functions

        public MorphicResult<Brush, MorphicUnit> GetBrushForSchedule(Schedule schedule)
        {
            if (schedule == Engine.UserPreferences.Schedules.Schedule1)
            {
                return MorphicResult.OkResult(AppEngine.Schedule1Brush);
            }
            else if (schedule == Engine.UserPreferences.Schedules.Schedule2)
            {
                return MorphicResult.OkResult(AppEngine.Schedule2Brush);
            }
            else if (schedule == Engine.UserPreferences.Schedules.Schedule3)
            {
                return MorphicResult.OkResult(AppEngine.Schedule3Brush);
            }
            else if (schedule == Engine.UserPreferences.Schedules.Schedule4)
            {
                return MorphicResult.OkResult(AppEngine.Schedule4Brush);
            }
            else if (schedule == Engine.UserPreferences.Schedules.Schedule5)
            {
                return MorphicResult.OkResult(AppEngine.Schedule5Brush);
            }
            else
            {
                return MorphicResult.ErrorResult();
            }
        }


        private static int GetDayOfWeekFromIsActivePropertyName(string propertyName)
        {
            switch (propertyName)
            {
                case "IsActiveSunday":
                    return 0;
                case "IsActiveMonday":
                    return 1;
                case "IsActiveTuesday":
                    return 2;
                case "IsActiveWednesday":
                    return 3;
                case "IsActiveThursday":
                    return 4;
                case "IsActiveFriday":
                    return 5;
                case "IsActiveSaturday":
                    return 6;
            }

            // if the supplied property name is invalid, throw an exception
            throw new ArgumentOutOfRangeException(nameof(propertyName));
        }

        private static bool GetDayOfWeekIsActiveProperty(Schedule schedule, string propertyName)
        {
            switch (propertyName)
            {
                case "IsActiveSunday":
                    return schedule.IsActiveSunday;
                    break;
                case "IsActiveMonday":
                    return schedule.IsActiveMonday;
                    break;
                case "IsActiveTuesday":
                    return schedule.IsActiveTuesday;
                    break;
                case "IsActiveWednesday":
                    return schedule.IsActiveWednesday;
                    break;
                case "IsActiveThursday":
                    return schedule.IsActiveThursday;
                    break;
                case "IsActiveFriday":
                    return schedule.IsActiveFriday;
                    break;
                case "IsActiveSaturday":
                    return schedule.IsActiveSaturday;
                    break;
                default:
                    // if the supplied property name is invalid, throw an exception
                    throw new ArgumentOutOfRangeException(nameof(propertyName));
            }
        }

        private static void SetDayOfWeekIsActiveProperty(Schedule schedule, string propertyName, bool value)
        {
            switch (propertyName)
            {
                case "IsActiveSunday":
                    schedule.IsActiveSunday = value;
                    break;
                case "IsActiveMonday":
                    schedule.IsActiveMonday = value;
                    break;
                case "IsActiveTuesday":
                    schedule.IsActiveTuesday = value;
                    break;
                case "IsActiveWednesday":
                    schedule.IsActiveWednesday = value;
                    break;
                case "IsActiveThursday":
                    schedule.IsActiveThursday = value;
                    break;
                case "IsActiveFriday":
                    schedule.IsActiveFriday = value;
                    break;
                case "IsActiveSaturday":
                    schedule.IsActiveSaturday = value;
                    break;
                default:
                    // if the supplied property name is invalid, throw an exception
                    throw new ArgumentOutOfRangeException(nameof(propertyName));
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
