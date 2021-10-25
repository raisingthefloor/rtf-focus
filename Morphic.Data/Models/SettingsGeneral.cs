using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class General : BaseClass
    {
        private bool _dontGive5MinWarning = false;
        private bool _showBreakCountdownTimer = true;
        private bool _blockScreen1stMinofBreak = false;
        private CollAppsAndWebsites _temporarilyUnblock;

        public bool dontGive5MinWarning
        {
            get
            {
                return _dontGive5MinWarning;
            }
            set
            {
                if (value != this._dontGive5MinWarning)
                {
                    this._dontGive5MinWarning = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool showBreakCountdownTimer
        {
            get
            {
                return _showBreakCountdownTimer;
            }
            set
            {
                if (value != this._showBreakCountdownTimer)
                {
                    this._showBreakCountdownTimer = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool blockScreen1stMinofBreak
        {
            get
            {
                return _blockScreen1stMinofBreak;
            }
            set
            {
                if (value != this._blockScreen1stMinofBreak)
                {
                    this._blockScreen1stMinofBreak = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public CollAppsAndWebsites TemporarilyUnblock
        {
            get
            {
                if (_temporarilyUnblock == null)
                {
                    _temporarilyUnblock = new CollAppsAndWebsites();
                    _temporarilyUnblock.PropertyChanged += _temporarilyUnblock_PropertyChanged;
                }
                return _temporarilyUnblock;
            }
            set
            {
                if (value != _temporarilyUnblock)
                {
                    _temporarilyUnblock = value;
                    _temporarilyUnblock.PropertyChanged += _temporarilyUnblock_PropertyChanged;
                }
            }
        }

        #region PropertyChanged
        private void _temporarilyUnblock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }
        #endregion
    }

    public class CollAppsAndWebsites : BaseClass, IEquatable<CollAppsAndWebsites>
    {
        private ObservableCollection<ActiveAppsAndWebsites> _activeAppsAndWebsites;

        public ObservableCollection<ActiveAppsAndWebsites> ActiveAppsAndWebsites
        {
            get
            {
                if (_activeAppsAndWebsites == null)
                {
                    _activeAppsAndWebsites = new ObservableCollection<ActiveAppsAndWebsites>();
                    _activeAppsAndWebsites.CollectionChanged += _appsAndWebsites_CollectionChanged;
                }
                return _activeAppsAndWebsites;
            }
            set
            {
                if (value != _activeAppsAndWebsites)
                {
                    _activeAppsAndWebsites = value;
                    _activeAppsAndWebsites.CollectionChanged += _appsAndWebsites_CollectionChanged;
                    foreach (AppsAndWebsites item in _activeAppsAndWebsites)
                        item.PropertyChanged += Item_PropertyChanged;

                }
            }
        }

        #region PropertyChanged
        private void _appsAndWebsites_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ActiveAppsAndWebsites item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (ActiveAppsAndWebsites item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            NotifyPropertyChanged();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        #endregion

        #region IEquatable
        public override bool Equals(object obj)
        {
            return Equals(obj as CollAppsAndWebsites);
        }

        public bool Equals(CollAppsAndWebsites other)
        {
            return other != null &&
                   EqualityComparer<ObservableCollection<ActiveAppsAndWebsites>>.Default.Equals(ActiveAppsAndWebsites, other.ActiveAppsAndWebsites);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ActiveAppsAndWebsites);
        }

        public static bool operator ==(CollAppsAndWebsites left, CollAppsAndWebsites right)
        {
            return EqualityComparer<CollAppsAndWebsites>.Default.Equals(left, right);
        }

        public static bool operator !=(CollAppsAndWebsites left, CollAppsAndWebsites right)
        {
            return !(left == right);
        }


        #endregion
    }

    public class ActiveAppsAndWebsites : AppsAndWebsites
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
    }

    public class AppsAndWebsites : BaseClass, IEquatable<AppsAndWebsites>
    {
        
        private string _name;
        private bool _isApp;
        private string _path;

        

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

        public bool IsApp
        {
            get
            {
                return _isApp;
            }
            set
            {
                if (value != this._isApp)
                {
                    this._isApp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (value != this._path)
                {
                    this._path = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AppsAndWebsites);
        }

        public bool Equals(AppsAndWebsites other)
        {
            return other != null &&
                   Name == other.Name &&
                   IsApp == other.IsApp &&
                   Path == other.Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, IsApp, Path);
        }

        public static bool operator ==(AppsAndWebsites left, AppsAndWebsites right)
        {
            return EqualityComparer<AppsAndWebsites>.Default.Equals(left, right);
        }

        public static bool operator !=(AppsAndWebsites left, AppsAndWebsites right)
        {
            return !(left == right);
        }
    }
}
