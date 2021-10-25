using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class Blocklist : BaseClass
    {
        public Blocklist(){}

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
                foreach (Blockcategory item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (Blockcategory item in e.NewItems)
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

    public class Blockcategory : Category, IEquatable<Blockcategory>
    {
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

        public override bool Equals(object obj)
        {
            return Equals(obj as Blockcategory);
        }

        public bool Equals(Blockcategory other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Name == other.Name &&
                   EqualityComparer<ObservableCollection<string>>.Default.Equals(AppsAndWebsites, other.AppsAndWebsites) &&
                   IsActive == other.IsActive;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Name, AppsAndWebsites, IsActive);
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


    public class CategoryCollection : BaseClass
    {
        private ObservableCollection<Category> _categories = null;

        public ObservableCollection<Category> Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new ObservableCollection<Category>();
                    _categories.CollectionChanged += _categories_CollectionChanged; ;
                }
                return _categories;
            }
            set
            {
                if (value != _categories)
                {
                    _categories = value;
                    _categories.CollectionChanged += _categories_CollectionChanged;
                    foreach (Category item in _categories)
                        item.PropertyChanged += Item_PropertyChanged; ;

                }
            }
        }

        private void _categories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Category item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (Category item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            NotifyPropertyChanged();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }
    }
    public class Category : BaseClass, IEquatable<Category>
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

        private ObservableCollection<string> _appsAndWebsites;

        public ObservableCollection<string> AppsAndWebsites
        {
            get
            {
                if (_appsAndWebsites == null)
                {
                    _appsAndWebsites = new ObservableCollection<string>();
                    _appsAndWebsites.CollectionChanged += _appsAndWebsites_CollectionChanged;
                }
                return _appsAndWebsites;
            }
            set
            {
                if (value != _appsAndWebsites)
                {
                    _appsAndWebsites = value;
                    _appsAndWebsites.CollectionChanged += _appsAndWebsites_CollectionChanged;
                    //foreach (string item in _appsAndWebsites)
                    //    item.PropertyChanged += Item_PropertyChanged;

                }
            }
        }

        #region PropertyChanged
        private void _appsAndWebsites_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Category);
        }

        public bool Equals(Category other)
        {
            return other != null &&
                   Name == other.Name &&
                   EqualityComparer<ObservableCollection<string>>.Default.Equals(AppsAndWebsites, other.AppsAndWebsites);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, AppsAndWebsites);
        }

        public static bool operator ==(Category left, Category right)
        {
            return EqualityComparer<Category>.Default.Equals(left, right);
        }

        public static bool operator !=(Category left, Category right)
        {
            return !(left == right);
        }

        #endregion


    }
}
