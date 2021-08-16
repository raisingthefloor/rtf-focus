using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Core
{
    class OverrideList
    {
        //Backbone. Will be modified for data-binding
        private int _id;

        private string name;

        private string type;

        private DateTime _createdOn;

        private DateTime _deletedOn;

        private string _createdBy;

        private string _deletedBy;

        public string DeletedBy
        {
            get { return _deletedBy; }
            set { _deletedBy = value; }
        }


        public string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }


        public DateTime DeletedOn
        {
            get { return _deletedOn; }
            set { _deletedOn = value; }
        }

        public DateTime CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; }
        }


        public string Type
        {
            get { return type; }
            set { type = value; }
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
