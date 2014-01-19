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

    public class message_Obj
    {
        public byte red;
        public byte green;
        public byte blue;
        public string type;
        public bool useCustomColor;

        public message_Obj(byte r, byte g, byte b, string t, bool use)
        {
            red = r;
            green = g;
            blue = b;
            type = t;
            useCustomColor = use;
        }
    }

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

        public List<message_Obj> Message_Colours = new List<message_Obj>()
        { 
            new message_Obj(255, 255, 255, "[WARNING] Please do not change the case or names of these values", false),

            new message_Obj(255, 255, 255, "day", false),
            new message_Obj(255, 255, 255, "night", false),
            new message_Obj(255, 255, 255, "special", false), 
            new message_Obj(255, 255, 255, "minion", false),
            new message_Obj(255, 255, 255, "boss", false),
            new message_Obj(255, 255, 255, "command", false),
            new message_Obj(255, 255, 255, "message", false)
        };

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

        public Dictionary<string, bool> Boss_Arenas = new Dictionary<string, bool>() { { "example_Arena", false } };

        public List<Toggle_Obj> Boss_Toggles = new List<Toggle_Obj>()
        {
            new Toggle_Obj("[Note] Please do not change the case or names of these values", false),

            new Toggle_Obj("day", false),
            new Toggle_Obj("night", false),
            new Toggle_Obj("special", false)
        };

        public List<Toggle_Obj> Minion_Toggles = new List<Toggle_Obj>()
        {
            new Toggle_Obj("[Note] Please do not change the case or names of these values", false),

            new Toggle_Obj("day", false),
            new Toggle_Obj("night", false),
            new Toggle_Obj("special", false)
        };

        /* Now has much nicer formatting & names */
        public List<Day_Boss_Set> Day_BossList;
        public List<Day_Minion_Obj> Day_Minionlist;

        public List<Night_Boss_Set> Night_BossList;
        public List<Night_Minion_Obj> Night_MinionList;

        public List<Special_Boss_Set> Special_BossList;
        public List<Special_Minion_Obj> Special_MinionList;



        public static Boss_Config Read(string path)
        {
            if (!File.Exists(path))
                return new Boss_Config();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }

        public static Boss_Config Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<Boss_Config>(sr.ReadToEnd());
                if (ConfigRead != null)
                    ConfigRead(cf);
                return cf;
            }
        }

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }

        public void Write(Stream stream)
        {
            /* Night bosses */
            {
                Night_BossList = new List<Night_Boss_Set>();
                List<Night_Boss_Obj> night_Bosses = new List<Night_Boss_Obj>();
                night_Bosses.Add(new Night_Boss_Obj(4, 5));
                Night_BossList.Add(new Night_Boss_Set(night_Bosses));

                night_Bosses = new List<Night_Boss_Obj>();
                night_Bosses.Add(new Night_Boss_Obj(134, 1));
                Night_BossList.Add(new Night_Boss_Set(night_Bosses));

                night_Bosses = new List<Night_Boss_Obj>();
                night_Bosses.Add(new Night_Boss_Obj(125, 1));
                night_Bosses.Add(new Night_Boss_Obj(126, 1));
                Night_BossList.Add(new Night_Boss_Set(night_Bosses));

                night_Bosses = new List<Night_Boss_Obj>();
                night_Bosses.Add(new Night_Boss_Obj(127, 1));
                Night_BossList.Add(new Night_Boss_Set(night_Bosses));

                night_Bosses = new List<Night_Boss_Obj>();
                night_Bosses.Add(new Night_Boss_Obj(134, 2));
                Night_BossList.Add(new Night_Boss_Set(night_Bosses));

                night_Bosses = new List<Night_Boss_Obj>();
                night_Bosses.Add(new Night_Boss_Obj(50, 5));
                Night_BossList.Add(new Night_Boss_Set(night_Bosses));
            }

            /* Day Bosses */
            {
                Day_BossList = new List<Day_Boss_Set>();
                List<Day_Boss_Obj> day_Bosses = new List<Day_Boss_Obj>();
                day_Bosses.Add(new Day_Boss_Obj(50, 5));
                Day_BossList.Add(new Day_Boss_Set(day_Bosses));

                day_Bosses = new List<Day_Boss_Obj>();
                day_Bosses.Add(new Day_Boss_Obj(50, 1));
                Day_BossList.Add(new Day_Boss_Set(day_Bosses));

                day_Bosses = new List<Day_Boss_Obj>();
                day_Bosses.Add(new Day_Boss_Obj(87, 12));
                Day_BossList.Add(new Day_Boss_Set(day_Bosses));

                day_Bosses = new List<Day_Boss_Obj>();
                day_Bosses.Add(new Day_Boss_Obj(143, 10));
                day_Bosses.Add(new Day_Boss_Obj(144, 5));
                day_Bosses.Add(new Day_Boss_Obj(145, 15));
                Day_BossList.Add(new Day_Boss_Set(day_Bosses));

                day_Bosses = new List<Day_Boss_Obj>();
                day_Bosses.Add(new Day_Boss_Obj(87, 2));
                Day_BossList.Add(new Day_Boss_Set(day_Bosses));
            }

            /* Special bosses */
            {
                Special_BossList = new List<Special_Boss_Set>();
                List<Special_Boss_Obj> special_Bosses = new List<Special_Boss_Obj>();
                special_Bosses.Add(new Special_Boss_Obj(1, 10));
                Special_BossList.Add(new Special_Boss_Set(special_Bosses));
            }

            /* Minions */
            {
                Day_Minionlist = new List<Day_Minion_Obj>();
                Day_Minionlist.Add(new Day_Minion_Obj(1, 1));
                Day_Minionlist.Add(new Day_Minion_Obj(2, 1));

                Night_MinionList = new List<Night_Minion_Obj>();
                Night_MinionList.Add(new Night_Minion_Obj(1, 1));
                Night_MinionList.Add(new Night_Minion_Obj(2, 1));

                Special_MinionList = new List<Special_Minion_Obj>();
                Special_MinionList.Add(new Special_Minion_Obj(1, 1));
                Special_MinionList.Add(new Special_Minion_Obj(2, 1));
            }

            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }
        public static Action<Boss_Config> ConfigRead;
    }
}