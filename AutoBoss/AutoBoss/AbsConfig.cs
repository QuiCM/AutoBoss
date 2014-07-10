using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace AutoBoss
{
    public class Config
    {
        public bool AutoStartEnabled = false;

        public bool ContinuousBoss = false;

        public int MessageInterval = 10;

        public bool EnableDayTimerText = false;
        public string[] DayTimerText =
        {
            "[Day] Initial message",
            "[Day] Secondary message",
            "[Day] Third message",
            "[Day] Etc",
            "[Day] Bosses spawning"
        };
        public string DayTimerFinished = "Boss battle complete.";

        public bool EnableNightTimerText = false;
        public string[] NightTimerText =
        {
            "[Night] Initial message",
            "[Night] Secondary message",
            "[Night] Third message",
            "[Night] Etc",
            "[Night] Bosses spawning"
        };
        public string NightTimerFinished = "Boss battle complete.";

        public bool EnableSpecialTimerText = false;
        public string[] SpecialTimerText =
        {
            "[Special] Initial message",
            "[Special] Secondary message",
            "[Special] Third message",
            "[Special] Etc",
            "[Special] Bosses spawning"
        };
        public string SpecialTimerFinished = "Boss battle complete.";

        public bool AnnounceMinions = false;
        public int[] MinionsSpawnTimer = { 10, 30 };

        public Dictionary<string, bool> BossArenas = new Dictionary<string, bool>();

        public Dictionary<string, bool> BossToggles = new Dictionary<string, bool>
        {
            {"day", false},
            {"night", false},
            {"special", false}
        };

        public Dictionary<string, bool> MinionToggles = new Dictionary<string, bool>
        {
            {"day", false},
            {"night", false},
            {"special", false}
        };


        public Dictionary<string, Dictionary<int, int>> DayBosses = new Dictionary<string, Dictionary<int, int>>
        {
            {"test", new Dictionary<int, int>{{1, 1}}}
        };
        public Dictionary<string, Dictionary<int, int>> NightBosses = new Dictionary<string, Dictionary<int, int>>();
        public Dictionary<string, Dictionary<int, int>> SpecialBosses = new Dictionary<string, Dictionary<int, int>>();
        
        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string path)
        {
            return !File.Exists(path)
                ? new Config()
                : JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }
    }
}