using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace AutoBoss
{
    public class BossEvents
    {
        static Random rndNum = new Random();

        public static bool BossesActive = false;
        public static bool MinionsActive = false;

        #region DayBattle
        public static void startBossBattleDay()
        {
            string broadcastString = "Boss selected:";
            DayBossSet dayBosses = AutoBoss.Tools.bossConfig.DayBossList[rndNum.Next(0, 
                AutoBoss.Tools.bossConfig.DayBossList.Count)];

            var bosses = new Dictionary<int, int>();
            foreach (DayBossObj b in dayBosses.dayBosses)
            {
                int npcID = -1;
                foreach (Region region in AutoBoss.Tools.ActiveArenas)
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

                broadcastString += " " + b.amt * AutoBoss.Tools.arenaCount + "x " + npc.name + " +";
            }

            AutoBoss.Tools.bossList = bosses;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, Color.Crimson);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region SpecialBattle
        public static void startBossBattleSpecial()
        {
                        string broadcastString = "Boss selected:";

            SpecialBossSet specialBosses = AutoBoss.Tools.bossConfig.SpecialBossList[rndNum.Next(0,
                AutoBoss.Tools.bossConfig.SpecialBossList.Count)];

            var bosses = new Dictionary<int, int>();
            foreach (SpecialBossObj b in specialBosses.specialBosses)
            {
                int npcID = -1;
                foreach (Region region in AutoBoss.Tools.ActiveArenas)
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

                broadcastString += " " + b.amt * AutoBoss.Tools.arenaCount + "x " + npc.name + " +";
            }

            AutoBoss.Tools.bossList = bosses;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, Color.Crimson);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region NightBattle
        public static void startBossBattleNight()
        {
            string broadcastString = "Boss selected:";
            NightBossSet nightBosses = AutoBoss.Tools.bossConfig.NightBossList[rndNum.Next(0,
                AutoBoss.Tools.bossConfig.NightBossList.Count)];

            var bosses = new Dictionary<int, int>();
            foreach (NightBossObj b in nightBosses.nightBosses)
            {
                int npcID = -1;
                foreach (Region region in AutoBoss.Tools.ActiveArenas)
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

                broadcastString += " " + b.amt * AutoBoss.Tools.arenaCount + "x " + npc.name + " +";
            }

            AutoBoss.Tools.bossList = bosses;

            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TSPlayer.All.SendMessage(broadcastString, Color.Crimson);

            Console.WriteLine(broadcastString);
        }
        #endregion

        #region DayMinions
        public static void startDayMinionSpawns()
        {
            var m = AutoBoss.Tools.bossConfig.DayMinionList[rndNum.Next(0, AutoBoss.Tools.bossConfig.DayMinionList.Count)];
            
            int npcID = -1;
            foreach (Region region in AutoBoss.Tools.ActiveArenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);

                for (int i = 0; i < m.amt; i++)
                {
                    npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
                }
            }

            AutoBoss.Tools.minionList.Add(npcID, m.id);

            NPC npc = Main.npc[npcID];

            if (AutoBoss.Tools.bossConfig.AnnounceMinions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + m.amt * AutoBoss.Tools.arenaCount + "x " +
                    npc.name + "!", Color.CadetBlue);
            }
        }
        #endregion

        #region SpecialMinions
        public static void startSpecialMinionSpawns()
        {
            var m = AutoBoss.Tools.bossConfig.DayMinionList[rndNum.Next(0, AutoBoss.Tools.bossConfig.SpecialMinionList.Count)];
            
            int henchmenNumber = rndNum.Next(m.amt / 2, (int)((m.amt * 2) / 1.5));

            int npcID = -1;
            foreach (Region region in AutoBoss.Tools.ActiveArenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);

                for (int i = 0; i < m.amt; i++)
                {
                    npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
                }
            }

            AutoBoss.Tools.minionList.Add(npcID, m.id);

            NPC npc = Main.npc[npcID];

            if (AutoBoss.Tools.bossConfig.AnnounceMinions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * AutoBoss.Tools.arenaCount + "x " +
                    npc.name + "!", Color.CadetBlue);
            }
        }
        #endregion

        #region NightMinions
        public static void startNightMinionSpawns()
        {
            var m = AutoBoss.Tools.bossConfig.DayMinionList[rndNum.Next(0, AutoBoss.Tools.bossConfig.NightMinionList.Count)];

            int henchmenNumber = rndNum.Next(m.amt / 2, (int)((m.amt * 2) / 1.5));

            int npcID = -1;
            foreach (Region region in AutoBoss.Tools.ActiveArenas)
            {
                int arenaX = region.Area.X + (region.Area.Width / 2);
                int arenaY = region.Area.Y + (region.Area.Height / 2);

                for (int i = 0; i < m.amt; i++)
                {
                    npcID = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
                }
            }
            
            AutoBoss.Tools.minionList.Add(npcID, m.id);

            NPC npc = Main.npc[npcID];

            if (AutoBoss.Tools.bossConfig.AnnounceMinions)
            {
                TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * AutoBoss.Tools.arenaCount + "x " +
                    npc.name + "!", Color.CadetBlue);
            }
        }
        #endregion
    }
}
