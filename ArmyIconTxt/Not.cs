using System;
using System.Collections.Generic;
using System.Text;

using Pdoxcl2Sharp;

namespace HOI4Tool
{
    public class Not : IParadoxRead
    {
        public IList<string> Tags { get; set; }

        public Not()
        {
            Tags = new List<string>();
        }

        public void TokenCallback(Pdoxcl2Sharp.ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "tag": Tags.Add(parser.ReadString()); break;
            }
        }
    }
}
