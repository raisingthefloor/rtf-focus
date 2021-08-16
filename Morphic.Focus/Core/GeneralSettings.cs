using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Core
{
    class GeneralSettings
    {
        //Backbone. Will be modified for data-binding
        private bool _lockComputer;

        private bool _showCountDownForStop;

        private bool _showCountUpForStop;

        public bool ShowCountUpForStop
        {
            get { return _showCountUpForStop; }
            set { _showCountUpForStop = value; }
        }


        public bool ShowCountDownForStop
        {
            get { return _showCountDownForStop; }
            set { _showCountDownForStop = value; }
        }


        public bool LockComputer
        {
            get { return _lockComputer; }
            set { _lockComputer = value; }
        }

    }
}
