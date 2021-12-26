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

using System.Drawing;

namespace HOI4Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                
                mainFrame.Source = new Uri("Pages\\Start.xaml", UriKind.Relative);                 
                
                if (GetType().Assembly.GetName().Version != null)
                {
                    this.Title += " Testversion: " + GetType().Assembly.GetName().Version.ToString();
                }

                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Fehler in der Anwendung :-(", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private void UpdateBackground(Report report)
        //{            
        //    mainFrame.Background = report.brush;
        //}

        //private async void BGAnimation()
        //{
        //    Report report = new Report();
        //    report.brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)); 
        //    Action<Report> actionUpdateBackground = UpdateBackground;
        //    IProgress<Report> progressUpdateLabel = new Progress<Report>(actionUpdateBackground);
        //    await Task.Run(() =>
        //    {
        //        int blau = 0;
        //        for (int i = 0; i < 100; i++)
        //        {
        //            if (blau < 255)
        //            {
        //                blau += 5;
        //            }
        //            report.brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, (byte)blau));
        //            progressUpdateLabel.Report(report);
        //            System.Threading.Thread.Sleep(300);
        //        }
        //        progressUpdateLabel.Report(report);
        //    });
        //}

        private void MenuItemWelcome_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Pages\\Start.xaml", UriKind.Relative);
        }
        private void MenuItemViewer_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Pages\\Navigator.xaml", UriKind.Relative);
        }
        private void MenuItemEditor_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Pages\\Insignieneditor.xaml", UriKind.Relative);
        }
        private void MenuItemSetup_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Source = new Uri("Pages\\Setup.xaml", UriKind.Relative);
        }
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public enum ParadoxCategory
    {
        Default = 100,
        Army = 1,
        ArmyGroup = 2,    
        Navy = 3,
        Taskforce = 4,
        NavalEquipmentRole = 5
    }
}
