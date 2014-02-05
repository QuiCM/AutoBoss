using Terraria;
using System;
using System.IO;
using System.Collections.Generic;
using TShockAPI;

using TerrariaApi;
using TerrariaApi.Server;

namespace AutoBossSpawner
{
    [ApiVersion(1, 14)]
    public class AutoBossSpawner : TerrariaPlugin
    {
        private DateTime LastCheck = DateTime.UtcNow;
        private DateTime OtherLastCheck = DateTime.UtcNow;
        public ABSconfig configObj { get; set; }
        internal static string ABSconfigPath { get { return Path.Combine(TShock.SavePath, "ABSconfig.json"); } }
        private TShockAPI.DB.Region arenaregion = new TShockAPI.DB.Region();
        private int BossTimer = 30;
        private List<Terraria.NPC> bossList = new List<Terraria.NPC>();
        private bool BossToggle = false;
        private int BossForce = -1;
        Random rndGen = new Random();


        public override string Name
        {
            get { return "Auto Boss Spawner"; }
        }
        public override string Author
        {
            get { return "by InanZen"; }
        }
        public override string Description
        {
            get { return "Auto spawn bosses every night"; }
        }
        public override Version Version
        {
            get { return new Version("1.3"); }
        }
        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }
        public AutoBossSpawner(Main game)
            : base(game)
        {
            Order = 5;
        }
        public void OnUpdate(EventArgs e)
        {
            if (BossToggle && ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1))
            {
                LastCheck = DateTime.UtcNow;
                if (!Main.dayTime && BossTimer < 601)
                {
                    if (BossTimer == configObj.BossTimer)
                    {
                        if (configObj.BossText.Length > 1) { TShockAPI.TShock.Utils.Broadcast(configObj.BossText, Color.Aquamarine); }
                    }
                    else if (BossTimer == 10)
                    {
                        if (configObj.BossText10s.Length > 1) { TShockAPI.TShock.Utils.Broadcast(configObj.BossText10s, Color.Aquamarine); }
                    }
                    else if (BossTimer == 0)
                    {
                        if (configObj.BossText0s.Length > 1) { TShockAPI.TShock.Utils.Broadcast(configObj.BossText0s, Color.Aquamarine); }
                        startBossBattle();
                    }
                    else if ((BossTimer < 0) && (BossTimer % configObj.MinionsTimer == 0))
                    {
                        bool bossActive = false;
                        for (int i = 0; i < bossList.Count; i++)
                        {
                            if (bossList[i].active) bossActive = true;
                        }
                        if (bossActive) spawnMinions();
                        else
                        {
                            if (configObj.BossDefeat.Length > 1) { TShockAPI.TShock.Utils.Broadcast(configObj.BossDefeat, Color.Aquamarine); }
                            if (configObj.BossContinuous)
                                BossTimer = configObj.BossTimer + 1;
                            else
                                BossTimer = 601;
                            for (int i = 0; i < Main.npc.Length; i++)
                            {
                                if (Main.npc[i].type == 70 || Main.npc[i].type == 72)
                                {
                                    if (arenaregion.Area.Intersects(new Rectangle(((int)Main.npc[i].position.X) / 16, ((int)Main.npc[i].position.Y) / 16, 20, 20)))
                                    {
                                        TSPlayer.Server.StrikeNPC(i, 9999, 90f, 1);
                                    }
                                }
                            }
                        }
                    }
                    BossTimer--;
                }
                else if (BossTimer != configObj.BossTimer && Main.dayTime)
                {
                    BossTimer = configObj.BossTimer;
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (Main.npc[i].type == 70 || Main.npc[i].type == 72)
                        {
                            if (arenaregion.Area.Intersects(new Rectangle(((int)Main.npc[i].position.X) / 16, ((int)Main.npc[i].position.Y) / 16, 20, 20)))
                            {
                                TSPlayer.Server.StrikeNPC(i, 9999, 90f, 1);
                            }
                        }
                    }
                }
            }
        }

        private void startBossBattle()
        {
            NPC npc = new NPC();
            arenaregion = TShock.Regions.GetRegionByName("arena");
            int arenaX = arenaregion.Area.X + (arenaregion.Area.Width / 2);
            int arenaY = arenaregion.Area.Y + (arenaregion.Area.Height / 2);
            string broadcastString = "Boss selected:";
            BossSet bossSet;
            if (BossForce > -1)
            {
                bossSet = getBossByID(BossForce);
                BossForce = -1;
            }
            else
                bossSet = configObj.BossList[rndGen.Next(0, configObj.BossList.Count)];
            foreach (BossObj b in bossSet.bosses)
            {
                npc = TShockAPI.TShock.Utils.GetNPCById(b.id);
                TSPlayer.Server.SpawnNPC(npc.type, npc.name, b.amt, arenaX, arenaY, (arenaregion.Area.Width / 2), (arenaregion.Area.Height / 2));
                broadcastString += " " + b.amt + "x " + npc.name + " +";
            }
            broadcastString = broadcastString.Remove(broadcastString.Length - 2);
            TShockAPI.TShock.Utils.Broadcast(broadcastString, Color.Aquamarine);
            bossList = new List<Terraria.NPC>();
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].boss && Main.npc[i].active && Main.npc[i].type != 113) bossList.Add(Main.npc[i]);
            }
        }
        private void spawnMinions()
        {
            //TODO: num of henchmen based on players in arena, spawn life when boss is at half health.
            NPC npc = new NPC();
            npc = TShockAPI.TShock.Utils.GetNPCById(configObj.MinionsList[rndGen.Next(0, configObj.MinionsList.Length)]);
            arenaregion = TShock.Regions.GetRegionByName("arena");
            int arenaX = arenaregion.Area.X + (arenaregion.Area.Width / 2);
            int arenaY = arenaregion.Area.Y + (arenaregion.Area.Height / 2);
            int henchmenNumber = rndGen.Next(configObj.MinionsMinMax[0], configObj.MinionsMinMax[1] + 1);
            TSPlayer.Server.SpawnNPC(npc.type, npc.name, henchmenNumber, arenaX, arenaY, (arenaregion.Area.Width / 2), (arenaregion.Area.Height / 2));
            if (configObj.MinionsAnnounce) { TShockAPI.TShock.Utils.Broadcast("Spawning Boss Minions: " + henchmenNumber + "x " + npc.name + "!", Color.SteelBlue); }
        }

        #region Commands
        public void OnInitialize(EventArgs e)
        {
            configObj = new ABSconfig();
            SetupConfig();

            Commands.ChatCommands.Add(new Command("autoboss", ABScommand, "abs"));
        }
        public BossSet getBossByID(int id)
        {
            foreach (BossSet bossSet in configObj.BossList)
            {
                if (bossSet.setID == id) return bossSet;
            }
            return configObj.BossList[0];
        }

        public void ABScommand(CommandArgs args)
        {
            string cmd = "";
            if (args.Parameters.Count > 0)
            {
                cmd = args.Parameters[0].ToLower();
            }
            switch (cmd)
            {
                case "toggle":
                    {
                        BossToggle = !BossToggle;
                        if (BossToggle == true)
                        {
                            foreach (TShockAPI.DB.Region reg in TShock.Regions.ListAllRegions(Main.worldID.ToString()))
                            {
                                if (reg.Name == "arena") { arenaregion = reg; }
                            }
                            if (arenaregion.Name != "arena") { TShockAPI.TShock.Utils.Broadcast("Error: Region 'arena' is not defined.", Color.Red); BossToggle = false; }
                        }
                        args.Player.SendMessage("Boss battles now: " + ((BossToggle) ? "Enabled" : "Disabled"), Color.Aquamarine);
                        BossTimer = configObj.BossTimer;
                        break;
                    }
                case "reload":
                    {
                        SetupConfig();
                        args.Player.SendMessage("AutoBossSpawner config reloaded.", Color.Aquamarine);
                        break;
                    }
                case "force":
                    {
                        if (args.Parameters.Count > 1)
                        {
                            int forceID = -1;
                            int.TryParse(args.Parameters[1], out forceID);
                            if (forceID > -1)
                            {
                                args.Player.SendMessage("Forcing BossSet " + forceID, Color.Aquamarine);
                                BossForce = forceID;
                                BossTimer = 1;
                                if (Main.dayTime)
                                    TSPlayer.Server.SetTime(false, 0.0);
                            }
                            else
                                args.Player.SendMessage("Invalid syntax! Use: /abs force SetID", Color.Red);
                        }
                        else
                            args.Player.SendMessage("Invalid syntax! Use: /abs force SetID", Color.Red);
                        break;
                    }
                default:
                    {
                        args.Player.SendMessage("Available commands:", Color.Aquamarine);
                        args.Player.SendMessage("/abs toggle - enables/disables automatic boss battles", Color.Aquamarine);
                        args.Player.SendMessage("/abs force bossID - forces the start of bossID battle:", Color.Aquamarine);
                        args.Player.SendMessage("/abs reload - reloads the ABSconfig.json", Color.Aquamarine);
                        break;
                    }
            }
        }

        #endregion
        public void SetupConfig()
        {
            try
            {
                if (File.Exists(ABSconfigPath))
                {
                    configObj = new ABSconfig();
                    configObj = ABSconfig.Read(ABSconfigPath);
                    BossTimer = configObj.BossTimer;
                    BossToggle = configObj.BossStartEnabled;
                }
                else { configObj.Write(ABSconfigPath); }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error in config file");
                Console.ForegroundColor = ConsoleColor.Gray;
                Log.Error("Config Exception");
                Log.Error(ex.ToString());
            }
        }
    }

}