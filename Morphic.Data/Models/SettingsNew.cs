
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Morphic.Data.Models
{
    public class UserPreferences : BaseClass
    {
        private General _general;
        public General General
        {
            get
            {
                if (_general == null)
                {
                    _general = new General();
                    _general.PropertyChanged += _general_PropertyChanged; ;
                }
                return _general;
            }
            set
            {
                if (value != _general)
                {
                    _general = value;
                    _general.PropertyChanged += _general_PropertyChanged;
                }
            }
        }

        //Propagate the User Preference changes to App Engine for App Engine to update settings json file
        private void _general_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        private ObservableCollection<Blocklist> _blockLists;

        public ObservableCollection<Blocklist> BlockLists
        {
            get
            {
                if (_blockLists == null)
                {
                    _blockLists = new ObservableCollection<Blocklist>();
                    _blockLists.CollectionChanged += _blockLists_CollectionChanged; ;
                }
                return _blockLists;
            }
            set
            {
                if (value != _blockLists)
                {
                    _blockLists = value;
                    _blockLists.CollectionChanged += _blockLists_CollectionChanged;
                    foreach (Blocklist item in _blockLists)
                        item.PropertyChanged += Item_PropertyChanged;

                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        private void _blockLists_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Blocklist item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (Blocklist item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            NotifyPropertyChanged();
        }


        private Schedules _schedules;

        public Schedules Schedules
        {
            get
            {
                if (_schedules == null)
                {
                    _schedules = new Schedules();
                    _schedules.PropertyChanged += _schedules_PropertyChanged; ;
                }
                return _schedules;
            }
            set
            {
                if (value != _schedules)
                {
                    _schedules = value;
                    _schedules.PropertyChanged += _schedules_PropertyChanged;
                }
            }
        }

        //Propagate the User Preference changes to App Engine for App Engine to update settings json file
        private void _schedules_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        public Todaysschedule _todaysSchedule;

        public Todaysschedule TodaysSchedule
        {
            get
            {
                if (_todaysSchedule == null)
                {
                    _todaysSchedule = new Todaysschedule();
                    _todaysSchedule.PropertyChanged += _todaysSchedule_PropertyChanged;
                }
                return _todaysSchedule;
            }
            set
            {
                if (value != _todaysSchedule)
                {
                    _todaysSchedule = value;
                    _todaysSchedule.PropertyChanged += _todaysSchedule_PropertyChanged;
                }
            }
        }

        private void _todaysSchedule_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }
    }
}