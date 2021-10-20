using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class Blocklist : BaseClass
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Blockcategory[] blockCategories { get; set; }
        public Alsoblock alsoBlock { get; set; }
        public Exceptions exceptions { get; set; }
        public string breakBehavior { get; set; }
        public string penalty { get; set; }
        public int penaltyValue { get; set; }
    }
}
