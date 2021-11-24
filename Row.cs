using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.ObjectModel; // ObservableCollection

namespace HOI4Tool
{
    public class Row
    {
        public Row(int nummer)
        {
            this._No = nummer;
        }
        private int _No;
        public ObservableCollection<Icon> Icons { get; set; } = new ObservableCollection<Icon>();
        public int No => _No;
    }
}
