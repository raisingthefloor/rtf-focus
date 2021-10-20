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
        private Temporarilyunblock _temporarilyUnblock;

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

        public Temporarilyunblock TemporarilyUnblock
        {
            get
            {
                if (_temporarilyUnblock == null)
                {
                    _temporarilyUnblock = new Temporarilyunblock();
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

    public class Temporarilyunblock : BaseClass
    {
        private ObservableCollection<AppsAndWebsites> _appsAndWebsites;

        public ObservableCollection<AppsAndWebsites> AppsAndWebsites
        {
            get
            {
                if (_appsAndWebsites == null)
                {
                    _appsAndWebsites = new ObservableCollection<AppsAndWebsites>();
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
                    foreach (AppsAndWebsites item in _appsAndWebsites)
                        item.PropertyChanged += Item_PropertyChanged;

                }
            }
        }

        #region PropertyChanged
        private void _appsAndWebsites_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }
        #endregion
    }

    public class AppsAndWebsites : BaseClass, IEquatable<AppsAndWebsites>
    {
        private bool _isActive;
        private string _name;
        private bool _isApp;
        private string _path;

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
                   IsActive == other.IsActive &&
                   Name == other.Name &&
                   IsApp == other.IsApp &&
                   Path == other.Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsActive, Name, IsApp, Path);
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
