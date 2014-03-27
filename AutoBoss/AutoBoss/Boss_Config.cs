using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;

using Terraria;
using TShockAPI;

namespace AutoBoss
{
    #region Minions
    public class DayMinionObj
    {
        public int id;
        public int amt;
        public DayMinionObj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class SpecialMinionObj
    {
        public int id;
        public int amt;
        public SpecialMinionObj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class NightMinionObj
    {
        public int id;
        public int amt;
        public NightMinionObj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }
    #endregion

    #region Bosses
    public class DayBossObj
    {
        public int id;
        public int amt;
        public DayBossObj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class SpecialBossObj
    {
        public int id;
        public int amt;
        public SpecialBossObj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class NightBossObj
    {
        public int id;
        public int amt;
        public NightBossObj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class DayBossSet
    {
        public List<DayBossObj> dayBosses;
        public DayBossSet(List<DayBossObj> dayBosses)
        {
            this.dayBosses = dayBosses;
        }
    }

    public class SpecialBossSet
    {
        public List<SpecialBossObj> specialBosses;
        public SpecialBossSet(List<SpecialBossObj> specialBosses)
        {
            this.specialBosses = specialBosses;
        }
    }

    public class NightBossSet
    {
        public List<NightBossObj> nightBosses;
        public NightBossSet(List<NightBossObj> nightBosses)
        {
            this.nightBosses = nightBosses;
        }
    }
    #endregion

    public class ToggleObj
    {
        public string type;
        public bool enabled;

        public ToggleObj(string t, bool e)
        {
            type = t;
            enabled = e;
        }
    }

    public class BossConfig
    {
        public bool AutoStartEnabled = false;

        public bool ContinuousBoss = false;

        public int MessageInterval = 10;

        public bool EnableDayTimerText = false;
        public string[] DayTimerText =
        {
            "[Note] These messages are printed in-game in the order they are placed in this config file",
            "[Note] Leave the message that indicates the spawning of bosses last. These notes can be deleted",
            "[Note] All pieces of text in this section can be modified, deleted etc.",
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
            "[Note] These messages are printed in-game in the order they are placed in this config file",
            "[Note] Leave the message that indicates the spawning of bosses last. These notes can be deleted",
            "[Note] All pieces of text in this section can be modified, deleted etc.",
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
            "[Note] These messages are printed in-game in the order they are placed in this config file",
            "[Note] Leave the message that indicates the spawning of bosses last. These notes can be deleted",
            "[Note] All pieces of text in this section can be modified, deleted etc.",
            "[Special] Initial message",
            "[Special] Secondary message",
            "[Special] Third message",
            "[Special] Etc",
            "[Special] Bosses spawning"
        };
        public string SpecialTimerFinished = "Boss battle complete.";

        public bool AnnounceMinions = false;
        public int[] MinionsSpawnTimer = new int[2] { 10, 30 };

        public Dictionary<string, bool> BossArenas = new Dictionary<string, bool>();

        public List<ToggleObj> BossToggles;

        public List<ToggleObj> MinionToggles;

        public List<DayBossSet> DayBossList;
        public List<DayMinionObj> DayMinionList;

        public List<NightBossSet> NightBossList;
        public List<NightMinionObj> NightMinionList;

        public List<SpecialBossSet> SpecialBossList;
        public List<SpecialMinionObj> SpecialMinionList;

        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static BossConfig Read(string path)
        {
            if (!File.Exists(path))
                return new BossConfig();
            return JsonConvert.DeserializeObject<BossConfig>(File.ReadAllText(path));
        }

        public static Action<BossConfig> ConfigRead;
    }
}