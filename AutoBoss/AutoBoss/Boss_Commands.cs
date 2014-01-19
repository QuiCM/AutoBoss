using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Terraria;
using TShockAPI;

namespace Auto_Boss
{
    public class Boss_Commands
    {
        public static void Boss_Command(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || args.Parameters.Count > 3)
            {
                args.Player.SendErrorMessage("Invalid syntax; Use /boss <toggle\\reload\\config\\list>");
                args.Player.SendErrorMessage("List options: [boss\\minions\\mobs]");
                return;
            }
            else
                switch (args.Parameters[0])
                {
                    case "toggle":
                        {
                            Boss_Tools.Bosses_Toggled = !Boss_Tools.Bosses_Toggled;

                            if (Boss_Tools.Bosses_Toggled)
                            {
                                foreach (KeyValuePair<string, bool> pair in Boss_Tools.boss_Config.Boss_Arenas)
                                    if (!TShock.Regions.Regions.Contains(TShock.Regions.GetRegionByName(pair.Key))
                                        && pair.Value == true)
                                    {
                                        args.Player.SendErrorMessage("Error: Region {0} is undefined", pair.Key);
                                        args.Player.SendErrorMessage("Boss battles disabled");
                                        Boss_Tools.Bosses_Toggled = false;
                                        return;
                                    }

                                if (!Boss_Timer.boss_Timer.Enabled)
                                    Boss_Timer.boss_Timer.Enabled = true;
                            }

                            args.Player.SendSuccessMessage((Boss_Tools.Bosses_Toggled ? "Enabled" : "Disabled") +
                                " automatic boss spawnings");

                            break;
                        }
                    case "reload":
                        {
                            if (args.Player == TSPlayer.Server)
                                Boss_Tools.reloadConfig(true);
                            else
                                Boss_Tools.reloadConfig(false, args.Player);
                            args.Player.SendSuccessMessage("Reloaded Boss_Config.json");
                            break;
                        }

                    #region boss and mob lists
                    case "list":
                        {
                            if (args.Parameters.Count > 1)
                            {
                                switch (args.Parameters[1])
                                {
                                    case "bosses":
                                    case "boss":
                                        {
                                            int page_Number;
                                            if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out page_Number))
                                                return;

                                            PaginationTools.SendPage(args.Player, page_Number,
                                                PaginationTools.BuildLinesFromTerms(getDictionary(Boss_Tools.boss_List)),
                                                new PaginationTools.Settings
                                                {
                                                    HeaderFormat = "Boss List: {0}/{1}",
                                                    FooterFormat = "Use /boss list boss {0} for more bosses",
                                                    HeaderTextColor = Color.Lime,
                                                    FooterTextColor = Color.Lime,
                                                    LineTextColor = Color.White,
                                                    NothingToDisplayString = "There are no active bosses to be listed"
                                                });
                                            break;
                                        }
                                    case "mobs":
                                    case "minions":
                                        {
                                            int page_Number;
                                            if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out page_Number))
                                                return;

                                            PaginationTools.SendPage(args.Player, page_Number,
                                                PaginationTools.BuildLinesFromTerms(getDictionary(Boss_Tools.minion_List)),
                                                new PaginationTools.Settings
                                                {
                                                    HeaderFormat = "Minion List: {0}/{1}",
                                                    FooterFormat = "Use /boss list mobs {0} for more minions",
                                                    HeaderTextColor = Color.Lime,
                                                    FooterTextColor = Color.Lime,
                                                    LineTextColor = Color.White,
                                                    NothingToDisplayString = "There are no active minions to be listed"
                                                });
                                            break;
                                        }

                                    default:
                                        {
                                            args.Player.SendErrorMessage("Usage: /boss list [boss\\minions\\mobs]");
                                            break;
                                        }
                                }
                            }
                            else
                                args.Player.SendErrorMessage("Valid list options: [boss\\minions\\mobs]");
                            break;
                        }
                    #endregion

                    case "config":
                        {
                            int page_Number;

                            #region Configuration viewing
                            List<object> SetupLines = new List<object>();
                            SetupLines.Add("Auto-Start enabled: " + Boss_Tools.boss_Config.AutoStart_Enabled);
                            SetupLines.Add("Continuous boss spawning: " + Boss_Tools.boss_Config.Continuous_Boss);
                            SetupLines.Add("Interval between boss messages: " + Boss_Tools.boss_Config.Message_Interval);
                            SetupLines.Add("Text enabled on day-time bosses: " + Boss_Tools.boss_Config.Enable_DayTimer_Text);
                            SetupLines.Add("Text enabled on night-time bosses: " + Boss_Tools.boss_Config.Enable_DayTimer_Text);
                            SetupLines.Add("Text enabled on special event bosses: " + Boss_Tools.boss_Config.Enable_DayTimer_Text);
                            SetupLines.Add("Text announcement on minion spawn: " + Boss_Tools.boss_Config.Announce_Minions);
                            SetupLines.Add("Minion types enabled: " + CheckMobTypes);
                            SetupLines.Add("Boss types enabled: " + CheckBossTypes);
                            SetupLines.Add("Minions spawn every " + Boss_Tools.boss_Config.Minions_Spawn_Timer[0] + " to " +
                                Boss_Tools.boss_Config.Minions_Spawn_Timer[1] + " seconds");
                            SetupLines.Add("Boss countdown timer enabled: " + Boss_Tools.Bosses_Toggled);
                            SetupLines.Add("There " + (Boss_Tools.arena_Count > 1 || Boss_Tools.arena_Count == 0 ? "are" : "is") + " currently " +
                                Boss_Tools.arena_Count + " active " + (Boss_Tools.arena_Count > 1 || Boss_Tools.arena_Count == 0 ? "arenas" : "arena"));
                            #endregion

                            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out page_Number))
                                return;

                            else
                                PaginationTools.SendPage(args.Player, page_Number,
                                PaginationTools.BuildLinesFromTerms(SetupLines), new PaginationTools.Settings
                                {
                                    HeaderFormat = "AutoBoss+ setup: {0}/{1}",
                                    FooterFormat = "Use /boss config {0} for more information",
                                    HeaderTextColor = Color.Lime,
                                    FooterTextColor = Color.Lime,
                                    LineTextColor = Color.White,
                                    NothingToDisplayString = "There are no valid setup options to be displayed"
                                });
                            break;
                        }

                    default:
                        {
                            args.Player.SendErrorMessage("Invalid syntax; Use /boss <toggle\\reload\\config\\list>");
                            args.Player.SendErrorMessage("List options: [bosses\\mobs]");
                            break;
                        }
                }
        }

        public static string CheckMobTypes
        {
            get
            {
                List<string> enabled = new List<string>();
                foreach (Toggle_Obj t in Boss_Tools.boss_Config.Minion_Toggles)
                {
                    if (t.enabled && !t.type.StartsWith("please"))
                        enabled.Add(t.type);
                }
                return string.Format("{0}", string.Join(", ", enabled));
            }
        }

        public static string CheckBossTypes
        {
            get
            {
                List<string> enabled = new List<string>();
                foreach (Toggle_Obj t in Boss_Tools.boss_Config.Minion_Toggles)
                {
                    if (t.enabled && !t.type.StartsWith("please"))
                        enabled.Add(t.type);
                }
                return string.Format("{0}", string.Join(", ", enabled));
            }
        }

        public static List<string> getDictionary(Dictionary<int, int> dict)
        {
                            
            List<string> ret = new List<string>();
            Dictionary<string, int> strings = new Dictionary<string, int>();

            foreach (KeyValuePair<int, int> pair in dict)
            {
                if (Main.npc[pair.Key].type == pair.Value && Main.npc[pair.Key].active)
                {
                    if (!strings.ContainsKey(Main.npc[pair.Key].name))
                        strings.Add(Main.npc[pair.Key].name, 1);
                    else
                        strings[Main.npc[pair.Key].name]++;
                }
            }

            foreach (KeyValuePair<string, int> pair in strings)
            {
                if (pair.Value > 1)
                    ret.Add(pair.Key + " (" + pair.Value + ")");
                else
                    ret.Add(pair.Key);
            }

            return ret;
        }
    }
}