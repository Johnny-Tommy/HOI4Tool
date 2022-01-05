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
using System.Reflection;

//[assembly: AssemblyProductAttribute("HOI4-Tool")]
//// Das ist die Programmversion (auch unter Einstellungen->Details der EXE-Datei zu sehen)
//// Dies ist noch eine Testversion. Deshalb fangen wir hier mit 0.1 an :-)
//[assembly: AssemblyFileVersionAttribute("0.2.0.0")]
//// Die Version wird nur bei Assemblys mit starkem Namen überprüft.
//// Diese ist dann relevant für die Assemblies die diese nutzen!
//[assembly: AssemblyVersionAttribute("1.0.*")]
//[assembly: AssemblyDescription("A little modding tool for the nice game Hearts of Iron IV from Paradox Interactive.")]
//[assembly: AssemblyInformationalVersion("Testversion")]
//[assembly: AssemblyCopyright("Johannes Thom")]

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
                CustomAssemblyInfo info = new CustomAssemblyInfo();
                Application.Current.Properties["Info"] = info;
                InitializeComponent();
                Hauptfenster.DataContext = info;
                mainFrame.DataContext = (CustomAssemblyInfo)Application.Current.Properties["Info"];
                mainFrame.Source = new Uri("Pages\\Start.xaml", UriKind.Relative);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Fehler in der Anwendung :-(", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
