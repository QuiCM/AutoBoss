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

namespace Auto_Boss
{
    public class Boss_Tools
    {
        public static Boss_Config boss_Config { get; set; }
        public static string config_Path { get { return Path.Combine(TShock.SavePath, "Boss_Config.json"); } }

        public static string invalid_Regions = string.Empty;

        public static Dictionary<int, int> boss_List = new Dictionary<int, int>();
        public static Dictionary<int, int> minion_List = new Dictionary<int, int>();

        public static List<Region> Active_Arenas = new List<Region>();
        public static List<string> Inactive_Arenas = new List<string>();
        public static int arena_Count = 0;

        public static bool Bosses_Toggled = false;

        #region Setup_Config
       /* public static void SetupConfig()
        {
            bool failed = false;

            List<string> logExceptions = new List<string>();

            foreach (KeyValuePair<string, bool> pair in boss_Config.Boss_Arenas)
            {
                if (pair.Value == true)
                {
                    if (TShock.Regions.GetRegionByName(pair.Key) != null)
                    {
                        if (!Active_Arenas.Contains(TShock.Regions.GetRegionByName(pair.Key)))
                            Active_Arenas.Add(TShock.Regions.GetRegionByName(pair.Key));

                        failed = false;
                    }

                    else
                    {
                        invalid_Regions += (invalid_Regions.Length > 0 ? ", " : "") + pair.Key;

                        logExceptions.Add("Invalid Regions: " + invalid_Regions);

                        invalid_Regions = string.Empty;

                        failed = true;
                    }
                }
            }

            arena_Count = Active_Arenas.Count;

            if (arena_Count > 0)
                Bosses_Toggled = boss_Config.AutoStart_Enabled;

            if (arena_Count == 0)
                logExceptions.Add("No arenas defined");

            if (!failed)
                Log.ConsoleInfo("[AutoBoss+] Initialized successfully");
            else
                SendMultipleErrors(true, null, logExceptions);
            
        }*/
        #endregion

        public static void reloadConfig(bool console = false, TSPlayer receiver = null)
        {
            (boss_Config = Boss_Config.Read(config_Path)).Write(config_Path);

            foreach (KeyValuePair<string, bool> pair in boss_Config.Boss_Arenas)
            {
                if (pair.Value)
                {
                    if (TShock.Regions.GetRegionByName(pair.Key) != null)
                    {
                        if (!Active_Arenas.Contains(TShock.Regions.GetRegionByName(pair.Key)))
                            Active_Arenas.Add(TShock.Regions.GetRegionByName(pair.Key));
                    }
                    else
                        invalid_Regions += (invalid_Regions.Length > 0 ? "', '" : "'") + pair.Key;
                }
                else
                    if (!Inactive_Arenas.Contains(pair.Key))
                        Inactive_Arenas.Add(pair.Key);
            }

            foreach (string arena in Inactive_Arenas)
            {
                if (!boss_Config.Boss_Arenas.ContainsKey(arena))
                {
                    Inactive_Arenas.Remove(arena);
                }
            }

            if (!string.IsNullOrWhiteSpace(invalid_Regions))
            {
                invalid_Regions = invalid_Regions + "'";

                if (console)
                    TSServerPlayer.Server.SendErrorMessage("[AutoBoss+] Invalid regions found: {0}", invalid_Regions);
                else if (receiver != null)
                    receiver.SendErrorMessage("[AutoBoss+] Invalid regions found: {0}", invalid_Regions);
            }

            invalid_Regions = string.Empty;

            foreach (Toggle_Obj t in boss_Config.Boss_Toggles)
            {
                if (t.type == "day" && t.enabled)
                    Boss_Timer.dayBossEnabled = true;
                else
                    Boss_Timer.dayBossEnabled = false;

                if (t.type == "night" && t.enabled)
                    Boss_Timer.nightBossEnabled = true;
                else
                    Boss_Timer.nightBossEnabled = false;

                if (t.type == "special" && t.enabled)
                    Boss_Timer.specialBossEnabled = true;
                else
                    Boss_Timer.specialBossEnabled = false;
            }

            foreach (Toggle_Obj t in boss_Config.Minion_Toggles)
            {
                if (t.type == "day" && t.enabled)
                    Boss_Timer.dayMinionEnabled = true;
                else
                    Boss_Timer.dayMinionEnabled = false;

                if (t.type == "night" && t.enabled)
                    Boss_Timer.nightMinionEnabled = true;
                else
                    Boss_Timer.nightMinionEnabled = false;

                if (t.type == "special" && t.enabled)
                    Boss_Timer.specialMinionEnabled = true;
                else 
                    Boss_Timer.specialMinionEnabled = false;
            }
        }

        #region PostInitialize
        public static void PostInitialize(EventArgs args)
        {
            reloadConfig(true);


            foreach (Toggle_Obj t in boss_Config.Boss_Toggles)
            {
                if (t.type == "day" && t.enabled)
                    Boss_Timer.dayBossEnabled = true;
                if (t.type == "night" && t.enabled)
                    Boss_Timer.nightBossEnabled = true;
                if (t.type == "special" && t.enabled)
                    Boss_Timer.specialBossEnabled = true;
            }

            foreach (Toggle_Obj t in boss_Config.Minion_Toggles)
            {
                if (t.type == "day" && t.enabled)
                    Boss_Timer.dayMinionEnabled = true;
                if (t.type == "night" && t.enabled)
                    Boss_Timer.nightMinionEnabled = true;
                if (t.type == "special" && t.enabled)
                    Boss_Timer.specialMinionEnabled = true;
            }
        }
        #endregion

        #region MultipleError
        public static void SendMultipleErrors(bool console = false, TSPlayer receiver = null, List<string> errors = null)
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
