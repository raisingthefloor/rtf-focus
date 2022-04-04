using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Morphic.Focus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            
        }

        private EventWaitHandle eventWaitHandle;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                Task.Run(async () =>
                {
                    await AppEngine.Instance.StopTelemetrySessionAsync();
                });
            }
            catch { }

        }
    }
}
