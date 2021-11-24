using System;
using System.Collections.Generic;
using System.Text;

using Pdoxcl2Sharp;

namespace HOI4Tool
{
    public class FrameAnimatedSpriteType : IParadoxRead
    {
        public string Name { get; set; }
        public string Texturefile { get; set; }
        public string NoOfFrames { get; set; }
        public string AnimationRateFPS { get; set; }
        public string Looping { get; set; }
        public string PlayOnShow { get; set; }
        public string PauseOnLoop { get; set; }
        public string AlwaysTransparent { get; set; }
        public string EffectFile { get; set; }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "name": Name = parser.ReadString(); break;
                case "texturefile": Texturefile = parser.ReadString(); break;
                case "noOfFrames": NoOfFrames = parser.ReadString(); break;
                case "animation_rate_fps": AnimationRateFPS = parser.ReadString(); break;
                case "looping": Looping = parser.ReadString(); break;
                case "play_on_show": PlayOnShow = parser.ReadString(); break;
                case "pause_on_loop": PauseOnLoop = parser.ReadString(); break;
                case "alwaystransparent": AlwaysTransparent = parser.ReadString(); break;
                case "effectFile": EffectFile = parser.ReadString(); break;
            }
        }
    }
}
