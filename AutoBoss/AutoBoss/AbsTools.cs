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

            foreach (var arena in AutoBoss.InactiveArenas.Where(a => !AutoBoss.config.BossArenas.ContainsKey(a)) )
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

        #region MultipleError
        public void SendMultipleErrors(bool console = false, TSPlayer receiver = null, List<string> errors = null)
        {
            if (errors == null) return;

            if (errors.Count > 1)
            {
                if (console)
                    TSPlayer.Server.SendErrorMessage("Multiple errors encountered: '{0}'", string.Join("', '", errors));
                else
                {
                    receiver.SendErrorMessage("Multiple errors found on reloading:");
                    receiver.SendErrorMessage("'{0}'", string.Join("', '", errors));
                }
            }
            else if (errors.Count == 1)
            {
                if (console)
                    TSPlayer.Server.SendErrorMessage("Error encountered: '{0}'", string.Join("", errors));
                else
                    receiver.SendErrorMessage("Error encountered on reloading: '{0}'", string.Join("", errors));
            }
              
        }
        #endregion

    }

    public static class CommandExtensions
    {
        /// <summary>
        /// Returns a list of npc names from a dictionary containing IDs and types
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetList(this Dictionary<int, int> dict)
        {
            var ret = new List<string>();
            var strings = new Dictionary<string, int>();

            foreach (var pair in dict.Where(pair => Main.npc[pair.Key].type == pair.Value && Main.npc[pair.Key].active))
            {
                if (!strings.ContainsKey(Main.npc[pair.Key].name))
                    strings.Add(Main.npc[pair.Key].name, 1);
                else
                    strings[Main.npc[pair.Key].name]++;
            }

            foreach (var pair in strings)
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
