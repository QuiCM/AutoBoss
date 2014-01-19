using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace Auto_Boss
{
    public class Boss_Events
    {
        static Random rnd_Num = new Random();

        public static bool Bosses_Active = false;
        public static bool Minions_Active = false;

        #region Day_Battle
        public static void start_BossBattle_Day()
        {
            string broadcastString = "Boss selected:";
            Day_Boss_Set day_Bosses = Boss_Tools.boss_Config.Day_BossList[rnd_Num.Next(0, 
                Boss_Tools.boss_Config.Day_BossList.Count)];

            var bosses = new Dictionary<int, int>();
            foreach (Day_Boss_Obj b in day_Bosses.day_Bosses)
            {
                int npcID = -1;
                foreach (Region region in Boss_Tools.Active_Arenas)
                {
                    int arenaX = region.Area.X + (region.Area.Width / 2);
                    int arenaY = region.Area.Y + (region.Area.Height / 2);

                    for (int i = 0; i < b.amt; i++)
                    {
                        npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, b.id);
                    }
                }

                NPC npc = Main.npc[npcID];
                bosses.Add(npcID, b.id);

                broadcastString += " " + b.amt * Boss_Tools.arena_Count + "x " + npc.name + " +";
            }

            Boss_Tools.boss_List = bosses;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, Color.Crimson);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region Special_Battle
        public static void start_BossBattle_Special()
        {
                        string broadcastString = "Boss selected:";

            Special_Boss_Set special_Bosses = Boss_Tools.boss_Config.Special_BossList[rnd_Num.Next(0,
                Boss_Tools.boss_Config.Special_BossList.Count)];

            var bosses = new Dictionary<int, int>();
            foreach (Special_Boss_Obj b in special_Bosses.special_Bosses)
            {
                int npcID = -1;
                foreach (Region region in Boss_Tools.Active_Arenas)
                {
                    int arenaX = region.Area.X + (region.Area.Width / 2);
                    int arenaY = region.Area.Y + (region.Area.Height / 2);

                    for (int i = 0; i < b.amt; i++)
                    {
                        npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, b.id);
                    }
                }

                NPC npc = Main.npc[npcID];
                bosses.Add(npcID, b.id);

                broadcastString += " " + b.amt * Boss_Tools.arena_Count + "x " + npc.name + " +";
            }

            Boss_Tools.boss_List = bosses;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, Color.Crimson);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region Night_Battle
        public static void start_BossBattle_Night()
        {
            string broadcastString = "Boss selected:";
            Night_Boss_Set night_Bosses = Boss_Tools.boss_Config.Night_BossList[rnd_Num.Next(0,
                Boss_Tools.boss_Config.Night_BossList.Count)];

            var bosses = new Dictionary<int, int>();
            foreach (Night_Boss_Obj b in night_Bosses.night_Bosses)
            {
                int npcID = -1;
                foreach (Region region in Boss_Tools.Active_Arenas)
                {
                    int arenaX = region.Area.X + (region.Area.Width / 2);
                    int arenaY = region.Area.Y + (region.Area.Height / 2);

                    for (int i = 0; i < b.amt; i++)
                    {
                        npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, b.id);
                    }
                }

                NPC npc = Main.npc[npcID];

                bosses.Add(npcID, b.id);

                broadcastString += " " + b.amt * Boss_Tools.arena_Count + "x " + npc.name + " +";
            }

            Boss_Tools.boss_List = bosses;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, Color.Crimson);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region Day_Minions
        public static void start_DayMinion_Spawns()
        {
            var m = Boss_Tools.boss_Config.Day_MinionList[rnd_Num.Next(0, Boss_Tools.boss_Config.Day_MinionList.Count)];

            int henchmenNumber = rnd_Num.Next(m.amt / 2, (int)((m.amt * 2) /  1.5));

            int npcID = -1;
            foreach (Region region in Boss_Tools.Active_Arenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);

                for (int i = 0; i < m.amt; i++)
                {
                    npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
                }
            }

            Boss_Tools.minion_List.Add(npcID, m.id);

            NPC npc = Main.npc[npcID];

            if (Boss_Tools.boss_Config.Announce_Minions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * Boss_Tools.arena_Count + "x " +
                    npc.name + "!", Color.CadetBlue);
            }
        }
        #endregion

        #region Special_Minions
        public static void start_SpecialMinion_Spawns()
        {
            var m = Boss_Tools.boss_Config.Day_MinionList[rnd_Num.Next(0, Boss_Tools.boss_Config.Special_MinionList.Count)];
            
            int henchmenNumber = rnd_Num.Next(m.amt / 2, (int)((m.amt * 2) / 1.5));

            int npcID = -1;
            foreach (Region region in Boss_Tools.Active_Arenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);

                for (int i = 0; i < m.amt; i++)
                {
                    npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
                }
            }

            Boss_Tools.minion_List.Add(npcID, m.id);

            NPC npc = Main.npc[npcID];

            if (Boss_Tools.boss_Config.Announce_Minions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * Boss_Tools.arena_Count + "x " +
                    npc.name + "!", Color.CadetBlue);
            }
        }
        #endregion

        #region Night_Minions
        public static void start_NightMinion_Spawns()
        {
            var m = Boss_Tools.boss_Config.Day_MinionList[rnd_Num.Next(0, Boss_Tools.boss_Config.Night_MinionList.Count)];

            int henchmenNumber = rnd_Num.Next(m.amt / 2, (int)((m.amt * 2) / 1.5));

            int npcID = -1;
            foreach (Region region in Boss_Tools.Active_Arenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);

                for (int i = 0; i < m.amt; i++)
                {
                    npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
                }
            }
            
            Boss_Tools.minion_List.Add(npcID, m.id);

            NPC npc = Main.npc[npcID];

            if (Boss_Tools.boss_Config.Announce_Minions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * Boss_Tools.arena_Count + "x " +
                    npc.name + "!", Color.CadetBlue);
            }
        }
        #endregion
    }
}
