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

        #region Schedule Break
        private Schedulebreak _schedulebreak;
        public Schedulebreak Schedulebreak
        {
            get
            {
                if (_schedulebreak == null)
                {
                    _schedulebreak = new Schedulebreak();
                    _schedulebreak.PropertyChanged += PropertyChanged;
                }
                return _schedulebreak;
            }
            set
            {
                if (value != this._schedulebreak)
                {
                    this._schedulebreak = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private DateTime _startAt;

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
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime _endAt;

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
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<int> _days;

        public ObservableCollection<int> Days
        {
            get
            {
                if (_days == null)
                {
                    _days = new ObservableCollection<int>();
                    _days.CollectionChanged += _days_CollectionChanged;
                }
                return _days;
            }
            set
            {
                if (value != _days)
                {
                    _days = value;
                    _days.CollectionChanged += _days_CollectionChanged;
                    //foreach (string item in _appsAndWebsites)
                    //    item.PropertyChanged += Item_PropertyChanged;

                }
            }
        }

        #region PropertyChanged
        private void _days_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.OldItems != null)
            //{
            //    foreach (AppsAndWebsites item in e.OldItems)
            //        item.PropertyChanged -= Item_PropertyChanged;
            //}
            //if (e.NewItems != null)
            //{
            //    foreach (AppsAndWebsites item in e.NewItems)
            //        item.PropertyChanged += Item_PropertyChanged;
            //}

            NotifyPropertyChanged();
        }

        

        #endregion

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

        #region IEquatable
        public static bool operator ==(Schedule left, Schedule right)
        {
            return EqualityComparer<Schedule>.Default.Equals(left, right);
        }

        public static bool operator !=(Schedule left, Schedule right)
        {
            return !(left == right);
        }

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
                   EqualityComparer<ObservableCollection<int>>.Default.Equals(Days, other.Days) &&
                   IsActive == other.IsActive;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BlockListName, StartAt, EndAt, Days, IsActive);
        }
        #endregion
    }

    public class Schedulebreak : BaseClass
    { 
        public bool isActive { get; set; }
        public int breakDuration { get; set; } = 1;
        public int breakGap { get; set; } = 15;
    }
}
