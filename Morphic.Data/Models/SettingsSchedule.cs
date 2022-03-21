using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class Schedules : BaseClass
    {
        #region Schedule1
        private Schedule _schedule1;
        public Schedule Schedule1
        {
            get
            {
                if (_schedule1 == null)
                {
                    _schedule1 = new Schedule();
                    _schedule1.PropertyChanged += SchPropertyChanged;
                }
                return _schedule1;
            }
            set
            {
                if (value != this._schedule1)
                {
                    this._schedule1 = value;
                    _schedule1.PropertyChanged += SchPropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule2
        private Schedule _schedule2;
        public Schedule Schedule2
        {
            get
            {
                if (_schedule2 == null)
                {
                    _schedule2 = new Schedule();
                    _schedule2.PropertyChanged += SchPropertyChanged;
                }
                return _schedule2;
            }
            set
            {
                if (value != this._schedule2)
                {
                    this._schedule2 = value;
                    _schedule2.PropertyChanged += SchPropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule3
        private Schedule _schedule3;
        public Schedule Schedule3
        {
            get
            {
                if (_schedule3 == null)
                {
                    _schedule3 = new Schedule();
                    _schedule3.PropertyChanged += SchPropertyChanged;
                }
                return _schedule3;
            }
            set
            {
                if (value != this._schedule3)
                {
                    this._schedule3 = value;
                    _schedule3.PropertyChanged += SchPropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule4
        private Schedule _schedule4;
        public Schedule Schedule4
        {
            get
            {
                if (_schedule4 == null)
                {
                    _schedule4 = new Schedule();
                    _schedule4.PropertyChanged += SchPropertyChanged;
                }
                return _schedule4;
            }
            set
            {
                if (value != this._schedule4)
                {
                    this._schedule4 = value;
                    _schedule4.PropertyChanged += SchPropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule5
        private Schedule _schedule5;
        public Schedule Schedule5
        {
            get
            {
                if (_schedule5 == null)
                {
                    _schedule5 = new Schedule();
                    _schedule5.PropertyChanged += SchPropertyChanged;
                }
                return _schedule5;
            }
            set
            {
                if (value != this._schedule5)
                {
                    this._schedule5 = value;
                    _schedule5.PropertyChanged += SchPropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule Break
        private Schedulebreak _schedulebreak;
        public Schedulebreak Schedulebreak
        {
            get
            {
                if (_schedulebreak == null)
                {
                    _schedulebreak = new Schedulebreak();
                    _schedulebreak.PropertyChanged += SchPropertyChanged;
                }
                return _schedulebreak;
            }
            set
            {
                if (value != this._schedulebreak)
                {
                    this._schedulebreak = value;
                    _schedulebreak.PropertyChanged += SchPropertyChanged;
                }
            }
        }
        #endregion

        private void SchPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }
        
    }

    public class Schedule : BaseClass, IEquatable<Schedule>
    {
        private string _blockListName;

        public string BlockListName
        {
            get
            {
                return _blockListName;
            }
            set
            {
                if (value != this._blockListName)
                {
                    this._blockListName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime _startAt = DateTime.Today.AddHours(23).AddMinutes(59);

        public DateTime StartAt
        {
            get
            {
                return _startAt;
            }
            set
            {
                if (value != this._startAt)
                {
                    this._startAt = value;
                    EndAt = DateTime.Today.Add(EndAt.TimeOfDay);
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime _endAt = DateTime.Today.AddHours(23).AddMinutes(59);

        public DateTime EndAt
        {
            get
            {
                return _endAt;
            }
            set
            {
                if (value != this._endAt)
                {
                    this._endAt = value;
                    StartAt = DateTime.Today.Add(StartAt.TimeOfDay);
                    NotifyPropertyChanged();
                }
            }
        }

        

        private bool _isActive;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (value != this._isActive)
                {
                    this._isActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isActiveSunday;

        public bool IsActiveSunday
        {
            get
            {
                return _isActiveSunday;
            }
            set
            {
                if (value != this._isActiveSunday)
                {
                    this._isActiveSunday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isActiveMonday;

        public bool IsActiveMonday
        {
            get
            {
                return _isActiveMonday;
            }
            set
            {
                if (value != this._isActiveMonday)
                {
                    this._isActiveMonday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isActiveTuesday;

        public bool IsActiveTuesday
        {
            get
            {
                return _isActiveTuesday;
            }
            set
            {
                if (value != this._isActiveTuesday)
                {
                    this._isActiveTuesday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isActiveWednesday;

        public bool IsActiveWednesday
        {
            get
            {
                return _isActiveWednesday;
            }
            set
            {
                if (value != this._isActiveWednesday)
                {
                    this._isActiveWednesday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isActiveThursday;

        public bool IsActiveThursday
        {
            get
            {
                return _isActiveThursday;
            }
            set
            {
                if (value != this._isActiveThursday)
                {
                    this._isActiveThursday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isActiveFriday;

        public bool IsActiveFriday
        {
            get
            {
                return _isActiveFriday;
            }
            set
            {
                if (value != this._isActiveFriday)
                {
                    this._isActiveFriday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isActiveSaturday;

        public bool IsActiveSaturday
        {
            get
            {
                return _isActiveSaturday;
            }
            set
            {
                if (value != this._isActiveSaturday)
                {
                    this._isActiveSaturday = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #region IEquatable
        public override bool Equals(object obj)
        {
            return Equals(obj as Schedule);
        }

        public bool Equals(Schedule other)
        {
            return other != null &&
                   BlockListName == other.BlockListName &&
                   StartAt == other.StartAt &&
                   EndAt == other.EndAt &&
                   IsActive == other.IsActive &&
                   IsActiveSunday == other.IsActiveSunday &&
                   IsActiveMonday == other.IsActiveMonday &&
                   IsActiveTuesday == other.IsActiveTuesday &&
                   IsActiveWednesday == other.IsActiveWednesday &&
                   IsActiveThursday == other.IsActiveThursday &&
                   IsActiveFriday == other.IsActiveFriday &&
                   IsActiveSaturday == other.IsActiveSaturday;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(BlockListName);
            hash.Add(StartAt);
            hash.Add(EndAt);
            hash.Add(IsActive);
            hash.Add(IsActiveSunday);
            hash.Add(IsActiveMonday);
            hash.Add(IsActiveTuesday);
            hash.Add(IsActiveWednesday);
            hash.Add(IsActiveThursday);
            hash.Add(IsActiveFriday);
            hash.Add(IsActiveSaturday);
            return hash.ToHashCode();
        }

        public static bool operator ==(Schedule left, Schedule right)
        {
            return EqualityComparer<Schedule>.Default.Equals(left, right);
        }

        public static bool operator !=(Schedule left, Schedule right)
        {
            return !(left == right);
        }
        #endregion
    }

    public class Schedulebreak : BaseClass, IEquatable<Schedulebreak>
    {
        private bool _isActive;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (value != this._isActive)
                {
                    this._isActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _breakDuration = 1;
        public int BreakDuration
        {
            get
            {
                return _breakDuration;
            }
            set
            {
                if (value != this._breakDuration)
                {
                    this._breakDuration = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _breakGap = 15;
        public int BreakGap
        {
            get
            {
                return _breakGap;
            }
            set
            {
                if (value != this._breakGap)
                {
                    this._breakGap = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #region IEquatable
        public override bool Equals(object obj)
        {
            return Equals(obj as Schedulebreak);
        }

        public bool Equals(Schedulebreak other)
        {
            return other != null &&
                   IsActive == other.IsActive &&
                   BreakDuration == other.BreakDuration &&
                   BreakGap == other.BreakGap;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsActive, BreakDuration, BreakGap);
        }

        public static bool operator ==(Schedulebreak left, Schedulebreak right)
        {
            return EqualityComparer<Schedulebreak>.Default.Equals(left, right);
        }

        public static bool operator !=(Schedulebreak left, Schedulebreak right)
        {
            return !(left == right);
        }
        #endregion
    }

    public class Todaysschedule : BaseClass
    {
        #region Schedule1
        private Schedule _schedule1;
        public Schedule Schedule1
        {
            get
            {
                if (_schedule1 == null)
                {
                    _schedule1 = new Schedule();
                    _schedule1.PropertyChanged += PropertyChanged;
                }
                return _schedule1;
            }
            set
            {
                if (value != this._schedule1)
                {
                    this._schedule1 = value;
                    _schedule1.PropertyChanged += PropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule2
        private Schedule _schedule2;
        public Schedule Schedule2
        {
            get
            {
                if (_schedule2 == null)
                {
                    _schedule2 = new Schedule();
                    _schedule2.PropertyChanged += PropertyChanged;
                }
                return _schedule2;
            }
            set
            {
                if (value != this._schedule2)
                {
                    this._schedule2 = value;
                    _schedule2.PropertyChanged += PropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule3
        private Schedule _schedule3;
        public Schedule Schedule3
        {
            get
            {
                if (_schedule3 == null)
                {
                    _schedule3 = new Schedule();
                    _schedule3.PropertyChanged += PropertyChanged;
                }
                return _schedule3;
            }
            set
            {
                if (value != this._schedule3)
                {
                    this._schedule3 = value;
                    _schedule3.PropertyChanged += PropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule4
        private Schedule _schedule4;
        public Schedule Schedule4
        {
            get
            {
                if (_schedule4 == null)
                {
                    _schedule4 = new Schedule();
                    _schedule4.PropertyChanged += PropertyChanged;
                }
                return _schedule4;
            }
            set
            {
                if (value != this._schedule4)
                {
                    this._schedule4 = value;
                    _schedule4.PropertyChanged += PropertyChanged;
                }
            }
        }
        #endregion

        #region Schedule5
        private Schedule _schedule5;
        public Schedule Schedule5
        {
            get
            {
                if (_schedule5 == null)
                {
                    _schedule5 = new Schedule();
                    _schedule5.PropertyChanged += PropertyChanged;
                }
                return _schedule5;
            }
            set
            {
                if (value != this._schedule5)
                {
                    this._schedule5 = value;
                    _schedule5.PropertyChanged += PropertyChanged;
                }
            }
        }
        #endregion

        private DateTime _dateUpdated;

        public DateTime DateUpdated
        {
            get
            {
                return _dateUpdated;
            }
            set
            {
                if (value != this._dateUpdated)
                {
                    this._dateUpdated = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }
    }
}
