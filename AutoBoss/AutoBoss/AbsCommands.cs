using System.Linq;
using System.Collections.Generic;
using TShockAPI;

namespace AutoBoss
{
    public static class BossCommands
    {
        public static void BossCommand(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || args.Parameters.Count > 3)
            {
                args.Player.SendErrorMessage("Invalid syntax; Use /boss <[toggle][reload][config][list]");
                args.Player.SendErrorMessage("List options: [boss\\minions\\mobs]");
                return;
            }

            switch (args.Parameters[0])
            {
                case "toggle":
                {
                    AutoBoss.Tools.bossesToggled = !AutoBoss.Tools.bossesToggled;

                    if (AutoBoss.Tools.bossesToggled)
                    {
                        if (
                            !AutoBoss.config.BossArenas.Any(
                                p => TShock.Regions.Regions.Contains(TShock.Regions.GetRegionByName(p.Key)) && p.Value))
                        {
                            args.Player.SendErrorMessage("Error: Invalid regions encountered; Boss battles disabled");
                            return;
                        }

                        var day = AutoBoss.config.BossToggles["day"];
                        var night = AutoBoss.config.BossToggles["night"];
                        var special = AutoBoss.config.BossToggles["special"];
                        AutoBoss.Timers.StartBosses(day, night, special, true);
                    }

                    args.Player.SendSuccessMessage((AutoBoss.Tools.bossesToggled ? "Enabled" : "Disabled") +
                                                   " automatic boss spawnings");

                    break;
                }
                case "reload":
                {
                    if (args.Player == TSPlayer.Server)
                        AutoBoss.Tools.ReloadConfig(true);
                    else
                        AutoBoss.Tools.ReloadConfig(false, args.Player);
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
                                if (
                                    !PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player,
                                        out pageNumber))
                                    return;

                                PaginationTools.SendPage(args.Player, pageNumber,
                                    PaginationTools.BuildLinesFromTerms(AutoBoss.bossList.GetList()),
                                    new PaginationTools.Settings
                                    {
                                        HeaderFormat = "Boss List: {0}/{1}",
                                        FooterFormat = "Use /boss list bosses {0} for more bosses",
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
                                if (
                                    !PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player,
                                        out pageNumber))
                                    return;

                                PaginationTools.SendPage(args.Player, pageNumber,
                                    PaginationTools.BuildLinesFromTerms(AutoBoss.minionList.GetList()),
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
                                args.Player.SendErrorMessage("Usage: /boss list <[boss][mobs]>");
                                break;
                            }
                        }
                    }
                    else
                        args.Player.SendErrorMessage("Valid list options: <[boss][mobs]>");
                    break;
                }

                    #endregion

                case "config":
                {
                    int pageNumber;

                    #region Configuration viewing

                    var setupLines = new List<object>
                    {
                        "Auto-Start enabled: " + AutoBoss.config.AutoStartEnabled,
                        "Continuous boss spawning: " + AutoBoss.config.ContinuousBoss,
                        "Interval between boss messages: " + AutoBoss.config.MessageInterval,
                        "Text enabled on day-time bosses: " + AutoBoss.config.EnableDayTimerText,
                        "Text enabled on night-time bosses: " + AutoBoss.config.EnableDayTimerText,
                        "Text enabled on special event bosses: " + AutoBoss.config.EnableDayTimerText,
                        "Text announcement on minion spawn: " + AutoBoss.config.AnnounceMinions,
                        //"Minion types enabled: " + AutoBoss.config.MinionToggles.CheckMobTypes(),
                        //"Boss types enabled: " + AutoBoss.config.BossToggles.CheckBossTypes(),
                        "Minions spawn every " + AutoBoss.config.MinionsSpawnTimer[0] + " to " +
                        AutoBoss.config.MinionsSpawnTimer[1] + " seconds",
                        "Boss countdown timer enabled: " + AutoBoss.Tools.bossesToggled,
                        "There " +
                        (AutoBoss.Tools.arenaCount > 1 || AutoBoss.Tools.arenaCount == 0 ? "are" : "is") +
                        " currently " +
                        AutoBoss.Tools.arenaCount + " active " +
                        (AutoBoss.Tools.arenaCount > 1 || AutoBoss.Tools.arenaCount == 0
                            ? "arenas"
                            : "arena")
                    };

                    #endregion

                    if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
                        return;

                    PaginationTools.SendPage(args.Player, pageNumber,
                        PaginationTools.BuildLinesFromTerms(setupLines), new PaginationTools.Settings
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

    }

    
}