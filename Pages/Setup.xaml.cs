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
        public FileManager fileManager = null;

        public Setup()
        {
            if (fileManager == null)
            {
                fileManager = new FileManager();
                fileManager.AddDirectory(Properties.Settings.Default.PathArmyIcons, "PathArmyIcons", "Army-Icons Verzeichnis:");
                fileManager.Directories[0].AddFile(Properties.Settings.Default.FileArmyGroupIconGraphics, "FileArmyGroupIconGraphics", "Group-Icons Grafikdatei:");
                fileManager.Directories[0].AddFile(Properties.Settings.Default.FileArmyGroupIconGraphicsSelected, "FileArmyGroupIconGraphicsSelected", "(selektiert)");
                fileManager.Directories[0].AddFile(Properties.Settings.Default.FileArmyIconGraphics, "FileArmyIconGraphics", "Army-Icon Grafiken:");
                fileManager.Directories[0].AddFile(Properties.Settings.Default.FileArmyIconGraphicsSelected, "FileArmyIconGraphicsSelected", "(selektiert)");
                fileManager.Directories[0].AddFile(Properties.Settings.Default.FileNavyIconGraphics, "FileNavyIconGraphics", "Navy-Icon Grafiken:");
                fileManager.Directories[0].AddFile(Properties.Settings.Default.FileNavyIconGraphicsSelected, "FileNavyIconGraphicsSelected", "(selektiert)");
                fileManager.Directories[0].AddFile(Properties.Settings.Default.FileArmyIconConfig, "FileArmyIconConfig", "File-Army-Icon Konfig:");
                fileManager.AddDirectory(Properties.Settings.Default.PathInterface, "PathInterface", "Interface Verzeichnis:");
                fileManager.Directories[1].AddFile(Properties.Settings.Default.FileTheatreSelector, "FileTheatreSelector", "File-Theatre Konfig:");
                fileManager.AddDirectory(Properties.Settings.Default.PathBackup, "PathBackup", "Backup Verzeichnis:");
            }

            InitializeComponent();
            gridPfadeUndDateien.DataContext = fileManager;
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

        private async void Default_SettingsSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cmdSpeichern.IsEnabled = false;
            fileManager.CheckAll();
#warning Hier noch ein INotification in der Filemanager Klasse implantieren?!
            gridPfadeUndDateien.DataContext = null;
            gridPfadeUndDateien.DataContext = fileManager;

            if (fileManager.IsCheckOk)
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
            fileManager.Save();
        }

        private void OpenDir_Click(object sender, RoutedEventArgs e)
        {
            // <UseWindowsForms>true</UseWindowsForms> muss zusätzlich in die Projektdatei hinzugefügt werden -.-
            // Wird verwendet um auf die FolderBrowserDialog Klasse zugreifen zu können.
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.Description = "Verzeichnis für Army-Icons wählen...";
            folderDialog.ShowNewFolderButton = false;
            Button button = (Button)sender;
            if(Int32.TryParse(button.Tag.ToString(), out int dirIndex))
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileManager.Directories[dirIndex].CompletePath = folderDialog.SelectedPath;
#warning Hier noch ein INotification in der Filemanager Klasse implantieren?!
                    gridPfadeUndDateien.DataContext = null;
                    gridPfadeUndDateien.DataContext = fileManager;
                }
            }
            else
            {
                MessageBox.Show("Verzeichnisindex konnte nicht ermittelt werden. (" + button.Tag.ToString() +")");
            }
        }
    }
}
