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

using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel; // ObservableCollection
using System.Threading; // Cancellation

namespace HOI4Tool
{
    /// <summary>
    /// Interaktionslogik für Navigator.xaml
    /// </summary>
    public partial class Navigator : Page
    {
        private ObservableCollection<DDSFile> ddsListe;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;
        private int anzahlDateien;

        public Navigator()
        {
            InitializeComponent();
            lblFileNotFound.Visibility = Visibility.Hidden;
            progressBar.Value = 0;
            cmdAbbruch.IsEnabled = false;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {            
            lblFileNotFound.Visibility = Visibility.Hidden;
            dataGridDDSFiles.Visibility = Visibility.Visible;
            // <UseWindowsForms>true</UseWindowsForms> muss zusätzlich in die Projektdatei hinzugefügt werden -.-
            // Wird verwendet um auf die FolderBrowserDialog Klasse zugreifen zu können.
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.Description = "Verzeichnis wählen...";
            folderDialog.ShowNewFolderButton = false;            

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                cmdAbbruch.IsEnabled = true;
                Action<DDSFile> action = UpdateDataGrid;
                IProgress<DDSFile> progress = new Progress<DDSFile>(action);
                lblPfad.Content = folderDialog.SelectedPath;
                string[] dateien = Directory.GetFiles(lblPfad.Content.ToString(), "*.dds");                
                ddsListe = new ObservableCollection<DDSFile>();
                dataGridDDSFiles.ItemsSource = ddsListe;
                string pfad = lblPfad.Content.ToString();
                progressBar.Maximum = dateien.Length;
                anzahlDateien = dateien.Length;

                try
                {
                    await Task.Run(() =>
                    {
                        foreach (string datei in dateien)
                        {
                            if (token.IsCancellationRequested) break;
                            string[] parts = datei.Split('\\');
                            DDSFile ddsFile = new DDSFile(pfad, parts[parts.Length - 1]);
                            // Inheritance Object->DispatcherObject->DependencyObject->Freezable->Animatable->ImageSource->BitmapSource->BitmapImage
                            // Da eine BitmapSource u.a. ein DependencyObject implementiert, muss man die BitmapSource im Multithread auf Freeze setzten,
                            // damit man es später im Hauptthread verwenden bzw. anzeigen darf.
                            // Nicht gefreezed wir folgende Fehlermeldung angezeigt: Error: Must create DependencySource on same Thread as the DependencyObject even by using Dispatcher
                            // sobald man der ItemsSource das Objekt und somit auch die BitmapSource übergibt.
                            if (ddsFile.BMPSource != null) ddsFile.BMPSource.Freeze();
                            progress.Report(ddsFile);
                        }
                    }, token);
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                finally
                {
                    cmdAbbruch.IsEnabled = false;
                }

                if (ddsListe.Count <= 0)
                {
                    lblFileNotFound.Content = "";
                    lblFileNotFound.Visibility = Visibility.Visible;
                    dataGridDDSFiles.Visibility = Visibility.Hidden;                    
                    string message = "keine DDS-Dateien gefunden";
                    Report report = new Report();
                    Action<Report> actionUpdateLabel = UpdateLabelNotFound;
                    IProgress<Report> progressUpdateLabel = new Progress<Report>(actionUpdateLabel);
                    await Task.Run(() => 
                    {
                        for(int i = 0; i <= message.Length; i++)
                        {
                            report.message = message.Substring(0, i);
                            progressUpdateLabel.Report(report);
                            System.Threading.Thread.Sleep(10);
                        }

                        for(int i = 0; i < 21; i++)
                        {
                            report.brush = i % 2 == 0 ? Brushes.Gray : Brushes.Red;
                            progressUpdateLabel.Report(report);
                            System.Threading.Thread.Sleep(50);
                        }                                                
                    });
                }
            }
        }
        
        private void UpdateLabelNotFound(Report report)
        {
            lblFileNotFound.Content = report.message;
            lblFileNotFound.Foreground = report.brush;
        }

        private void UpdateDataGrid(DDSFile ddsFile)
        {
            ddsListe.Add(ddsFile);
            progressBar.Value++;
            txtProgress.Content = progressBar.Value + " von " + anzahlDateien.ToString() + " Dateien geladen";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
        }

        private void lblPfad_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /*
            // Wenn SelectionUnit des DataGrids auf Cell eingestellt ist (SelectedItems ist dann immer NULL!!!)
            List<DDSFile> liste = new List<DDSFile>();
            
            foreach (var unknownData in dataGridDDSFiles.SelectedCells)
            {
                DataGridCellInfo info = new DataGridCellInfo();
                if (unknownData != null)
                {
                    info = unknownData;                    
                    if (info.IsValid)
                    {
                        if(info.Item.GetType() == typeof(DDSFile))
                        {
                            //liste.Add((DDSFile)info.Item);
                            DDSFile ddsfile = (DDSFile)info.Item;
                            //MessageBox.Show(ddsfile.Filename + " Argb: " + ddsfile.BMP.GetPixel(49, 0).ToArgb().ToString() + " ... " + ddsfile.BMP.GetPixel(49, 16).ToArgb().ToString());
                            for(int x = 0; x < ddsfile.BMP.Width; x++)
                            {
                                File.AppendAllText(@"d:\bmpdata.txt", " X: " + x.ToString() + " Y: 0 " + " Argb: " + ddsfile.BMP.GetPixel(x, 0).ToArgb().ToString() +
                                    " Alpha: " + ddsfile.BMP.GetPixel(x, 0).A + " R: " + ddsfile.BMP.GetPixel(x, 0).R + " G: " + ddsfile.BMP.GetPixel(x, 0).G +
                                    " B: " + ddsfile.BMP.GetPixel(x, 0).B + "\n") ;
                            }
                            for (int x = 0; x < ddsfile.BMP.Width; x++)
                            {
                                File.AppendAllText(@"d:\bmpdata.txt"," X: " + x.ToString() + " Y: 16 " + " Argb: " + ddsfile.BMP.GetPixel(x, 16).ToArgb().ToString() +
                                    " Alpha: " + ddsfile.BMP.GetPixel(x, 16).A + " R: " + ddsfile.BMP.GetPixel(x, 16).R + " G: " + ddsfile.BMP.GetPixel(x, 16).G +
                                    " B: " + ddsfile.BMP.GetPixel(x, 16).B + "\n");
                            }
                        }
                    }
                }                
            }
            */
        }
    }
}
