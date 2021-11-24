using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HOI4Tool
{
    [Obsolete("Diese Klasse wird nicht benötigt, da Settings bereits alles implementiert was man braucht.")]
    public class SetupData : INotifyPropertyChanged
    {
        private string _PathArmyIcons;
        public string PathArmyIcons
        { 
            get
            {
                return _PathArmyIcons;
            } 
            set
            {
                _PathArmyIcons = value;
                OnPropertyChanged();
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
