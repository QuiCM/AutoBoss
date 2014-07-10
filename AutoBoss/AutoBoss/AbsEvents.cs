using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TShockAPI;

namespace AutoBoss
{
    public static class BossEvents
    {
        static readonly Random R = new Random();

        #region DayBattle
        public static void StartBossBattleDay()
        {
            var broadcast = new List<string>();

            var bossLists = AutoBoss.config.DayBosses.Values.ToList();

            var bosses = bossLists[R.Next(0, bossLists.Count)];
            AutoBoss.bossList.Clear();

            foreach (var item in bosses)
            {
                var npcId = -1;
                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width/2);
                    var arenaY = region.Area.Y + (region.Area.Height/2);

                    for (var i = 0; i < item.Value; i++)
                        npcId = NPC.NewNPC(arenaX*16, arenaY*16, item.Key);
                }

                var npc = Main.npc[npcId];
                AutoBoss.bossList.Add(npcId, item.Key);
                broadcast.Add(string.Format("{0}x {1}", item.Value*AutoBoss.ActiveArenas.Count, npc.name));
            }

            var bcStr = string.Join(", ", broadcast);
            TShock.Utils.Broadcast("Bosses selected: " + bcStr, Color.Crimson);
        }
        #endregion

        #region SpecialBattle
        public static void StartBossBattleSpecial()
        {
            var broadcast = new List<string>();

            var bossLists = AutoBoss.config.SpecialBosses.Values.ToList();

            var bosses = bossLists[R.Next(0, bossLists.Count)];
            AutoBoss.bossList.Clear();

            foreach (var item in bosses)
            {
                var npcId = -1;
                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width / 2);
                    var arenaY = region.Area.Y + (region.Area.Height / 2);

                    for (var i = 0; i < item.Value; i++)
                        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, item.Key);
                }

                var npc = Main.npc[npcId];
                AutoBoss.bossList.Add(npcId, item.Key);
                broadcast.Add(string.Format("{0}x {1}", item.Value * AutoBoss.ActiveArenas.Count, npc.name));
            }

            var bcStr = string.Join(", ", broadcast);
            TShock.Utils.Broadcast("Bosses selected: " + bcStr, Color.Crimson);
        }
        #endregion

        #region NightBattle
        public static void StartBossBattleNight()
        {
            var broadcast = new List<string>();

            var bossLists = AutoBoss.config.NightBosses.Values.ToList();

            var bosses = bossLists[R.Next(0, bossLists.Count)];
            AutoBoss.bossList.Clear();

            foreach (var item in bosses)
            {
                var npcId = -1;
                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width / 2);
                    var arenaY = region.Area.Y + (region.Area.Height / 2);

                    for (var i = 0; i < item.Value; i++)
                        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, item.Key);
                }

                var npc = Main.npc[npcId];
                AutoBoss.bossList.Add(npcId, item.Key);
                broadcast.Add(string.Format("{0}x {1}", item.Value * AutoBoss.ActiveArenas.Count, npc.name));
            }

            var bcStr = string.Join(", ", broadcast);
            TShock.Utils.Broadcast("Bosses selected: " + bcStr, Color.Crimson);
        }
        #endregion

        #region DayMinions
        public static void StartDayMinionSpawns()
        {
            //var m = AutoBoss.config.DayMinionList[rndNum.Next(0, AutoBoss.config.DayMinionList.Count)];
            
            //var npcId = -1;
            //foreach (var region in AutoBoss.ActiveArenas)
            //{
            //    var arenaX = region.Area.X + (region.Area.Width / 2);
            //    var arenaY = region.Area.Y + (region.Area.Height / 2);

            //    for (var i = 0; i < m.amt; i++)
            //        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
            //}

            //AutoBoss.minionList.Add(npcId, m.id);

            //var npc = Main.npc[npcId];

            //if (AutoBoss.config.AnnounceMinions)
            //{
            //    TSPlayer.All.SendMessage("Spawning Minions: " + m.amt * AutoBoss.Tools.arenaCount + "x " +
            //        npc.name + "!", Color.CadetBlue);
            //}
        }
        #endregion

        #region SpecialMinions
        public static void StartSpecialMinionSpawns()
        {
            //var m = AutoBoss.config.DayMinionList[rndNum.Next(0, AutoBoss.config.SpecialMinionList.Count)];
            
            //var henchmenNumber = rndNum.Next(m.amt / 2, (int)((m.amt * 2) / 1.5));

            //var npcId = -1;
            //foreach (Region region in AutoBoss.ActiveArenas)
            //{
            //    var arenaX = region.Area.X + (region.Area.Width / 2);
            //    var arenaY = region.Area.Y + (region.Area.Height / 2);

            //    for (var i = 0; i < m.amt; i++)
            //        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
            //}

            //AutoBoss.minionList.Add(npcId, m.id);

            //var npc = Main.npc[npcId];

            //if (AutoBoss.config.AnnounceMinions)
            //{
            //    TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * AutoBoss.Tools.arenaCount + "x " +
            //        npc.name + "!", Color.CadetBlue);
            //}
        }
        #endregion

        #region NightMinions
        public static void StartNightMinionSpawns()
        {
            //var m = AutoBoss.config.DayMinionList[rndNum.Next(0, AutoBoss.config.NightMinionList.Count)];

            //var henchmenNumber = rndNum.Next(m.amt / 2, (int)((m.amt * 2) / 1.5));

            //var npcId = -1;
            //foreach (var region in AutoBoss.ActiveArenas)
            //{
            //    var arenaX = region.Area.X + (region.Area.Width / 2);
            //    var arenaY = region.Area.Y + (region.Area.Height / 2);

            //    for (var i = 0; i < m.amt; i++)
            //        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, m.id);
            //}
            
            //AutoBoss.minionList.Add(npcId, m.id);

            //var npc = Main.npc[npcId];

            //if (AutoBoss.config.AnnounceMinions)
            //{
            //    TSPlayer.All.SendMessage("Spawning Minions: " + henchmenNumber * AutoBoss.Tools.arenaCount + "x " +
            //        npc.name + "!", Color.CadetBlue);
            //}
        }
        #endregion
    }
}
