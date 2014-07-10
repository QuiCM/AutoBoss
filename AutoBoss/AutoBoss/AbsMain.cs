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

        public static readonly List<Region> ActiveArenas = new List<Region>();
        public static readonly List<string> InactiveArenas = new List<string>();

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
            Timers = new BossTimer();

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.GamePostInitialize.Register(this, PostInitialize);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
                ServerApi.Hooks.GamePostInitialize.Deregister(this, PostInitialize);
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
        }
        #endregion

        #region PostInitialize

        private static void PostInitialize(EventArgs args)
        {
            Tools.ReloadConfig(true);

            var day = config.BossToggles["day"];
            var night = config.BossToggles["night"];
            var special = config.BossToggles["special"];

            if (config.AutoStartEnabled)
                Timers.StartBosses(day, night, special, true);
            else
                Timers.StartBosses(day, night, special);
        }
        #endregion

        #region NetGreetPlayer

        private static void OnGreet(GreetPlayerEventArgs args)
        {
            if (TShock.Players[args.Who] != null)
            {
                if (config.AutoStartEnabled)
                    if (TShock.Players[0].Index == args.Who && TShock.Players.Length < 2)
                        if (!Timers.bossTimer.Enabled)
                            Timers.bossTimer.Enabled = true;
            }
        }
        #endregion
    }
}
