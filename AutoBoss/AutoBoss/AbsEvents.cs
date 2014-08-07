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

        public static void StartBossBattle(string type)
        {
            var bossLists = new List<Dictionary<int, int>>();
            var bossCounter = new Dictionary<string, int>();

            switch (type)
            {
                case "day":
                    bossLists = AutoBoss.config.DayBosses.Values.ToList();
                    break;
                case "night":
                    bossLists = AutoBoss.config.NightBosses.Values.ToList();
                    break;
                case "special":
                    bossLists = AutoBoss.config.SpecialBosses.Values.ToList();
                    break;
            }

            var bosses = bossLists[R.Next(0, bossLists.Count)];
            AutoBoss.bossList.Clear();
            AutoBoss.bossCounts.Clear();

            foreach (var boss in bosses)
            {
                var npc = TShock.Utils.GetNPCById(boss.Key);

                if (bossCounter.ContainsKey(npc.name))
                    bossCounter[npc.name]++;
                else
                    bossCounter.Add(npc.name, 1);

                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width/2);
                    var arenaY = region.Area.Y + (region.Area.Height/2);

                    AutoBoss.bossCounts.Add(npc.name, boss.Value);
                    int spawnTileX;
                    int spawnTileY;
                    TShock.Utils.GetRandomClearTileWithInRange(arenaX, arenaY, 50, 20, out spawnTileX, out spawnTileY);

                    var npcid = NPC.NewNPC(spawnTileX*16, spawnTileY*16, boss.Key);
                    // This is for special slimes
                    Main.npc[npcid].SetDefaults(npc.name);

                    AutoBoss.bossList.Add(npcid, boss.Key);
                }
            }

            var broadcast =
                bossCounter.Select(kvp => string.Format("{0}x {1}", kvp.Value * AutoBoss.ActiveArenas.Count, kvp.Key))
                    .ToList();

            TShock.Utils.Broadcast("Bosses selected: " + string.Join(", ", broadcast), Color.Crimson);

            if (AutoBoss.config.MinionToggles[type])
            {
                BossTimer.minionTime = R.Next(AutoBoss.config.MinionsSpawnTimer[0],
                    AutoBoss.config.MinionsSpawnTimer[1] + 1);

                BossTimer.minionSpawnCount = R.Next(AutoBoss.config.MinionSpawnCount[0],
                    AutoBoss.config.MinionSpawnCount[1] + 1);

                StartMinionSpawns(SelectMinions(type));
            }
        }

        public static void StartMinionSpawns(IEnumerable<int> types)
        {
            var minionCounter = new Dictionary<string, int>();

            foreach (var minion in types)
            {
                var npc = TShock.Utils.GetNPCById(minion);

                //if (npc.name == string.Empty)
                //    continue;

                if (minionCounter.ContainsKey(npc.name))
                    minionCounter[npc.name]++;
                else
                    minionCounter.Add(npc.name, 1);

                foreach (var region in AutoBoss.ActiveArenas)
                {
                    var arenaX = region.Area.X + (region.Area.Width/2);
                    var arenaY = region.Area.Y + (region.Area.Height/2);

                    TSPlayer.Server.SpawnNPC(minion, npc.name, 1, arenaX, arenaY, 50, 20);
                }
            }
            if (!AutoBoss.config.AnnounceMinions) return;

            var broadcast =
                minionCounter.Select(kvp => string.Format("{0}x {1}", kvp.Value*AutoBoss.ActiveArenas.Count, kvp.Key))
                    .ToList();

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
                    ret.AddCheck(AutoBoss.config.DayMinionList[R.Next(0, AutoBoss.config.DayMinionList.Count)]);
                if (night)
                    ret.AddCheck(AutoBoss.config.NightMinionList[R.Next(0, AutoBoss.config.NightMinionList.Count)]);
                if (special)
                    ret.AddCheck(AutoBoss.config.SpecialMinionList[R.Next(0, AutoBoss.config.SpecialMinionList.Count)]);
            }

            return ret;
        }
    }
}
