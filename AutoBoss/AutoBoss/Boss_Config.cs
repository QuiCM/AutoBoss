using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;

using Terraria;
using TShockAPI;

namespace Auto_Boss
{
    #region Minions
    public class Day_Minion_Obj
    {
        public int id;
        public int amt;
        public Day_Minion_Obj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class Special_Minion_Obj
    {
        public int id;
        public int amt;
        public Special_Minion_Obj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class Night_Minion_Obj
    {
        public int id;
        public int amt;
        public Night_Minion_Obj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }
    #endregion

    #region Bosses
    public class Day_Boss_Obj
    {
        public int id;
        public int amt;
        public Day_Boss_Obj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class Special_Boss_Obj
    {
        public int id;
        public int amt;
        public Special_Boss_Obj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class Night_Boss_Obj
    {
        public int id;
        public int amt;
        public Night_Boss_Obj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }

    public class Day_Boss_Set
    {
        public List<Day_Boss_Obj> day_Bosses;
        public Day_Boss_Set(List<Day_Boss_Obj> day_Bosses)
        {
            this.day_Bosses = day_Bosses;
        }
    }

    public class Special_Boss_Set
    {
        public List<Special_Boss_Obj> special_Bosses;
        public Special_Boss_Set(List<Special_Boss_Obj> special_Bosses)
        {
            this.special_Bosses = special_Bosses;
        }
    }

    public class Night_Boss_Set
    {
        public List<Night_Boss_Obj> night_Bosses;
        public Night_Boss_Set(List<Night_Boss_Obj> night_Bosses)
        {
            this.night_Bosses = night_Bosses;
        }
    }
    #endregion

    public class Toggle_Obj
    {
        public string type;
        public bool enabled;

        public Toggle_Obj(string t, bool e)
        {
            type = t;
            enabled = e;
        }
    }

    public class Boss_Config
    {
        public bool AutoStart_Enabled = false;

        public bool Continuous_Boss = false;

        public int Message_Interval = 10;

        public bool Enable_DayTimer_Text = false;
        public string[] DayTimer_Text =
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
        public string DayTimer_Finished = "Boss battle complete.";

        public bool Enable_NightTimer_Text = false;
        public string[] NightTimer_Text =
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
        public string NightTimer_Finished = "Boss battle complete.";

        public bool Enable_SpecialTimer_Text = false;
        public string[] SpecialTimer_Text =
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
        public string SpecialTimer_Finished = "Boss battle complete.";

        public bool Announce_Minions = false;
        public int[] Minions_Spawn_Timer = new int[2] { 10, 30 };

        public Dictionary<string, bool> Boss_Arenas = new Dictionary<string, bool>();

        public List<Toggle_Obj> Boss_Toggles;

        public List<Toggle_Obj> Minion_Toggles;

        /* Now has much nicer formatting & names */
        public List<Day_Boss_Set> Day_BossList;
        public List<Day_Minion_Obj> Day_Minionlist;

        public List<Night_Boss_Set> Night_BossList;
        public List<Night_Minion_Obj> Night_MinionList;

        public List<Special_Boss_Set> Special_BossList;
        public List<Special_Minion_Obj> Special_MinionList;

        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Boss_Config Read(string path)
        {
            if (!File.Exists(path))
                return new Boss_Config();
            return JsonConvert.DeserializeObject<Boss_Config>(File.ReadAllText(path));
        }

        public static Action<Boss_Config> ConfigRead;
    }
}