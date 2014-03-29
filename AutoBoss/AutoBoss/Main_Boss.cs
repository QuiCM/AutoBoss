using System;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TerrariaApi;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using System.Reflection;

namespace AutoBoss
{
    [ApiVersion(1, 15)]
    public class AutoBoss : TerrariaPlugin
    {
        public static BossTools Tools;
        public static BossTimer Timers;
        public static BossConfig bossConfig = new BossConfig();

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
            Tools = new BossTools();
            Timers = new BossTimer();

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.GamePostInitialize.Register(this, AutoBoss.Tools.PostInitialize);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
                ServerApi.Hooks.GamePostInitialize.Deregister(this, AutoBoss.Tools.PostInitialize);
            }
            base.Dispose(disposing);
        }

        public AutoBoss(Main game)
            : base(game) { }
        #endregion

        #region OnInitialize
        public void OnInitialize(EventArgs args)
        {

            Commands.ChatCommands.Add(new Command("boss.root", BossCommands.BossCommand, "boss")
                {
                    HelpText = "Toggles automatic boss spawns; Reloads the configuration; Lists bosses and minions spawned by the plugin"
                });

            string configPath = Path.Combine(TShock.SavePath, "BossConfig.json");
            (bossConfig = BossConfig.Read(configPath)).Write(configPath);


            Timers.bossTimer.Elapsed += new ElapsedEventHandler(Timers.bossTimerElapsed);

            Timers.minionTimer.Elapsed += new ElapsedEventHandler(Timers.MinionElapsedEvent);
        }
        #endregion

        #region NetGreetPlayer
        public void OnGreet(GreetPlayerEventArgs args)
        {
            if (TShock.Players[args.Who] != null)
            {
                if (AutoBoss.bossConfig.AutoStartEnabled)
                    if (TShock.Players[0].Index == args.Who && TShock.Players.Length < 2)
                        if (!Timers.bossTimer.Enabled)
                            Timers.bossTimer.Enabled = true;
            }
        }
        #endregion
    }
}
