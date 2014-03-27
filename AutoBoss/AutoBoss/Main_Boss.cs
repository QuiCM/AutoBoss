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
        public static BossTools Tools = BossTools.Instance;
        public static BossTimer Timer = BossTimer.Instance;

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
            : base(game)
        {
            Order = -10;
            Tools.bossConfig = new BossConfig();
        }
        #endregion

        #region OnInitialize
        public void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("boss.root", BossCommands.BossCommand, "boss")
                {
                    HelpText = "Toggles automatic boss spawns; Reloads the configuration; Lists bosses and minions spawned by the plugin"
                });

            (Tools.bossConfig = BossConfig.Read(Tools.configPath)).Write(Tools.configPath);


            Timer.bossTimer.Elapsed += new ElapsedEventHandler(Timer.bossTimerElapsed);

            Timer.minionTimer.Elapsed += new ElapsedEventHandler(Timer.MinionElapsedEvent);
        }
        #endregion

        #region NetGreetPlayer
        public void OnGreet(GreetPlayerEventArgs args)
        {
            if (TShock.Players[args.Who] != null)
            {
                if (AutoBoss.Tools.bossConfig.AutoStartEnabled)
                    if (TShock.Players[0].Index == args.Who && TShock.Players.Length < 2)
                        if (!Timer.bossTimer.Enabled)
                            Timer.bossTimer.Enabled = true;
            }
        }
        #endregion
    }
}
