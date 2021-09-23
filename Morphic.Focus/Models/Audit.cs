using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Models
{
    class Audit
    {
        //Backbone. Will be modified for data-binding
        private int _id;

        private string _userName;

        private string _actionType;

        private string _controlName;

        private string _screenName;

        private DateTime _dateTime;

        private int _focusID;

        public int FocusID
        {
            get { return _focusID; }
            set { _focusID = value; }
        }


        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        public string ScreenName
        {
            get { return _screenName; }
            set { _screenName = value; }
        }

        public string ControlName
        {
            get { return _controlName; }
            set { _controlName = value; }
        }


        public string ActionType
        {
            get { return _actionType; }
            set { _actionType = value; }
        }


        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
