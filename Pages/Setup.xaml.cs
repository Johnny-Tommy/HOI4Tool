using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HOI4Tool.Properties;
using System.Configuration;
using System.Threading.Tasks;

namespace HOI4Tool
{
    /// <summary>
    /// Interaktionslogik für Setup.xaml
    /// </summary>
    public partial class Setup : Page
    {
        public Setup()
        {          
            InitializeComponent();
            gridPfadeUndDateien.DataContext = Settings.Default;
            gridInsignien.DataContext = Settings.Default;
            lblSpeicherort.Content = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            Settings.Default.SettingsSaving += Default_SettingsSaving;
        }

        private async void Default_SettingsSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBox.Show("Die Einstellungen wurden gespeichert.", "Gespeichert!", MessageBoxButton.OK, MessageBoxImage.Information);
            cmdSpeichern.IsEnabled = false;
                        
            Report report = new Report();
            report.message = "Gespeichert!";
            report.visible = Visibility.Visible;
            Action<Report> actionUpdateLabel = ShowStatus;
            IProgress<Report> progressUpdateLabel = new Progress<Report>(actionUpdateLabel);
            await Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    report.brush = i % 2 == 0 ? Brushes.White : Brushes.LightGreen;
                    progressUpdateLabel.Report(report);
                    System.Threading.Thread.Sleep(100);
                }

                report.visible = Visibility.Hidden;
                progressUpdateLabel.Report(report);
            });
            
            cmdSpeichern.IsEnabled = true;
        }

        private void ShowStatus(Report report)
        {
            lblGespeichert.Content = report.message;
            lblGespeichert.Visibility = report.visible;
            lblGespeichert.Foreground = report.brush;
        }

        private void cmdSpeichern_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
