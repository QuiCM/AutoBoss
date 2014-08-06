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

            var invalidRegions = new List<string>();

            foreach (var arena in AutoBoss.config.BossArenas.Where(a => a.Value))
            {
                var region = TShock.Regions.GetRegionByName(arena.Key);
                if (region == null)
                {
                    invalidRegions.Add(arena.Key);
                    continue;
                }
                if (!AutoBoss.ActiveArenas.Contains(region)) AutoBoss.ActiveArenas.Add(region);
            }

            arenaCount = AutoBoss.ActiveArenas.Count;

            if (invalidRegions.Count == 0) return;
            Log.ConsoleError("Invalid regions encountered: " + string.Join(", ", invalidRegions));

            if (!console && receiver != null)
                receiver.SendErrorMessage("Invalid regions encountered: " + string.Join(", ", invalidRegions));

            AutoBoss.Timers = new BossTimer();
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
    }
}
