using System;
using System.Collections.Generic;
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

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for SettingsStudyInformation.xaml
    /// </summary>
    public partial class SettingsStudyInformation : UserControl
    {
        public SettingsStudyInformation()
        {
            InitializeComponent();

            var telemetryId = AppEngine.Instance.GetTelemetryClientId();
            this.ParticipantIdTextBox.Text = telemetryId ?? "Error: no participant ID assigned; please contact the study manager.";
        }

        private void ParticipantIdCopyButton_Click(object sender, RoutedEventArgs e)
        {
            var participantId = this.ParticipantIdTextBox.Text;
            Clipboard.Clear();
            Clipboard.SetText(participantId);
            MessageBox.Show("Your participant id has been copied to the clipboard.");
        }
    }
}
