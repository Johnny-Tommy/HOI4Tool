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
using System.IO;

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

        /// <summary>
        /// Hier später noch eine elegantere Prüfung einbauen... :-/
        /// </summary>
        /// <param name="txtBox"></param>
        /// <returns></returns>
        private bool CheckNumericFields(TextBox txtBox)
        {
            if (Int32.TryParse(txtBox.Text, out int result))
            {
                txtBox.Background = Brushes.White;
                return true;
            }
            else
            {
                txtBox.Background = Brushes.Orange;
                return false;
            }
        }

        /// <summary>
        /// Hier später noch eine elegantere Prüfung einbauen... :-/
        /// </summary>
        /// <param name="txtBox"></param>
        /// <returns></returns>
        private bool CheckFileOrDirectory(TextBox txtBox)
        {
            string pfad;

            if(txtBox.Text.Contains('\\'))
            {
                if (Directory.Exists(txtBox.Text))
                {
                    txtBox.Background = Brushes.White;
                    return true;
                }
                else
                {
                    txtBox.Background = Brushes.Orange;
                    return false;
                }
            }
            else
            {
                if(txtBox.Text.Contains(".dds") || txtBox.Text.Contains(".txt"))
                {
                    pfad = txtPfad_ArmyIcon.Text;
                }
                else
                {
                    pfad = txtPfad_Interface.Text;
                }
                
                if (File.Exists(pfad + txtBox.Text))
                {
                    txtBox.Background = Brushes.White;
                    return true;
                }
                else
                {
                    txtBox.Background = Brushes.Orange;
                    return false;
                }
            }
        }

        private async void Default_SettingsSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool readyToSave = true;
            
            cmdSpeichern.IsEnabled = false;

#warning Hier, falls möglich, eine elegantere Prüfung einbauen
            // ********** Prüfungen ob Verzeichnisse und Dateien wirklich vorhanden sind und korrekte Integerwerte eingegeben worden sind **********
            readyToSave = CheckFileOrDirectory(txtPfad_ArmyIcon);
            readyToSave = CheckFileOrDirectory(txtFile_ArmyIconConfig);
            readyToSave = CheckFileOrDirectory(txtFile_ArmyIconGraphics);
            readyToSave = CheckFileOrDirectory(txtFile_ArmyIconGraphicsSelected);
            readyToSave = CheckFileOrDirectory(txtFile_ArmyGroupIconGraphics);
            readyToSave = CheckFileOrDirectory(txtFile_ArmyGroupIconGraphicsSelected);
            readyToSave = CheckFileOrDirectory(txtFile_ArmyNavyIconGraphics);
            readyToSave = CheckFileOrDirectory(txtFile_ArmyNavyIconGraphicsSelected);
            readyToSave = CheckFileOrDirectory(txtPfad_Interface);
            readyToSave = CheckFileOrDirectory(txtFile_GFX);
            readyToSave = CheckFileOrDirectory(txtPfad_Backup);
            readyToSave = CheckNumericFields(txtInsigniaGap);
            readyToSave = CheckNumericFields(txtInsigniaX);
            readyToSave = CheckNumericFields(txtInsigniaY);

            if (readyToSave)
            {
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
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("Ein oder mehrere Dateien / Verzeichnisse sind nicht vorhanden oder es wurden falsche Pixeldaten eingegeben. Bitte die Einstellungen überprüfen! Es wird nicht gespeichert.", 
                                "Speichern fehlgeschlagen", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Warning);
            }

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
