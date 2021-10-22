using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class Blocklist : BaseClass
    {
        #region Name
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
        #endregion

        #region Categories
        private ObservableCollection<Blockcategory> _blockcategories;

        public ObservableCollection<Blockcategory> Blockcategories
        {
            get
            {
                if (_blockcategories == null)
                {
                    _blockcategories = new ObservableCollection<Blockcategory>();
                    _blockcategories.CollectionChanged += _blockcategories_CollectionChanged;
                }
                return _blockcategories;
            }
            set
            {
                if (value != _blockcategories)
                {
                    _blockcategories = value;
                    _blockcategories.CollectionChanged += _blockcategories_CollectionChanged;
                    foreach (Blockcategory item in _blockcategories)
                        item.PropertyChanged += Item_PropertyChanged; ;

                }
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        private void _blockcategories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (AppsAndWebsites item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (AppsAndWebsites item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            NotifyPropertyChanged();
        }
        #endregion

        public Alsoblock alsoBlock { get; set; }
        public Exceptions exceptions { get; set; }
        public string breakBehavior { get; set; }
        public string penalty { get; set; }
        public int penaltyValue { get; set; }
    }

    public class Blockcategory : BaseClass, IEquatable<Blockcategory>
    {
        private bool _isActive;
        private string _name;

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

        public override bool Equals(object obj)
        {
            return Equals(obj as Blockcategory);
        }

        public bool Equals(Blockcategory other)
        {
            return other != null &&
                   IsActive == other.IsActive &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsActive, Name);
        }

        public static bool operator ==(Blockcategory left, Blockcategory right)
        {
            return EqualityComparer<Blockcategory>.Default.Equals(left, right);
        }

        public static bool operator !=(Blockcategory left, Blockcategory right)
        {
            return !(left == right);
        }
    }
}
