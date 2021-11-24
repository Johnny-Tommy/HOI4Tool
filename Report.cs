using System.Windows.Media;
using System.Windows;

namespace HOI4Tool
{
    internal class Report
    {   
        internal string message { get; set; } = "";
        internal bool enabled { get; set; } = false;
        internal Visibility visible { get; set; } = Visibility.Visible;
        internal Brush brush { get; set; } = Brushes.Gray;
        internal DDSFile ddsFile { get; set; }
    }
}
