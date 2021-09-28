using Morphic.Focus.Models;
using Morphic.Focus.Screens;
using Morphic.Focus.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Morphic.Focus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        FocusMain focusMain = null;

        
        private bool _isFocusRunning = false;
        private Session? _currSession1 = null;
        private string _buttonText = "Focus";

        public MainWindow()
        {
            InitializeComponent();
            CheckIsFocusRunning();

            DataContext = this;
        }

        private void CheckIsFocusRunning()
        {
            IsFocusRunning = File.Exists(Common.MakeFilePath(Common.SESSION_FILE_NAME));
        }

        public Session? CurrSession1 { get => _currSession1; set => _currSession1 = value; }

        public bool IsFocusRunning
        {
            get
            {
                return _isFocusRunning;
            }
            set
            {
                _isFocusRunning = value;
                NotifyPropertyChanged("IsFocusRunning"); // method implemented below
            }
        }

        public string ButtonText
        {
            get
            {
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                NotifyPropertyChanged("ButtonText"); // method implemented below
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnMainFocus_Click(object sender, RoutedEventArgs e)
        {
            if (focusMain == null)
            {
                focusMain = new FocusMain();
                focusMain.SessionUpdate += FocusMain_SessionUpdate;
            }
            focusMain.Show();
        }

        private void FocusMain_SessionUpdate(Session? currSession1)
        {
            CurrSession1 = currSession1;
            IsFocusRunning = CurrSession1 != null;

            ButtonText = IsFocusRunning ? "Focus" + Environment.NewLine + CurrSession1.SessionDuration : "Focus";
        }


        //Property changed
        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
