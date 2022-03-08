using Morphic.Data.Services;
using Morphic.Focus.Screens;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Morphic.Focus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Members and Constructor

        private string _buttonText = "Focus";
        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }

        public MainWindow()
        {
            try
            {
                //App Engine
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    _engine = AppEngine.Instance;
                }

                InitializeComponent();

                DataContext = this;

                //TODO - Review
                //Engine.FocusMain.SessionUpdate += FocusMain_SessionUpdate;
                //Engine.FocusStatus.SessionUpdate += FocusMain_SessionUpdate;

                Engine.PropertyChanged += Engine_PropertyChanged;
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region Properties

        public string ButtonText
        {
            get
            {
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Events
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch (Exception ex)
                {
                    LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
                }
            }
        }

        private void btnMainFocus_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
                Engine.ShowFocusWindow();
            }
            catch (Exception ex)
            {

                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void btnMainFocus_Click(object sender, RoutedEventArgs e)
        {
            //If Focus Session is running then show Active Sessions Window, else show Main Menu Window
            Engine.ShowFocusWindow();

            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                LoggingService.WriteAppLog(ex.Message + ex.StackTrace);
            }
        }

        private void Engine_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();

            //if (e.PropertyName == "IsFocusRunning" || e.PropertyName == "Session1" || e.PropertyName == "Session2")
            //    FocusMain_SessionUpdate();

            if (e.PropertyName == "TimeTillNextBreak")
                UpdateButtonText(true);
            else if (e.PropertyName == "TimeTillNextBreakEnds")
                UpdateButtonText(false);
        }

        private void UpdateButtonText(bool isFocus)
        {
            if (Engine.IsFocusRunning)
            {
                string timerText = isFocus ? Engine.TimeTillNextBreakHHMM : Engine.TimeTillNextBreakEndsHHMM;
                string focusBreakText = isFocus ? "Focus" : "Break";

                if (Engine.IsFocusTillStop)
                {
                    ButtonText = "Focus till Stop";
                }
                else
                {
                    if (Engine.UserPreferences.General.showBreakCountdownTimer)
                        ButtonText = focusBreakText + Environment.NewLine + timerText;
                    else
                        ButtonText = focusBreakText;
                }
            }
            else
            {
                ButtonText = "Focus";
            }
        }
        #endregion

        #region INotifyPropertyChanged implement
        public event PropertyChangedEventHandler? PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Do not allow the window to close if focus is running
            if (Engine.IsFocusRunning)
                e.Cancel = true; 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            HwndSource.FromHwnd(WindowHandle)?.AddHook(new HwndSourceHook(HandleMessages));
        }

        public static IntPtr WindowHandle { get; private set; }

        internal static void HandleParameter(string[] args)
        {
            // Do stuff with the args
            if (Application.Current?.MainWindow is MainWindow mainWindow)
                if (args.Count() > 0)
                    new BlockedAppModal(string.Join("\r\n", args)).ShowDialog();
                    //mainWindow.ButtonText = string.Join("\r\n", args);
                //else
                //    mainWindow.ButtonText = string.Join("\r\n", "No args");
                
        }

        private static IntPtr HandleMessages
        (IntPtr handle, int message, IntPtr wParameter, IntPtr lParameter, ref Boolean handled)
        {
            var data = UnsafeNative.GetMessage(message, lParameter);

            if (data != null)
            {
                if (Application.Current.MainWindow == null)
                    return IntPtr.Zero;

                if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;

                UnsafeNative.SetForegroundWindow(new WindowInteropHelper
                                                (Application.Current.MainWindow).Handle);

                var args = data.Split(' ');
                HandleParameter(args);
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}
