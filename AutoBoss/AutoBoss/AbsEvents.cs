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
                        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, item.Key);
                }

                if (npcId == -1) continue;
                var npc = Main.npc[npcId];
                broadcast.Add(string.Format("{0}x {1}", item.Value*AutoBoss.ActiveArenas.Count, npc.name));
                AutoBoss.bossCounts.Add(npc.name, item.Value);
                AutoBoss.bossList.Add(npcId, item.Key);
            }

            if (AutoBoss.config.MinionToggles["day"])
            {
                BossTimer.minionTime = R.Next(AutoBoss.config.MinionsSpawnTimer[0],
                    AutoBoss.config.MinionsSpawnTimer[1] + 1);

                BossTimer.minionSpawnCount = R.Next(AutoBoss.config.MinionSpawnCount[0],
                    AutoBoss.config.MinionSpawnCount[1] + 1);

                StartMinionSpawns(SelectMinions("day"));
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
            AutoBoss.bossCounts.Clear();

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

                if (npcId == -1) continue;
                var npc = Main.npc[npcId];
                broadcast.Add(string.Format("{0}x {1}", item.Value * AutoBoss.ActiveArenas.Count, npc.name));
                AutoBoss.bossCounts.Add(npc.name, item.Value);
                AutoBoss.bossList.Add(npcId, item.Key);
            }

            var bcStr = string.Join(", ", broadcast);
            TShock.Utils.Broadcast("Bosses selected: " + bcStr, Color.Crimson);

            if (AutoBoss.config.MinionToggles["special"])
            {
                BossTimer.minionTime = R.Next(AutoBoss.config.MinionsSpawnTimer[0],
                    AutoBoss.config.MinionsSpawnTimer[1] + 1);

                BossTimer.minionSpawnCount = R.Next(AutoBoss.config.MinionSpawnCount[0],
                    AutoBoss.config.MinionSpawnCount[1] + 1);

                StartMinionSpawns(SelectMinions("special"));
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
                        npcId = NPC.NewNPC(arenaX * 16, arenaY * 16, item.Key);
                }

                if (npcId == -1) continue;
                var npc = Main.npc[npcId];
                broadcast.Add(string.Format("{0}x {1}", item.Value * AutoBoss.ActiveArenas.Count, npc.name));
                AutoBoss.bossCounts.Add(npc.name, item.Value);
                AutoBoss.bossList.Add(npcId, item.Key);
            }

            var bcStr = string.Join(", ", broadcast);
            TShock.Utils.Broadcast("Bosses selected: " + bcStr, Color.Crimson);

            if (AutoBoss.config.MinionToggles["night"])
            {
                BossTimer.minionTime = R.Next(AutoBoss.config.MinionsSpawnTimer[0],
                    AutoBoss.config.MinionsSpawnTimer[1] + 1);

                BossTimer.minionSpawnCount = R.Next(AutoBoss.config.MinionSpawnCount[0],
                    AutoBoss.config.MinionSpawnCount[1] + 1);

                StartMinionSpawns(SelectMinions("night"));
            }
        }
        #endregion

        public static void StartMinionSpawns(IEnumerable<int> types)
        {
            var broadcast = new List<string>();
            //                              Type  Count, Main.npc position
            var minionCounter = new Dictionary<int, int[]>();
            foreach (var minion in types)
            {
                var npcId = -1;
                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width/2);
                    var arenaY = region.Area.Y + (region.Area.Height/2);

                    var offsetX = R.Next(0, 25);
                    var offsetY = R.Next(0, 25);
                    npcId = NPC.NewNPC((arenaX*16) + offsetX, (arenaY*16) + offsetY, minion);
                }
                if (!minionCounter.ContainsKey(minion))
                    minionCounter.Add(minion, new[] {1, npcId});
                else
                    minionCounter[minion][0]++;
            }
            if (!AutoBoss.config.AnnounceMinions) return;

            broadcast.AddRange(from kvp in minionCounter
                where kvp.Value.Length == 2
                where kvp.Value[1] >= 0 && kvp.Value[1] <= Main.maxNPCs
                select
                    string.Format("{0}x {1}", kvp.Value[0]*AutoBoss.ActiveArenas.Count, Main.npc[kvp.Value[1]].name));

            TShock.Utils.Broadcast("Minions selected: " + string.Join(", ", broadcast), Color.Crimson);
        }


        public static IEnumerable<int> SelectMinions(string type)
        {
            bool day = false, night = false, special = false;
            switch (type)
            {
                case "day":
                    day = true;
                    break;
                case "night":
                    night = true;
                    break;
                case "special":
                    special = true;
                    break;
            }

            var ret = new List<int>();
            for (var i = 0; i < BossTimer.minionSpawnCount; i++)
            {
                if (day)
                    ret.AddCheck(R.Next(0, AutoBoss.config.DayMinionList.Count));
                if (night)
                    ret.AddCheck(R.Next(0, AutoBoss.config.NightMinionList.Count));
                if (special)
                    ret.AddCheck(R.Next(0, AutoBoss.config.SpecialMinionList.Count));
            }

            return ret;
        }
    }
}
