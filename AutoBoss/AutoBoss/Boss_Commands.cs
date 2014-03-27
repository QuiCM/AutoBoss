using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Terraria;
using TShockAPI;

namespace AutoBoss
{
    public class BossCommands
    {
        public static void BossCommand(CommandArgs args)
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
                            AutoBoss.Tools.BossesToggled = !AutoBoss.Tools.BossesToggled;

                            if (AutoBoss.Tools.BossesToggled)
                            {
                                if (!AutoBoss.Timer.bossTimer.Enabled)
                                    AutoBoss.Timer.bossTimer.Enabled = true;

                                foreach (KeyValuePair<string, bool> pair in AutoBoss.Tools.bossConfig.BossArenas)
                                    if (!TShock.Regions.Regions.Contains(TShock.Regions.GetRegionByName(pair.Key))
                                        && pair.Value == true)
                                    {
                                        args.Player.SendErrorMessage("Error: Region {0} is undefined", pair.Key);
                                        args.Player.SendErrorMessage("Boss battles disabled");
                                        AutoBoss.Tools.BossesToggled = false;
                                        AutoBoss.Timer.bossTimer.Enabled = false;
                                        return;
                                    }
                            }

                            args.Player.SendSuccessMessage((AutoBoss.Tools.BossesToggled ? "Enabled" : "Disabled") +
                                " automatic boss spawnings");

                            break;
                        }
                    case "reload":
                        {
                            if (args.Player == TSPlayer.Server)
                                AutoBoss.Tools.reloadConfig(true);
                            else
                                AutoBoss.Tools.reloadConfig(false, args.Player);
                            args.Player.SendSuccessMessage("Reloaded BossConfig.json");
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
                                            int pageNumber;
                                            if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out pageNumber))
                                                return;

                                            PaginationTools.SendPage(args.Player, pageNumber,
                                                PaginationTools.BuildLinesFromTerms(getDictionary(AutoBoss.Tools.bossList)),
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
                                            int pageNumber;
                                            if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out pageNumber))
                                                return;

                                            PaginationTools.SendPage(args.Player, pageNumber,
                                                PaginationTools.BuildLinesFromTerms(getDictionary(AutoBoss.Tools.minionList)),
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
                            int pageNumber;

                            #region Configuration viewing
                            List<object> SetupLines = new List<object>();
                            SetupLines.Add("Auto-Start enabled: " + AutoBoss.Tools.bossConfig.AutoStartEnabled);
                            SetupLines.Add("Continuous boss spawning: " + AutoBoss.Tools.bossConfig.ContinuousBoss);
                            SetupLines.Add("Interval between boss messages: " + AutoBoss.Tools.bossConfig.MessageInterval);
                            SetupLines.Add("Text enabled on day-time bosses: " + AutoBoss.Tools.bossConfig.EnableDayTimerText);
                            SetupLines.Add("Text enabled on night-time bosses: " + AutoBoss.Tools.bossConfig.EnableDayTimerText);
                            SetupLines.Add("Text enabled on special event bosses: " + AutoBoss.Tools.bossConfig.EnableDayTimerText);
                            SetupLines.Add("Text announcement on minion spawn: " + AutoBoss.Tools.bossConfig.AnnounceMinions);
                            SetupLines.Add("Minion types enabled: " + CheckMobTypes);
                            SetupLines.Add("Boss types enabled: " + CheckBossTypes);
                            SetupLines.Add("Minions spawn every " + AutoBoss.Tools.bossConfig.MinionsSpawnTimer[0] + " to " +
                                AutoBoss.Tools.bossConfig.MinionsSpawnTimer[1] + " seconds");
                            SetupLines.Add("Boss countdown timer enabled: " + AutoBoss.Tools.BossesToggled);
                            SetupLines.Add("There " + (AutoBoss.Tools.arenaCount > 1 || AutoBoss.Tools.arenaCount == 0 ? "are" : "is") + " currently " +
                                AutoBoss.Tools.arenaCount + " active " + (AutoBoss.Tools.arenaCount > 1 || AutoBoss.Tools.arenaCount == 0 ? "arenas" : "arena"));
                            #endregion

                            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
                                return;

                            else
                                PaginationTools.SendPage(args.Player, pageNumber,
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
                foreach (ToggleObj t in AutoBoss.Tools.bossConfig.MinionToggles)
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
                foreach (ToggleObj t in AutoBoss.Tools.bossConfig.MinionToggles)
                {
                    if (t.enabled && !t.type.StartsWith("please"))
                        enabled.Add(t.type);
                }
                return string.Format("{0}", string.Join(", ", enabled));
            }
        }

        /// <summary>
        /// Returns a list of npc names from a dictionary containing IDs and types
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
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