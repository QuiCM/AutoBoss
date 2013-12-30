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
            NPC npc = new NPC();

            string broadcastString = "Boss selected:";
            Day_Boss_Set day_Bosses = Boss_Tools.boss_Config.Day_BossList[rnd_Num.Next(0, 
                Boss_Tools.boss_Config.Day_BossList.Count)];

            foreach (Day_Boss_Obj b in day_Bosses.day_Bosses)
            {
                npc = TShock.Utils.GetNPCById(b.id);
                Boss_Tools.boss_List.Add(npc);

                foreach (Region region in Boss_Tools.Active_Arenas)
                {
                    int arenaX = region.Area.X + (region.Area.Width / 2);
                    int arenaY = region.Area.Y + (region.Area.Height / 2);
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, b.amt, arenaX, arenaY, 30, 30);
                }

                broadcastString += " " + b.amt * Boss_Tools.arena_Count + "x " + npc.name + " +";
            }

            Bosses_Active = true;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, CustomColours.bossColor);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region Special_Battle
        public static void start_BossBattle_Special()
        {
            NPC npc = new NPC();

            string broadcastString = "Boss selected:";

            Special_Boss_Set special_Bosses = Boss_Tools.boss_Config.Special_BossList[rnd_Num.Next(0,
                Boss_Tools.boss_Config.Special_BossList.Count)];

            foreach (Special_Boss_Obj b in special_Bosses.special_Bosses)
            {
                npc = TShock.Utils.GetNPCById(b.id);
                Boss_Tools.boss_List.Add(npc);

                foreach (Region region in Boss_Tools.Active_Arenas)
                {
                    int arenaX = region.Area.X + (region.Area.Width / 2);
                    int arenaY = region.Area.Y + (region.Area.Height / 2);
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, b.amt, arenaX, arenaY, 30, 30);
                }

                broadcastString += " " + b.amt * Boss_Tools.arena_Count + "x " + npc.name + " +";
            }

            Bosses_Active = true;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, CustomColours.bossColor);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region Night_Battle
        public static void start_BossBattle_Night()
        {
            NPC npc = new NPC();

            string broadcastString = "Boss selected:";
            Night_Boss_Set night_Bosses = Boss_Tools.boss_Config.Night_BossList[rnd_Num.Next(0,
                Boss_Tools.boss_Config.Night_BossList.Count)];

            foreach (Night_Boss_Obj b in night_Bosses.night_Bosses)
            {
                npc = TShock.Utils.GetNPCById(b.id);
                Boss_Tools.boss_List.Add(npc);

                foreach (Region region in Boss_Tools.Active_Arenas)
                {
                    int arenaX = region.Area.X + (region.Area.Width / 2);
                    int arenaY = region.Area.Y + (region.Area.Height / 2);
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, b.amt, arenaX, arenaY, 30, 30);
                }

                broadcastString += " " + b.amt * Boss_Tools.arena_Count + "x " + npc.name + " +";
            }

            Bosses_Active = true;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, CustomColours.bossColor);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region Day_Minions
        public static void start_DayMinion_Spawns()
        {
            NPC npc = new NPC();
            var npc_Obj = Boss_Tools.boss_Config.Day_Minionlist[rnd_Num.Next(0, Boss_Tools.boss_Config.Day_Minionlist.Count)];

            npc = TShock.Utils.GetNPCById(npc_Obj.id);

            Boss_Tools.minion_List.Add(npc);

            int henchmenNumber = rnd_Num.Next(npc_Obj.amt / 2, (int)((npc_Obj.amt * 2) /  1.5));

            foreach (Region region in Boss_Tools.Active_Arenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);
                TSPlayer.Server.SpawnNPC(npc.type, npc.name, henchmenNumber, arenaX, arenaY, 30, 30);
            }

            if (Boss_Tools.boss_Config.Announce_Minions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * Boss_Tools.arena_Count + "x " +
                    npc.name + "!", CustomColours.minionColor);
            }

            Minions_Active = true;
        }
        #endregion

        #region Special_Minions
        public static void start_SpecialMinion_Spawns()
        {
            NPC npc = new NPC();
            var npc_Obj = Boss_Tools.boss_Config.Special_MinionList[rnd_Num.Next(0,
                Boss_Tools.boss_Config.Special_MinionList.Count)];

            npc = TShock.Utils.GetNPCById(npc_Obj.id);

            Boss_Tools.minion_List.Add(npc);

            int henchmenNumber = rnd_Num.Next(npc_Obj.amt / 2, (int)((npc_Obj.amt * 2) / 1.5));

            foreach (Region region in Boss_Tools.Active_Arenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);
                TSPlayer.Server.SpawnNPC(npc.type, npc.name, henchmenNumber, arenaX, arenaY, 30, 30);
            }

            if (Boss_Tools.boss_Config.Announce_Minions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * Boss_Tools.arena_Count + "x " +
                    npc.name + "!", CustomColours.minionColor);
            }

            Minions_Active = true;
        }
        #endregion

        #region Night_Minions
        public static void start_NightMinion_Spawns()
        {
            NPC npc = new NPC();
            var npc_Obj = Boss_Tools.boss_Config.Night_MinionList[rnd_Num.Next(0, Boss_Tools.boss_Config.Night_MinionList.Count)];

            npc = TShock.Utils.GetNPCById(npc_Obj.id);

            Boss_Tools.minion_List.Add(npc);

            int henchmenNumber = rnd_Num.Next(npc_Obj.amt / 2, (int)((npc_Obj.amt * 2) / 1.5));

            foreach (Region region in Boss_Tools.Active_Arenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);
                TSPlayer.Server.SpawnNPC(npc.type, npc.name, henchmenNumber, arenaX, arenaY, 30, 30);
            }

            if (Boss_Tools.boss_Config.Announce_Minions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * Boss_Tools.arena_Count + "x " +
                    npc.name + "!", CustomColours.minionColor);
            }

            Minions_Active = true;
        }
        #endregion
    }
}
