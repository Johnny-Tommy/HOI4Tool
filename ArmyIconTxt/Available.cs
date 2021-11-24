using System;
using System.Collections.Generic;
using System.Text;

using Pdoxcl2Sharp;

namespace HOI4Tool
{
    public class Available : IParadoxRead
    {
        public string HasDlc { get; set; }
        public string Tag { get; set; }
        public string HasGovernment { get; set; }
        public IList<Not> NOT { get; set; }
        public List<string> HasDlcSrc { get; set; } = new List<string>() { "", "Man the Guns", "Waking the Tiger", "La Résistance" };
        public List<string> HasGovernmentSrc { get; set; } = new List<string>() { "", "democratic", "fascism", "communism", "neutrality" };
        public List<string> TagSrc { get; set; } = new List<string>() { "", "USA", "BUL", "FRA", "GER", "SOV", "ENG", "TUR", "GRE" };

        public Available()
        {
            NOT = new List<Not>();
        }

        /// <summary>
        /// Gibt die Tags zurück, die noch nicht verwendet werden. Als Pool wird die Eigenschaft TagSrc verwendet, in der
        /// alle verfügbaren Tags aufgelistet sind.
        /// </summary>
        public List<string> TagAvailableSrc
        {
            get
            {
                List<string> tags = new List<string>(this.TagSrc);
                tags.Remove("");

                if(this.NOT.Count > 0)
                {
                    foreach(string tag in this.NOT[0].Tags)
                    {
                        if(tags.Contains(tag))
                        {
                            tags.Remove(tag);
                        }
                    }
                }

                return tags;
            }
        }

        public List<string> NotSrc
        {
            get
            {
                if(this.NOT.Count > 0) 
                {
                    return (List<string>)this.NOT[0].Tags;
                }
                else
                {
                    return new List<string>();
                }
            }
        }

        public bool NotIsEnableSrc
        {
            get
            {
                return this.NOT.Count == 0 ? false : true;
            }
        }

        public bool HasGovernmentIsEnabledSrc
        {
            get
            {
                return this.HasGovernment == null ? false : true;
            }
        }

        public bool TagIsEnabledSrc
        {
            get
            {
                return this.Tag == null ? false : true;
            }
        }

        public bool HasDlcIsEnabledSrc
        {
            get
            {
                return this.HasDlc == null ? false : true;
            }
        }


        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "NOT": NOT.Add(parser.Parse(new Not())); break;
                case "has_dlc": HasDlc = parser.ReadString(); break;
                case "tag": Tag = parser.ReadString(); break;
                case "has_government": HasGovernment = parser.ReadString(); break;
            }
        }
    }
}
