using System;
using System.IO;
using System.Collections.Generic;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using System.Reflection;

namespace AutoBoss
{
    [ApiVersion(1, 16)]
    public class AutoBoss : TerrariaPlugin
    {
        public static AbsTools Tools;
        public static BossTimer Timers;
        public static Config config = new Config();

        public static Dictionary<int, int> bossList = new Dictionary<int, int>();
        public static Dictionary<int, int> minionList = new Dictionary<int, int>();
        public static Dictionary<string, int> bossCounts = new Dictionary<string, int>();
        public static Dictionary<string, int> minionCounts = new Dictionary<string, int>(); 

        public static readonly List<Region> ActiveArenas = new List<Region>();

        #region TerrariaPlugin
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override string Author
        {
            get { return "WhiteX"; }
        }

        public override string Description
        {
            get { return "Automatic boss spawner"; }
        }

        public override string Name
        {
            get { return "AutoBoss+"; }
        }

        public override void Initialize()
        {
            Tools = new AbsTools();

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
            }
            base.Dispose(disposing);
        }

        public AutoBoss(Main game)
            : base(game) { }
        #endregion

        #region OnInitialize

        private static void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("boss.root", BossCommands.BossCommand, "boss")
                {
                    HelpText = "Toggles automatic boss spawns; Reloads the configuration; Lists bosses and minions spawned by the plugin"
                });

            var configPath = Path.Combine(TShock.SavePath, "BossConfig.json");
            (config = Config.Read(configPath)).Write(configPath);

            Timers = new BossTimer();
        }
        #endregion

        #region NetGreetPlayer

        private static void OnGreet(GreetPlayerEventArgs args)
        {
            if (TShock.Players[args.Who] != null)
            {
                if (config.AutoStartEnabled)
                    if (TShock.Utils.ActivePlayers() == 1)
                    {
                        Tools.ReloadConfig(true);
                        var day = config.BossToggles[BattleType.Day];
                        var night = config.BossToggles[BattleType.Night];
                        var special = config.BossToggles[BattleType.Special];
                        Tools.bossesToggled = true;
                        Timers.StartBosses(day, night, special, true);
                    }
            }
        }
        #endregion
    }
}
