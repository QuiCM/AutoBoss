using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TShockAPI;

namespace AutoBoss
{
    public class AbsTools
    {
        public int arenaCount;

        public bool bossesToggled;

        public void ReloadConfig(bool console = false, TSPlayer receiver = null)
        {
            var configPath = Path.Combine(TShock.SavePath, "BossConfig.json");
            (AutoBoss.config = Config.Read(configPath)).Write(configPath);

            var invalids = new List<string>();

            foreach (var pair in AutoBoss.config.BossArenas)
            {
                if (pair.Value)
                {
                    if (TShock.Regions.GetRegionByName(pair.Key) != null)
                    {
                        if (!AutoBoss.ActiveArenas.Contains(TShock.Regions.GetRegionByName(pair.Key)))
                            AutoBoss.ActiveArenas.Add(TShock.Regions.GetRegionByName(pair.Key));
                        continue;
                    }
                    invalids.Add(pair.Key);
                }
                else if (!AutoBoss.InactiveArenas.Contains(pair.Key))
                    AutoBoss.InactiveArenas.Add(pair.Key);
            }

            foreach (var arena in AutoBoss.InactiveArenas.Where(a => !AutoBoss.config.BossArenas.ContainsKey(a)))
                AutoBoss.InactiveArenas.Remove(arena);

            if (invalids.Count > 0)
            {
                var str = string.Join(" ", invalids);

                if (console)
                    TSPlayer.Server.SendErrorMessage("[AutoBoss+] Invalid regions found: {0}", str);
                else if (receiver != null)
                    receiver.SendErrorMessage("[AutoBoss+] Invalid regions found: {0}", str);
            }

            arenaCount = AutoBoss.ActiveArenas.Count;
        }
    }

    public static class CommandExtensions
    {
        public static IEnumerable<string> GetList(this Dictionary<string, int> dict)
        {
            var ret = new List<string>();

            foreach (var pair in dict)
            {
                if (pair.Value > 1)
                {
                    ret.Add(pair.Key + " (" + pair.Value + ")");
                    continue;
                }
                ret.Add(pair.Key);
            }
            return ret;
        }

        public static void AddSafe(this Dictionary<int, int> dict, int key, int value, bool minions = false)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);

                if (minions) AutoBoss.minionCounts.Add(Main.npc[key].name, 1);
                else AutoBoss.bossCounts.Add(Main.npc[key].name, 1);
                return;
            }

            if (minions) AutoBoss.minionCounts[Main.npc[key].name]++;
            else AutoBoss.bossCounts[Main.npc[key].name]++;
        }
    }
}
