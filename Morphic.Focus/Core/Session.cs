
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Core
{
    class Session
    {
        //Backbone. Will be modified for data-binding
        private int _id;

        private DateTime _actualStartTime;

        private DateTime _actualEndTime;

        private string _userName;

        private int _scheduleID;

        private DateTime _lastStartTime;

        private DateTime _lastBreakStartTime;

        private string _focusType;

        public string FocusType
        {
            get { return _focusType; }
            set { _focusType = value; }
        }


        public DateTime LastBreakStartTime
        {
            get { return _lastBreakStartTime; }
            set { _lastBreakStartTime = value; }
        }


        public DateTime LastStartTime
        {
            get { return _lastStartTime; }
            set { _lastStartTime = value; }
        }
            

        public int ScheduleID
        {
            get { return _scheduleID; }
            set { _scheduleID = value; }
        }


        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }


        public DateTime ActualEndTime
        {
            get { return _actualEndTime; }
            set { _actualEndTime = value; }
        }


        public DateTime ActualStartTime
        {
            get { return _actualStartTime; }
            set { _actualStartTime = value; }
        }


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }


    }
}
