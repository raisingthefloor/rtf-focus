
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Morphic.Data.Models
{
    public class UserPreferences : INotifyPropertyChanged
    {
        private General _general;
        public General General
        {
            get
            {
                if (_general == null)
                {
                    _general = new General();
                    _general.PropertyChanged += General_PropertyChanged;
                }
                return _general;
            }
            set
            {
                if (value != _general)
                {
                    _general = value;
                    _general.PropertyChanged += General_PropertyChanged;
                }
            }
        }

        private ObservableCollection<Blocklist> _blockLists;

        public ObservableCollection<Blocklist> BlockLists
        {
            get
            {
                if (_blockLists == null)
                {
                    _blockLists = new ObservableCollection<Blocklist>();
                    _blockLists.CollectionChanged += _blockLists_CollectionChanged; ;
                }
                return _blockLists;
            }
            set
            {
                if (value != _blockLists)
                {
                    _blockLists = value;
                    _blockLists.CollectionChanged += _blockLists_CollectionChanged;
                    foreach (Blocklist item in _blockLists)
                        item.PropertyChanged += Item_PropertyChanged;

                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        private void _blockLists_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Blocklist item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (Blocklist item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            NotifyPropertyChanged();
        }


        private Schedules _schedules;

        public Schedules Schedules
        {
            get
            {
                if (_schedules == null)
                {
                    _schedules = new Schedules();
                    //_schedules.PropertyChanged += General_PropertyChanged;
                }
                return _schedules;
            }
            set
            {
                if (value != _schedules)
                {
                    _schedules = value;
                    //_schedules.PropertyChanged += General_PropertyChanged;
                }
            }
        }

        public Todaysschedule _todaysSchedule;

        public Todaysschedule TodaysSchedule
        {
            get
            {
                if (_todaysSchedule == null)
                {
                    _todaysSchedule = new Todaysschedule();
                    //_todaysSchedule.PropertyChanged += General_PropertyChanged;
                }
                return _todaysSchedule;
            }
            set
            {
                if (value != _todaysSchedule)
                {
                    _todaysSchedule = value;
                    //_todaysSchedule.PropertyChanged += General_PropertyChanged;
                }
            }
        }

        //Propagate the User Preference changes to App Engine for App Engine to update settings json file
        private void General_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class Schedules
    {
        public Schedule schedule1 { get; set; } = new Schedule();
        public Schedule schedule2 { get; set; } = new Schedule();
        public Schedule schedule3 { get; set; } = new Schedule();
        public Schedule schedule4 { get; set; } = new Schedule();
        public Schedule schedule5 { get; set; } = new Schedule();
        public Schedulebreak scheduleBreak { get; set; } = new Schedulebreak();
    }

    public class Schedule
    {
        public string blockListName { get; set; } = string.Empty;
        public string startAt { get; set; } = string.Empty;
        public string endAt { get; set; } = string.Empty;
        public List<int> days { get; set; } = new List<int>();
        public bool isActive { get; set; }
    }

    public class Schedulebreak
    {
        public bool isActive { get; set; }
        public int breakDuration { get; set; } = 1;
        public int breakGap { get; set; } = 15;
    }

    public class Todaysschedule
    {
        public ScheduleToday schedule1 { get; set; } = new ScheduleToday();
        public ScheduleToday schedule2 { get; set; } = new ScheduleToday();
        public ScheduleToday schedule3 { get; set; } = new ScheduleToday();
        public ScheduleToday schedule4 { get; set; } = new ScheduleToday();
        public ScheduleToday schedule5 { get; set; } = new ScheduleToday();
    }

    public class ScheduleToday
    {
        public string blockListName { get; set; } = string.Empty;
        public string startAt { get; set; } = string.Empty;
        public string endAt { get; set; } = string.Empty;
        public bool isActive { get; set; }
    }

    

    public class Alsoblock
    {
        public object[] apps { get; set; }
        public object[] websites { get; set; }
    }

    public class Exceptions
    {
        public object[] apps { get; set; }
        public object[] websites { get; set; }
    }

    public class Blockcategory
    {
        public string name { get; set; }
        public bool block { get; set; }
    }
}