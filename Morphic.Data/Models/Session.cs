
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class Session : BaseClass
    {
        private bool _turnONDND;
        private bool _provideBreak;
        private bool _isBreakRunning = false;
        private int _breakDuration;
        private int _breakGap;
        private int _sessionDuration;
        private string _blockListName;
        private string _focusType;
        private DateTime _actualStartTime;
        private DateTime _actualEndTime;
        private DateTime _lastStartTime;
        private DateTime _lastBreakStartTime;
        private Schedule _schedule;

        public bool TurnONDND
        {
            get
            {
                return _turnONDND;
            }
            set
            {
                if (value != this._turnONDND)
                {
                    this._turnONDND = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool ProvideBreak
        {
            get
            {
                return _provideBreak;
            }
            set
            {
                if (value != this._provideBreak)
                {
                    this._provideBreak = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NextBreakTime");
                }
            }
        }
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
                    NotifyPropertyChanged("NextBreakTime");
                }
            }
        }
        public int SessionDuration
        {
            get
            {
                return _sessionDuration;
            }
            set
            {
                if (value != this._sessionDuration)
                {
                    this._sessionDuration = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NextBreakTime");
                    NotifyPropertyChanged("EndTime");
                }
            }
        }
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
        public string FocusType
        {
            get
            {
                return _focusType;
            }
            set
            {
                if (value != this._focusType)
                {
                    this._focusType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DateTime ActualStartTime
        {
            get
            {
                return _actualStartTime;
            }
            set
            {
                if (value != this._actualStartTime)
                {
                    this._actualStartTime = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NextBreakTime");
                    NotifyPropertyChanged("EndTime");
                }
            }
        }
        public DateTime ActualEndTime
        {
            get
            {
                return _actualEndTime;
            }
            set
            {
                if (value != this._actualEndTime)
                {
                    this._actualEndTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DateTime LastStartTime
        {
            get
            {
                return _lastStartTime;
            }
            set
            {
                if (value != this._lastStartTime)
                {
                    this._lastStartTime = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NextBreakTime");
                    NotifyPropertyChanged("RunForHRMM");
                }
            }
        }
        public DateTime LastBreakStartTime
        {
            get
            {
                return _lastBreakStartTime;
            }
            set
            {
                if (value != this._lastBreakStartTime)
                {
                    this._lastBreakStartTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DateTime NextBreakTime
        {
            get
            {
                //If it is Focus until Stop
                if (SessionDuration == 0)
                    return DateTime.MinValue;
                else
                {
                    if (ProvideBreak) //If breaks are required
                        return
                            new[] {
                                    LastStartTime.AddMinutes(BreakGap), //Next Break Time
                                    ActualStartTime.AddMinutes(SessionDuration) //Session End Time
                            }.Min(); //Minimum of Session End Time and next Break time
                    else
                        return ActualStartTime.AddMinutes(SessionDuration); //Session End Time
                }
            }
        }
        public String RunForHRMM
        {
            get
            {
                TimeSpan ts = DateTime.Now - ActualStartTime;

                if (ts.Hours > 0)
                    return ts.Hours + " hr " + ts.Minutes + " minutes ";
                else
                    return ts.Minutes + " minutes ";
            }
        }
        public String EndTime
        {
            get
            {
                if (Schedule != null)
                {
                    return Schedule.EndAt.ToString("hh:mm tt");
                }
                else
                {
                    //If it is focus till stop
                    if (SessionDuration == 0)
                        return string.Empty;

                    return ActualStartTime.AddMinutes(SessionDuration).ToString("hh:mm tt");
                }
            }
        }

        public bool IsBreakRunning
        {
            get
            {
                return _isBreakRunning;
            }
            set
            {
                if (value != this._isBreakRunning)
                {
                    this._isBreakRunning = value;
                    NotifyPropertyChanged();
                }
            }
        }

        

        #region Schedule
        public Schedule Schedule
        {
            get
            {
                //if (_schedule == null)
                //{
                //    _schedule = new Schedule();
                //    _schedule.PropertyChanged += PropertyChanged;
                //}
                return _schedule;
            }
            set
            {
                if (value != this._schedule)
                {
                    this._schedule = value;
                    _schedule.PropertyChanged += PropertyChanged;
                }
            }
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }
        #endregion

    }
}
