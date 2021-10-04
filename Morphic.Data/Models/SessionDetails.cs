using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    class SessionDetails
    {
        //Backbone. Will be modified for data-binding
        private int _id;

        private int _sessionID;

        private bool _isDnDON;

        private bool _isBreakON;

        private int _breakMins;

        private int _breakFreqMins { get; set; }

        private bool _isBlockListOn;

        private int _blockListID;

        public int BlockListID
        {
            get { return _blockListID; }
            set { _blockListID = value; }
        }


        public bool IsBlockListOn
        {
            get { return _isBlockListOn; }
            set { _isBlockListOn = value; }
        }


        public int BreakMins
        {
            get { return _breakMins; }
            set { _breakMins = value; }
        }

        public int BreakFreqMins
        {
            get { return _breakFreqMins; }
            set { _breakFreqMins = value; }
        }


        public bool IsBreakON
        {
            get { return _isBreakON; }
            set { _isBreakON = value; }
        }


        public bool IsDnDON
        {
            get { return _isDnDON; }
            set { _isDnDON = value; }
        }

        public int SessionID
        {
            get { return _sessionID; }
            set { _sessionID = value; }
        }


        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
