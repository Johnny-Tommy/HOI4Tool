using System;
using System.Collections.Generic;
using System.Text;

using Pdoxcl2Sharp;
using HOI4Tool.Properties;

namespace HOI4Tool
{
    public class ArmyIconsTxt : IParadoxRead, IParadoxWrite
    {
        public IList<ParadoxType> ParadoxTypes { get; set; }
        public ArmyIconsTxt()
        {
            ParadoxTypes = new List<ParadoxType>();
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "ï»¿army": // mit Byte Order Mark (BOM)
                case "army":
                    ParadoxTypes.Add(parser.Parse(new ParadoxType(Settings.Default.FileArmyIconGraphics, ParadoxCategory.Army)));
                    break;
                case "army_group": 
                    ParadoxTypes.Add(parser.Parse(new ParadoxType(Settings.Default.FileArmyGroupIconGraphics, ParadoxCategory.ArmyGroup))); 
                    break;
                case "fleet":
                    ParadoxTypes.Add(parser.Parse(new ParadoxType(Settings.Default.FileNavyIconGraphics, ParadoxCategory.Navy)));
                    break;
                case "task_force":
                    ParadoxTypes.Add(parser.Parse(new ParadoxType("", ParadoxCategory.Taskforce)));
                    break;
                case "naval_equipment_role":
                    ParadoxTypes.Add(parser.Parse(new ParadoxType("", ParadoxCategory.NavalEquipmentRole)));
                    break;
            }
        }

        public void Write(ParadoxStreamWriter writer)
        {
            writer.WriteComment($"Modified with HOI4-Tool ({DateTime.Now})");
            foreach (ParadoxType paradoxType in this.ParadoxTypes)
            {
                switch(paradoxType.ParadoxCategory)
                {
                    case ParadoxCategory.Army:
                        writer.WriteLine("army = {");
                        break;
                    case ParadoxCategory.ArmyGroup:
                        writer.WriteLine("army_group = {");
                        break;
                    case ParadoxCategory.Navy:
                        writer.WriteLine("fleet = {");
                        break;
                    case ParadoxCategory.Taskforce:
                        writer.WriteLine("task_force = {");
                        break;
                    case ParadoxCategory.NavalEquipmentRole:
                        writer.WriteLine("naval_equipment_role = {");
                        break;
                }
                if (!string.IsNullOrWhiteSpace(paradoxType.Gfx)) writer.WriteLine("gfx", paradoxType.Gfx, ValueWrite.LeadingTabs);
                writer.WriteLine("", ValueWrite.NewLine);
                if (paradoxType.Icons.Count > 0)
                {
                    foreach (Icon ico in paradoxType.Icons)
                    {
                        writer.WriteLine("icon = {", ValueWrite.LeadingTabs);
                        if (!string.IsNullOrEmpty(ico.ColorOverride)) writer.WriteLine("color_override", ico.ColorOverride, ValueWrite.LeadingTabs);
                        if (!string.IsNullOrWhiteSpace(ico.Name)) writer.WriteLine("name", ico.Name, ValueWrite.Quoted);
                        if (ico.Availables.Count > 0)
                        {
                            writer.WriteLine("available =", ValueWrite.LeadingTabs);
                            writer.WriteLine("{", ValueWrite.LeadingTabs);
                            foreach (Available avail in ico.Availables)
                            {
                                if (!string.IsNullOrWhiteSpace(avail.HasGovernment)) writer.WriteLine("has_government", avail.HasGovernment + " ", ValueWrite.LeadingTabs);
                                // Encoding.UTF8.GetString(Encoding.Default.GetBytes(avail.HasDlc))
                                if (!string.IsNullOrWhiteSpace(avail.HasDlc)) writer.WriteLine("has_dlc", avail.HasDlc, ValueWrite.Quoted);
                                if (!string.IsNullOrWhiteSpace(avail.Tag)) writer.WriteLine("tag", avail.Tag, ValueWrite.LeadingTabs);
                                if (avail.NOT.Count > 0)
                                {
#warning Sicherung einbauen, für den Fall, dass es mehere NOTs gibt...
                                    if(avail.NOT[0].Tags.Count > 0)
                                    {
                                        writer.WriteLine("NOT =", ValueWrite.LeadingTabs);
                                        writer.WriteLine("{", ValueWrite.LeadingTabs);
                                        foreach (Not not in avail.NOT)
                                        {
                                            foreach (string tag in not.Tags)
                                            {
                                                writer.WriteLine("tag", tag, ValueWrite.LeadingTabs);
                                            }
                                        }
                                        writer.WriteLine("}", ValueWrite.LeadingTabs);
                                    }
                                }
                            }
                            writer.WriteLine("}", ValueWrite.LeadingTabs);
                        }
                        writer.WriteLine("}", ValueWrite.LeadingTabs);
                    }
                }
                writer.WriteLine("}");
                writer.WriteLine("", ValueWrite.NewLine); 
            }
        }
    }
}
