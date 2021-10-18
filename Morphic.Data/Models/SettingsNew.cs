
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

    public Blocklist[] blockLists { get; set; }


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

public class General : INotifyPropertyChanged
{
    private bool _dontGive5MinWarning = false;
    private bool _showBreakCountdownTimer = true;
    private bool _blockScreen1stMinofBreak = false;
    private Temporarilyunblock _temporarilyUnblock;

    public bool dontGive5MinWarning
    {
        get
        {
            return _dontGive5MinWarning;
        }
        set
        {
            if (value != this._dontGive5MinWarning)
            {
                this._dontGive5MinWarning = value;
                NotifyPropertyChanged();
            }
        }
    }
    public bool showBreakCountdownTimer
    {
        get
        {
            return _showBreakCountdownTimer;
        }
        set
        {
            if (value != this._showBreakCountdownTimer)
            {
                this._showBreakCountdownTimer = value;
                NotifyPropertyChanged();
            }
        }
    }
    public bool blockScreen1stMinofBreak
    {
        get
        {
            return _blockScreen1stMinofBreak;
        }
        set
        {
            if (value != this._blockScreen1stMinofBreak)
            {
                this._blockScreen1stMinofBreak = value;
                NotifyPropertyChanged();
            }
        }
    }
    
    public Temporarilyunblock TemporarilyUnblock
    {
        get
        {
            if (_temporarilyUnblock == null)
            {
                _temporarilyUnblock = new Temporarilyunblock();
                
            }
            return _temporarilyUnblock;
        }
        set
        {
            if (value != _temporarilyUnblock)
            {
                _temporarilyUnblock = value;
                NotifyPropertyChanged();
            }
        }
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

public class Temporarilyunblock
{
    private ObservableCollection<AppsAndWebsites> _appsAndWebsites;
    

    public ObservableCollection<AppsAndWebsites> AppsAndWebsites
    {
        get
        {
            if (_appsAndWebsites == null)
            {
                _appsAndWebsites = new ObservableCollection<AppsAndWebsites>();
            }
            return _appsAndWebsites;
        }
        set
        {
            if (value != _appsAndWebsites)
            {
                _appsAndWebsites = value;
            }
        }
    }
}

public class AppsAndWebsites
{
    public bool isActive { get; set; }
    public string name { get; set; }
    public bool isApp { get; set; }
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

public class Blocklist
{
    public string name { get; set; }
    public Blockcategory[] blockCategories { get; set; }
    public Alsoblock alsoBlock { get; set; }
    public Exceptions exceptions { get; set; }
    public string breakBehavior { get; set; }
    public string penalty { get; set; }
    public int penaltyValue { get; set; }
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
