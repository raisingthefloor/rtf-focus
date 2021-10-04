using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    class Break
    {
        //Backbone. Will be modified for data-binding
        private int _id;

        private int _focusID;

        private DateTime _breakStartTime;

        private DateTime _breakEndTime;

        public DateTime BreakEndTime
        {
            get { return _breakEndTime; }
            set { _breakEndTime = value; }
        }


        public DateTime BreakStartTime
        {
            get { return _breakStartTime; }
            set { _breakStartTime = value; }
        }


        public int FocusID
        {
            get { return _focusID; }
            set { _focusID = value; }
        }


        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
