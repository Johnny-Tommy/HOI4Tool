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
        private Available _tempAvailable = null;
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

        /// <summary>
        /// Für die Checkbox in der GroupBox "Verfügbarkeit"
        /// </summary>
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
                // Wenn es noch keinen Eintrag gibt, muss trotzdem ein Available-Objekt zurückgegeben
                // werden. Vor der Rückgabe wird geschaut, ob es ein Dummyobjekt (das tempAvailable) gibt.
                // Wenn dies der Fall ist, wird dieses der offiziellen Liste zugewiesen und zurückgegeben.
                if(this.Availables.Count == 0)
                {
                    if(this._tempAvailable == null)
                    {
                        this.Availables.Add(new Available());
                    }
                    else
                    {
                        this.Availables.Add(this._tempAvailable);
                    }
                }
                else
                {
                    this.Availables.Clear();
                }

                // XAML-Bindingengine melden, dass sich die Daten geändert haben.
                OnPropertyChanged(); System.Windows.MessageBox.Show("OnPropertyChanged() Icons");
            }
        }

        /// <summary>
        /// Diese Eigenschaft dient als Quelle für die Groupbox "Verfügbarkeit" des Insignieneditors.
        /// Der "Availables-Block" ist nicht immer in der Konfigdatei vorhanden,
        /// muss aber trotzdem dargestellt werden (dann leer), da es sonst zu
        /// Bindingfehlern kommt. Dies wird mit einem Dummyobjektt (_tempAvailable)
        /// gelöst. Sobald dann wirklich ein Available-Block in der Konfigdatei
        /// erstellt werden soll, wird dieses temporäre Objekt offiziell in der
        /// Liste als 1. Objekt der Availables aufgenommen.
        /// </summary>
        public Available AvailablesSrc
        {
            get
            {
                if (this.Availables.Count > 0)
                {
                    return this.Availables[0];
                }
                else
                {
                    if (this._tempAvailable == null) this._tempAvailable = new Available();
                    return this._tempAvailable;
                }
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
