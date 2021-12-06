using System;
using System.Collections.Generic;
using System.Text;

using Pdoxcl2Sharp;
// u.a. BitmapSource
using System.Windows.Media.Imaging;
// System.Drawing.Common muss als Nugetpaket installiert werden.
// Stellt u.a. die Bitmap Klasse zur Verfügung.
using System.Drawing;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HOI4Tool
{
    public class Icon : IParadoxRead, INotifyPropertyChanged
    {
        private Available _tempAvailable;
        public string Name { get; set; }
        public string ColorOverride { get; set; }
        /// <summary>
        /// Wenn ein "Available-Block" in der Icons-Sektion vorhnaden ist, legt der Parser ein
        /// neues Objekt in der List an. Es ist zur Zeit nicht gewährleistet, dass es wirklich nur
        /// einen Available-Block gibt bzw. mehr als ein Block in der Datei würde zu Fehlern führen.
        /// </summary>
#warning Hier noch eine sinnvolle Sicherung implantieren falls es mehr als einen Avail. Block geben sollte.
        public IList<Available> Availables { get; set; }
        public BitmapSource BmpSource { get; set; }
        public Bitmap Bmp { get; set; }

        public Icon()
        {
            this.Availables = new List<Available>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

       public Available TestSrc
        {
            get
            {
                if (this.Availables.Count > 0)
                {
                    return this.Availables[0];
                }
                else
                {
                    this._tempAvailable = new Available();
                    return this._tempAvailable;
                }
            }
        }

        public bool AvailablesIsEnableSrc
        {
            get
            {
                // Hier wir nicht auf null abgefragt, da es immer automatisch eine erzeugte Liste vom Typ Available gibt:
                // (siehe oben) Es wird im Icon Konstruktor immer eine neue Liste erzeugt, egal ob es einen Block in der
                // Datei gibt oder nicht. Interessant ist deshalb nur die Anzahl der Einträge - in der Regel keiner oder einer.
                return this.Availables.Count == 0 ? false : true;
            }
            set
            {
                if(this.Availables.Count == 0)
                {
                    this.Availables.Add(new Available());
                }
                else
                {
                    this.Availables.Clear();
                }

                // XAML-Bindingengine melden, dass sich die Daten geändert haben.
                OnPropertyChanged(); System.Windows.MessageBox.Show("OnPropertyChanged() Icons");
            }
        }

        public bool NameIsEnabledSrc
        {
            get
            {
                return this.Name == null ? false : true;
            }
            set
            {
                if(this.Name == null)
                {
                    this.Name = "";
                }
                else
                {
                    this.Name = null;
                }

                // XAML-Bindingengine melden, dass sich die Daten geändert haben.
                OnPropertyChanged();
            }
        }

        public bool ColorOverrideIsEnabledSrc
        {
            get
            {
                return this.ColorOverride == null ? false : true;
            }
            set
            {
                if(this.ColorOverride == null)
                {
                    this.ColorOverride = "no";
                }
                else
                {
                    this.ColorOverride = null;
                }

                // XAML-Bindingengine melden, dass sich die Daten geändert haben.
                OnPropertyChanged();
            }
        }

        public bool ColorOverrideSrc
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(this.ColorOverride))
                {
                    return this.ColorOverride == "no" ? false : true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                this.ColorOverride = value == true ? "yes" : "no";
            }
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "color_override": ColorOverride = parser.ReadString(); break;
                case "name": Name = parser.ReadString(); break;
                case "available": Availables.Add(parser.Parse(new Available())); break;
            }
        }
    }
}
