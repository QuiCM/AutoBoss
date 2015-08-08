using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace AutoBoss
{
	public class Config
	{
		public bool AutoStartEnabled;

		public bool ContinuousBoss;

		public bool OneWave;

		public int MessageInterval = 10;

		public bool EnableDayTimerText;

		public string[] DayTimerText =
		{
			"[Day] Initial message",
			"[Day] Secondary message",
			"[Day] Third message",
			"[Day] Etc",
			"[Day] Bosses spawning"
		};

		public string DayTimerFinished = "Boss battle complete.";

		public bool EnableNightTimerText;

		public string[] NightTimerText =
		{
			"[Night] Initial message",
			"[Night] Secondary message",
			"[Night] Third message",
			"[Night] Etc",
			"[Night] Bosses spawning"
		};

		public string NightTimerFinished = "Boss battle complete.";

		public bool EnableSpecialTimerText;

		public string[] SpecialTimerText =
		{
			"[Special] Initial message",
			"[Special] Secondary message",
			"[Special] Third message",
			"[Special] Etc",
			"[Special] Bosses spawning"
		};

		public string SpecialTimerFinished = "Boss battle complete.";

		public bool AnnounceMinions;
		public int[] MinionsSpawnTimer = {10, 30};

		public Dictionary<string, bool> BossArenas;

		public Dictionary<BattleType, bool> BossToggles = new Dictionary<BattleType, bool>
		{
			{BattleType.Day, false},
			{BattleType.Night, false},
			{BattleType.Special, false}
		};

		public Dictionary<BattleType, bool> MinionToggles = new Dictionary<BattleType, bool>
		{
			{BattleType.Day, false},
			{BattleType.Night, false},
			{BattleType.Special, false}
		};


		public int[] MinionSpawnCount = {2, 5};
		public List<int> DayMinionList;
		public List<int> NightMinionList;
		public List<int> SpecialMinionList;


		public Dictionary<string, Dictionary<int, int>> DayBosses;
		public Dictionary<string, Dictionary<int, int>> NightBosses;
		public Dictionary<string, Dictionary<int, int>> SpecialBosses;

		public void Write(string path)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public static Config Read(string path)
		{
			return !File.Exists(path)
				? new Config()
				: JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
		}
	}
}