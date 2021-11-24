using System;
using System.Collections.Generic;
using System.Text;

using Pdoxcl2Sharp;

namespace HOI4Tool
{
    public class SpriteTypes : IParadoxRead, IParadoxWrite
    {
        public IList<FrameAnimatedSpriteType> FrameAnimatedSpritetypes { get; set; }

        public SpriteTypes()
        {
            FrameAnimatedSpritetypes = new List<FrameAnimatedSpriteType>();
        }

        public void TokenCallback(Pdoxcl2Sharp.ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "spriteType":
                    FrameAnimatedSpritetypes.Add(parser.Parse(new SpriteType()));
                    break;
                case "frameAnimatedSpriteType":
                    FrameAnimatedSpritetypes.Add(parser.Parse(new FrameAnimatedSpriteType()));
                    break;
            }
        }

        public void Write(ParadoxStreamWriter writer)
        {
            writer.WriteComment($"Modified with HOI4-Tool ({DateTime.Now})");
            writer.WriteLine("spriteTypes = {");
            foreach (FrameAnimatedSpriteType frameAniSprTyp in this.FrameAnimatedSpritetypes)
            {
                if (frameAniSprTyp.GetType() == typeof(SpriteType))
                {
                    writer.WriteLine("spriteType = {", ValueWrite.LeadingTabs);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.Name)) writer.WriteLine("name", frameAniSprTyp.Name, ValueWrite.Quoted);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.Texturefile)) writer.WriteLine("texturefile", frameAniSprTyp.Texturefile, ValueWrite.Quoted);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.NoOfFrames)) writer.WriteLine("noOfFrames", frameAniSprTyp.NoOfFrames, ValueWrite.LeadingTabs);
                    writer.WriteLine("}", ValueWrite.LeadingTabs);
                    writer.WriteLine("", ValueWrite.NewLine);
                }
                else if (frameAniSprTyp.GetType() == typeof(FrameAnimatedSpriteType))
                {
                    writer.WriteLine("frameAnimatedSpriteType = {", ValueWrite.LeadingTabs);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.Name)) writer.WriteLine("name", frameAniSprTyp.Name);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.Texturefile)) writer.WriteLine("texturefile", frameAniSprTyp.Texturefile, ValueWrite.Quoted);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.NoOfFrames)) writer.WriteLine("noOfFrames", frameAniSprTyp.NoOfFrames, ValueWrite.LeadingTabs);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.AnimationRateFPS)) writer.WriteLine("animation_rate_fps", frameAniSprTyp.AnimationRateFPS, ValueWrite.LeadingTabs);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.Looping)) writer.WriteLine("looping", frameAniSprTyp.Looping, ValueWrite.LeadingTabs);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.PlayOnShow)) writer.WriteLine("play_on_show", frameAniSprTyp.PlayOnShow, ValueWrite.LeadingTabs);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.PauseOnLoop)) writer.WriteLine("pause_on_loop", frameAniSprTyp.PauseOnLoop, ValueWrite.LeadingTabs);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.AlwaysTransparent)) writer.WriteLine("alwaystransparent", frameAniSprTyp.AlwaysTransparent, ValueWrite.LeadingTabs);
                    if (!string.IsNullOrEmpty(frameAniSprTyp.EffectFile)) writer.WriteLine("effectFile", frameAniSprTyp.EffectFile, ValueWrite.Quoted);
                    writer.WriteLine("}", ValueWrite.LeadingTabs);
                    writer.WriteLine("", ValueWrite.NewLine);
                }
            }
            writer.WriteLine("}");
        }
    }
}
