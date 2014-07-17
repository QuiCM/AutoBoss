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
            AutoBoss.bossCounts.Clear();

            foreach (var item in bosses)
            {
                var npcId = -1;
                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width/2);
                    var arenaY = region.Area.Y + (region.Area.Height/2);

                    for (var i = 0; i < item.Value; i++)
                    {
                        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, item.Key);
                        AutoBoss.bossList.AddSafe(npcId, item.Key);
                    }
                }

                var npc = Main.npc[npcId];
                broadcast.Add(string.Format("{0}x {1}", item.Value*AutoBoss.ActiveArenas.Count, npc.name));
            }

            var bcStr = string.Join(", ", broadcast);
            TShock.Utils.Broadcast("Bosses selected: " + bcStr, Color.Crimson);

            if (AutoBoss.config.MinionToggles["day"])
            {
                BossTimer.minionTime = R.Next(AutoBoss.config.MinionsSpawnTimer[0],
                    AutoBoss.config.MinionsSpawnTimer[1] + 1);

                BossTimer.minionSpawnCount = R.Next(AutoBoss.config.MinionSpawnCount[0],
                    AutoBoss.config.MinionSpawnCount[1] + 1);

                StartMinionSpawns(BossTimer.minionSpawnCount, SelectMinions("day"));
            }
        }
        #endregion

        #region SpecialBattle
        public static void StartBossBattleSpecial()
        {
            var broadcast = new List<string>();

            var bossLists = AutoBoss.config.SpecialBosses.Values.ToList();

            var bosses = bossLists[R.Next(0, bossLists.Count)];
            AutoBoss.bossList.Clear();
            AutoBoss.bossCounts.Clear();

            foreach (var item in bosses)
            {
                var npcId = -1;
                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width / 2);
                    var arenaY = region.Area.Y + (region.Area.Height / 2);

                    for (var i = 0; i < item.Value; i++)
                    {
                        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, item.Key);
                        AutoBoss.bossList.AddSafe(npcId, item.Key);
                    }
                }

                var npc = Main.npc[npcId];
                broadcast.Add(string.Format("{0}x {1}", item.Value * AutoBoss.ActiveArenas.Count, npc.name));
            }

            var bcStr = string.Join(", ", broadcast);
            TShock.Utils.Broadcast("Bosses selected: " + bcStr, Color.Crimson);

            if (AutoBoss.config.MinionToggles["special"])
            {
                BossTimer.minionTime = R.Next(AutoBoss.config.MinionsSpawnTimer[0],
                    AutoBoss.config.MinionsSpawnTimer[1] + 1);

                BossTimer.minionSpawnCount = R.Next(AutoBoss.config.MinionSpawnCount[0],
                    AutoBoss.config.MinionSpawnCount[1] + 1);

                StartMinionSpawns(BossTimer.minionSpawnCount, SelectMinions("special"));
            }
        }
        #endregion

        #region NightBattle
        public static void StartBossBattleNight()
        {
            var broadcast = new List<string>();

            var bossLists = AutoBoss.config.NightBosses.Values.ToList();

            var bosses = bossLists[R.Next(0, bossLists.Count)];
            AutoBoss.bossList.Clear();
            AutoBoss.bossCounts.Clear();

            foreach (var item in bosses)
            {
                var npcId = -1;
                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width / 2);
                    var arenaY = region.Area.Y + (region.Area.Height / 2);

                    for (var i = 0; i < item.Value; i++)
                    {
                        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, item.Key);
                        AutoBoss.bossList.AddSafe(npcId, item.Key);
                    }
                }

                var npc = Main.npc[npcId];
                broadcast.Add(string.Format("{0}x {1}", item.Value * AutoBoss.ActiveArenas.Count, npc.name));
            }

            var bcStr = string.Join(", ", broadcast);
            TShock.Utils.Broadcast("Bosses selected: " + bcStr, Color.Crimson);

            if (AutoBoss.config.MinionToggles["night"])
            {
                BossTimer.minionTime = R.Next(AutoBoss.config.MinionsSpawnTimer[0],
                    AutoBoss.config.MinionsSpawnTimer[1] + 1);

                BossTimer.minionSpawnCount = R.Next(AutoBoss.config.MinionSpawnCount[0],
                    AutoBoss.config.MinionSpawnCount[1] + 1);

                StartMinionSpawns(BossTimer.minionSpawnCount, SelectMinions("night"));
            }
        }
        #endregion

        public static void StartMinionSpawns(int count, IEnumerable<int> types)
        {
            var broadcast = new List<string>();
            foreach (var minion in types)
            {
                var npcId = -1;
                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width / 2);
                    var arenaY = region.Area.Y + (region.Area.Height / 2);

                    for (var i = 0; i < count; i++)
                    {
                        npcId = NPC.NewNPC(arenaX*16, arenaY*16, minion);
                        AutoBoss.minionList.AddSafe(npcId, minion);
                    }
                }
                if (AutoBoss.config.AnnounceMinions)
                {
                    var npc = Main.npc[npcId];
                    broadcast.Add(string.Format("{0}x {1}", count*AutoBoss.ActiveArenas.Count, npc.name));
                }
            }
            if (AutoBoss.config.AnnounceMinions)
                TShock.Utils.Broadcast("Minions selected: " + string.Join(", ", broadcast),
                    Color.Crimson);
        }


        public static IEnumerable<int> SelectMinions(string type)
        {
            bool day = (type == "day"), night = (type == "night"), special = (type == "special");

            var ret = new List<int>();
            for (var i = 0; i < BossTimer.minionSpawnCount; i++)
            {
                if (day)
                    ret.Add(R.Next(0, AutoBoss.config.DayMinionList.Count));
                if (night)
                    ret.Add(R.Next(0, AutoBoss.config.NightMinionList.Count));
                if (special)
                    ret.Add(R.Next(0, AutoBoss.config.SpecialMinionList.Count));
            }

            return ret;
        }
    }
}
