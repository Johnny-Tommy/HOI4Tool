using System;
using System.Collections.Generic;
using System.Text;

using Pdoxcl2Sharp; // Paradox Dateiparser
using System.Collections.ObjectModel; // ObservableCollection

namespace HOI4Tool
{
    // Army, ArmyGroup, Fleet...
    public class ParadoxType : IParadoxRead
    {
        private ObservableCollection<Row> _IconGrid = new ObservableCollection<Row>(); // nicht für den Parser
        private string _Grafikdateiname = ""; // nicht für den Parser
        private ParadoxCategory _ParadoxCategory = ParadoxCategory.Default; // nicht für den Parser
        public string Gfx { get; set; }
        public IList<Icon> Icons { get; set; }
        public string Grafikdateiname
        {
            get 
            {
                return _Grafikdateiname;
            } 
        }
        public ParadoxCategory ParadoxCategory
        {
            get
            {
                return this._ParadoxCategory;
            }
        }

        public ParadoxType(string filename, ParadoxCategory category)
        {
            this.Icons = new List<Icon>();
            this._Grafikdateiname = filename;
            this._ParadoxCategory = category;
        }

        public void TokenCallback(Pdoxcl2Sharp.ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "gfx": Gfx = parser.ReadString(); break;
                case "icon": Icons.Add(parser.Parse(new Icon())); break;                    
            }            
        }

        /// <summary>
        /// Bereitet ein logisches Grid vor welches aus Listen besteht die wiederum die Iconobjekte enthalten. 
        /// Die Iconobjekte beinhalten die ImageSources welche vom WPF DataGrid verwendet werden.
        /// </summary>
        public ObservableCollection<Row> IconGrid
        {
            get
            {
                this._IconGrid.Clear();
                int rowNo = 0;
                Row r = new Row(rowNo);
                int anzahlSpalten = 6;

                foreach (Icon icon in this.Icons)
                {
                    r.Icons.Add(icon);
                    if (r.Icons.Count == anzahlSpalten)
                    {
                        // aktuelle Zeile der "Gridliste" hinzufügen und Zeileninhalt löschen um eine Neue zu beginnen
                        this._IconGrid.Add(r);
                        rowNo++;
                        r = new Row(rowNo);
                    }
                }

                if (r.Icons.Count > 0) this._IconGrid.Add(r); // Den Rest auch noch hinzufügen

                return this._IconGrid;
            }
        }
    }
}
