using System.Collections.ObjectModel; // ObservableCollection

namespace HOI4Tool
{
    /// <summary>
    /// Stellt eine Zeile im Insignieneditor dar. Die Liste (hier Observable Collection) 
    /// muss man sich dann horizontal vorstellen.
    /// </summary>
    public class Row
    {
        public Row(int nummer)
        {
            this._No = nummer;
        }
        private int _No;
        public ObservableCollection<Icon> Icons { get; set; } = new ObservableCollection<Icon>();
        /// <summary>
        /// Gibt die Zeilennummer zurück der Position im Grid zurück. Eine korrekte
        /// Durchnummerierung muss von außen gesteuert werden d.h. über int-Wert des
        /// Konstruktors.
        /// </summary>
        public int No => _No;
    }
}
