using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Generic;

using Terraria;
using TerrariaApi;
using TerrariaApi.Server;

using TShockAPI;
using TShockAPI.DB;

namespace AutoBoss
{
    public class BossTools
    {
        private BossTools() { }

        private static volatile BossTools instance = null;

        public static BossTools Instance
        {
            get
            {
                if (instance == null)
                    instance = new BossTools();

                return instance;
            }
        }


        public BossConfig bossConfig { get; set; }
        public string configPath { get { return Path.Combine(TShock.SavePath, "BossConfig.json"); } }

        public string invalidRegions = string.Empty;

        public Dictionary<int, int> bossList = new Dictionary<int, int>();
        public Dictionary<int, int> minionList = new Dictionary<int, int>();

        public List<Region> ActiveArenas = new List<Region>();
        public List<string> InactiveArenas = new List<string>();
        public int arenaCount = 0;

        public bool BossesToggled = false;

        #region SetupConfig
       /* public static void SetupConfig()
        {
            bool failed = false;

            List<string> logExceptions = new List<string>();

            foreach (KeyValuePair<string, bool> pair in bossConfig.BossArenas)
            {
                if (pair.Value == true)
                {
                    if (TShock.Regions.GetRegionByName(pair.Key) != null)
                    {
                        if (!ActiveArenas.Contains(TShock.Regions.GetRegionByName(pair.Key)))
                            ActiveArenas.Add(TShock.Regions.GetRegionByName(pair.Key));

                        failed = false;
                    }

                    else
                    {
                        invalidRegions += (invalidRegions.Length > 0 ? ", " : "") + pair.Key;

                        logExceptions.Add("Invalid Regions: " + invalidRegions);

                        invalidRegions = string.Empty;

                        failed = true;
                    }
                }
            }

            arenaCount = ActiveArenas.Count;

            if (arenaCount > 0)
                BossesToggled = bossConfig.AutoStartEnabled;

            if (arenaCount == 0)
                logExceptions.Add("No arenas defined");

            if (!failed)
                Log.ConsoleInfo("[AutoBoss+] Initialized successfully");
            else
                SendMultipleErrors(true, null, logExceptions);
            
        }*/
        #endregion

        public void reloadConfig(bool console = false, TSPlayer receiver = null)
        {
            (bossConfig = BossConfig.Read(configPath)).Write(configPath);

            foreach (KeyValuePair<string, bool> pair in bossConfig.BossArenas)
            {
                if (pair.Value)
                {
                    if (TShock.Regions.GetRegionByName(pair.Key) != null)
                    {
                        if (!ActiveArenas.Contains(TShock.Regions.GetRegionByName(pair.Key)))
                            ActiveArenas.Add(TShock.Regions.GetRegionByName(pair.Key));
                    }
                    else
                        invalidRegions += (invalidRegions.Length > 0 ? "', '" : "'") + pair.Key;
                }
                else
                    if (!InactiveArenas.Contains(pair.Key))
                        InactiveArenas.Add(pair.Key);
            }

            foreach (string arena in InactiveArenas)
            {
                if (!bossConfig.BossArenas.ContainsKey(arena))
                {
                    InactiveArenas.Remove(arena);
                }
            }

            if (!string.IsNullOrWhiteSpace(invalidRegions))
            {
                invalidRegions = invalidRegions + "'";

                if (console)
                    TSServerPlayer.Server.SendErrorMessage("[AutoBoss+] Invalid regions found: {0}", invalidRegions);
                else if (receiver != null)
                    receiver.SendErrorMessage("[AutoBoss+] Invalid regions found: {0}", invalidRegions);
            }

            invalidRegions = string.Empty;

            foreach (ToggleObj t in bossConfig.BossToggles)
            {
                if (t.type == "day" && t.enabled)
                    AutoBoss.Timer.dayBossEnabled = true;
                else
                    AutoBoss.Timer.dayBossEnabled = false;

                if (t.type == "night" && t.enabled)
                    AutoBoss.Timer.nightBossEnabled = true;
                else
                    AutoBoss.Timer.nightBossEnabled = false;

                if (t.type == "special" && t.enabled)
                    AutoBoss.Timer.specialBossEnabled = true;
                else
                    AutoBoss.Timer.specialBossEnabled = false;
            }

            foreach (ToggleObj t in bossConfig.MinionToggles)
            {
                if (t.type == "day" && t.enabled)
                    AutoBoss.Timer.dayMinionEnabled = true;
                else
                    AutoBoss.Timer.dayMinionEnabled = false;

                if (t.type == "night" && t.enabled)
                    AutoBoss.Timer.nightMinionEnabled = true;
                else
                    AutoBoss.Timer.nightMinionEnabled = false;

                if (t.type == "special" && t.enabled)
                    AutoBoss.Timer.specialMinionEnabled = true;
                else
                    AutoBoss.Timer.specialMinionEnabled = false;
            }

            arenaCount = ActiveArenas.Count;
        }

        #region PostInitialize
        public void PostInitialize(EventArgs args)
        {
            reloadConfig(true);


            foreach (ToggleObj t in bossConfig.BossToggles)
            {
                if (t.type == "day" && t.enabled)
                    AutoBoss.Timer.dayBossEnabled = true;
                if (t.type == "night" && t.enabled)
                    AutoBoss.Timer.nightBossEnabled = true;
                if (t.type == "special" && t.enabled)
                    AutoBoss.Timer.specialBossEnabled = true;
            }

            foreach (ToggleObj t in bossConfig.MinionToggles)
            {
                if (t.type == "day" && t.enabled)
                    AutoBoss.Timer.dayMinionEnabled = true;
                if (t.type == "night" && t.enabled)
                    AutoBoss.Timer.nightMinionEnabled = true;
                if (t.type == "special" && t.enabled)
                    AutoBoss.Timer.specialMinionEnabled = true;
            }
        }
        #endregion

        #region MultipleError
        public void SendMultipleErrors(bool console = false, TSPlayer receiver = null, List<string> errors = null)
        {
            if (errors.Count > 1)
            {
                if (console)
                    TSServerPlayer.Server.SendErrorMessage("Multiple errors encountered: '{0}'", string.Join("', '", errors));
                else
                {
                    receiver.SendErrorMessage("Multiple errors found on reloading:");
                    receiver.SendErrorMessage("'{0}'", string.Join("', '", errors));
                }
            }
            else if (errors.Count == 1)
            {
                if (console)
                    TSServerPlayer.Server.SendErrorMessage("Error encountered: '{0}'", string.Join("", errors));
                else
                    receiver.SendErrorMessage("Error encountered on reloading: '{0}'", string.Join("", errors));
            }
              
        }
        #endregion
    }
}
