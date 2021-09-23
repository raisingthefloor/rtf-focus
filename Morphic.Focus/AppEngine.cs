using Morphic.Focus.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus
{
    public class AppEngine : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Singleton
        private static readonly AppEngine _instance = new AppEngine();
        public static AppEngine Instance { get { return _instance; } }

        private ObservableCollection<BlockList> blockLists = new ObservableCollection<BlockList>();
        public ObservableCollection<BlockList> BlockLists
        {
            get
            {
                return blockLists;
            }
            set
            {
                blockLists = value;
                NotifyPropertyChanged("BlockLists"); // method implemented below
            }
        }
    }
}
