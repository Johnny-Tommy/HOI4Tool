using System;
using System.Collections.Generic;
using System.Text;

using Pdoxcl2Sharp;
// u.a. BitmapSource
using System.Windows.Media.Imaging;
// System.Drawing.Common muss als Nugetpaket installiert werden.
// Stellt u.a. die Bitmap Klasse zur Verfügung.
using System.Drawing;

namespace HOI4Tool
{
    public class Icon : IParadoxRead
    {
        public string ColorOverride { get; set; }
        public string Name { get; set; }
        public IList<Available> Availables { get; set; }
        public BitmapSource BmpSource { get; set; }
        public Bitmap Bmp { get; set; }
        public Icon()
        {
            Availables = new List<Available>();
        }

        public bool AvailablesIsEnableSrc
        {
            get
            {
                return this.Availables.Count == 0 ? false : true;
            }
        }

        public bool NameIsEnabledSrc
        {
            get
            {
                return this.Name == null ? false : true;
            }
        }

        public bool ColorOverrideIsEnabledSrc
        {
            get
            {
                return this.ColorOverride == null ? false : true;
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
