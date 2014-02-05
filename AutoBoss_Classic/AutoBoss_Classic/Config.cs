using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AutoBossSpawner
{
    public class BossObj
    {
        public int id;
        public int amt;
        public BossObj(int i, int a)
        {
            id = i;
            amt = a;
        }
    }
    public class BossSet
    {
        public int setID = -1;
        public List<BossObj> bosses;
        public BossSet(int id, List<BossObj> bosses)
        {
            setID = id;
            this.bosses = bosses;
        }
    }
    public class ABSconfig
    {
        public bool BossStartEnabled = false;
        public int BossTimer = 30;
        public string BossText = "Nightly Boss battle in Arena starting in 30 seconds";
        public string BossText10s = "Nightly Boss battle in Arena starting in 10 seconds";
        public string BossText0s = "Boss battle in Arena has begun!";
        public string BossDefeat = "All Bosses have been defeated. That's it for tonight.";
        public bool BossContinuous = false;
        public bool MinionsAnnounce = true;
        public int MinionsTimer = 20;
        public int[] MinionsMinMax = { 10, 30 };
        public int[] MinionsList = { 2, 6, 16, 23, 24, 28, 29, 31, 32, 34, 42, 44, 45, 48, 59, 60, 62, 71, 75, 77, 78, 81, 82, 83, 84, 85, 86, 93, 104, 110, 111, 120, 121, 122, 133, 137, 138, 140, 141, 143, 144 };
        public List<BossSet> BossList;

        public static ABSconfig Read(string path)
        {
            if (!File.Exists(path))
                return new ABSconfig();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }

        public static ABSconfig Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<ABSconfig>(sr.ReadToEnd());
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
            BossList = new List<BossSet>();
            List<BossObj> bossSet = new List<BossObj>();
            bossSet.Add(new BossObj(4, 5));
            bossSet.Add(new BossObj(70, 5));
            BossList.Add(new BossSet(1, bossSet));

            bossSet = new List<BossObj>();
            bossSet.Add(new BossObj(134, 1));
            bossSet.Add(new BossObj(70, 5));
            BossList.Add(new BossSet(2, bossSet));

            bossSet = new List<BossObj>();
            bossSet.Add(new BossObj(125, 1));
            bossSet.Add(new BossObj(126, 1));
            bossSet.Add(new BossObj(70, 5));
            BossList.Add(new BossSet(3, bossSet));

            bossSet = new List<BossObj>();
            bossSet.Add(new BossObj(127, 1));
            bossSet.Add(new BossObj(70, 5));
            BossList.Add(new BossSet(4, bossSet));

            bossSet = new List<BossObj>();
            bossSet.Add(new BossObj(134, 2));
            BossList.Add(new BossSet(5, bossSet));

            bossSet = new List<BossObj>();
            bossSet.Add(new BossObj(50, 5));
            bossSet.Add(new BossObj(70, 5));
            BossList.Add(new BossSet(6, bossSet));

            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }
        public static Action<ABSconfig> ConfigRead;
    }
}