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

namespace HOI4Tool
{
    /// <summary>
    /// Interaktionslogik für Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            try
            {
                InitializeComponent();
                gridStart.DataContext = (CustomAssemblyInfo)Application.Current.Properties["Info"];
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
