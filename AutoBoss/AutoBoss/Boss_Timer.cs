using System;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Generic;

using Terraria;
using TShockAPI;

namespace AutoBoss
{
    public class timerObj
    {
        public int count;
        public int maxCount;
        public string type;
        public bool enabled;

        public timerObj(int c, string t, bool e, int m)
        {
            count = c;
            type = t;
            enabled = e;
            maxCount = m;
        }
    }

    public class BossTimer : IDisposable
    {
        private BossTimer() { }

        private static volatile BossTimer instance = null;

        public static BossTimer Instance
        {
            get
            {
                if (instance == null)
                    instance = new BossTimer();

                return instance;
            }
        }

        public Timer bossTimer  = new Timer(AutoBoss.Tools.bossConfig.MessageInterval * 1000);

        public bool dayBossEnabled = false;
        public bool nightBossEnabled = false;
        public bool specialBossEnabled = false;

        public bool dayMinionEnabled = false;
        public bool nightMinionEnabled = false;
        public bool specialMinionEnabled = false;

        public bool MinionTimerRunning = false;

        public bool bossesActive = false;

        internal timerObj ticker = new timerObj(-1, "day", false, 10);


        public void bossTimerElapsed(object sender, ElapsedEventArgs e)
        {
            bool bossActive = false;
            foreach (KeyValuePair<int, int> pair in AutoBoss.Tools.bossList)
                if (Main.npc[pair.Key].type == pair.Value && Main.npc[pair.Key].active)
                    bossActive = true;


            bossesActive = bossActive;

            if (!AutoBoss.Tools.BossesToggled)
            {
                Log.ConsoleInfo("[AutoBoss+] Timer Disabled: Boss toggle disabled");
                bossTimer.Enabled = false;
                AutoBoss.Tools.BossesToggled = false;
                ticker.count = -1;
                return;
            }

            //if (TShock.Players[0] == null)
            //{
            //    Log.ConsoleInfo("[AutoBoss+] Timer Disabled: No players online");
            //    bossTimer.Enabled = false;
            //    AutoBoss.Tools.BossesToggled = false;
            //    ticker.count = -1;
            //    return;
            //}

            if (Main.dayTime && dayBossEnabled)
            {
                ticker.type = "day";
                ticker.maxCount = AutoBoss.Tools.bossConfig.DayTimerText.Length - 1;
                ticker.enabled = true;
            }
            if (!Main.dayTime && !Main.raining && !Main.bloodMoon && !Main.eclipse && !Main.pumpkinMoon &&
                !Main.snowMoon && Main.invasionType == 0 && nightBossEnabled)
            {
                ticker.type = "night";
                ticker.maxCount = AutoBoss.Tools.bossConfig.NightTimerText.Length - 1;
                ticker.enabled = true;
            }

            if (Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                            Main.snowMoon || Main.invasionType > 0 && specialBossEnabled)
            {
                ticker.type = "special";
                ticker.maxCount = AutoBoss.Tools.bossConfig.SpecialTimerText.Length - 1;
                ticker.enabled = true;
            }

            if (ticker.enabled)
            {
                if (!bossActive)
                {
                    ticker.count++;

                    if (ticker.type == "day" && Main.dayTime)
                    {
                        if (AutoBoss.Tools.bossConfig.EnableDayTimerText)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.DayTimerText[ticker.count],
                                    Color.YellowGreen);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.DayTimerText[ticker.maxCount],
                                    Color.Crimson);

                                if (dayMinionEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                BossEvents.startBossBattleDay();

                                if (AutoBoss.Tools.bossConfig.ContinuousBoss)
                                    ticker.count = -1;
                                else
                                {
                                    TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.DayTimerFinished,
                                        Color.LightBlue);
                                    bossTimer.Enabled = false;
                                }
                            }
                        }
                    }
                    if (ticker.type == "night" && !Main.dayTime && !Main.raining && !Main.bloodMoon && 
                        !Main.eclipse && !Main.pumpkinMoon && !Main.snowMoon && Main.invasionType < 1)
                    {
                        if (AutoBoss.Tools.bossConfig.EnableNightTimerText)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.NightTimerText[ticker.count],
                                    Color.DarkMagenta);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.NightTimerText[ticker.maxCount],
                                   Color.Crimson);

                                if (nightBossEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                BossEvents.startBossBattleNight();
                                if (AutoBoss.Tools.bossConfig.ContinuousBoss)
                                    ticker.count = -1;
                                else
                                {
                                    TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.NightTimerFinished,
                                        Color.LightBlue);
                                    bossTimer.Enabled = false;
                                }
                            }
                        }
                    }
                    if (ticker.type == "special" && Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                            Main.snowMoon || Main.invasionType > 0)
                    {
                        if (AutoBoss.Tools.bossConfig.EnableSpecialTimerText)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.SpecialTimerText[ticker.count],
                                   Color.Orange);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.SpecialTimerText[ticker.maxCount],
                                    Color.Crimson);

                                if (specialMinionEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                BossEvents.startBossBattleSpecial();
                                if (AutoBoss.Tools.bossConfig.ContinuousBoss)
                                    ticker.count = -1;
                                else
                                {
                                    bossTimer.Enabled = false;
                                    TSPlayer.All.SendMessage(AutoBoss.Tools.bossConfig.SpecialTimerFinished,
                                        Color.LightBlue);
                                }
                            }
                        }
                    }
                }
            }
        }


        public Timer minionTimer = new Timer(new Random().Next(AutoBoss.Tools.bossConfig.MinionsSpawnTimer[0], 
            AutoBoss.Tools.bossConfig.MinionsSpawnTimer[1]) * 1000);

        //(AutoBoss.Tools.bossConfig.MinionsSpawnTimer[0] / (AutoBoss.Tools.bossConfig.MinionsSpawnTimer[1] * 1.0)

        public void MinionElapsedEvent(object sender, ElapsedEventArgs args)
        {
            if (Main.dayTime && dayMinionEnabled && bossesActive)
                BossEvents.startDayMinionSpawns();

            if (!Main.dayTime && !Main.raining && !Main.bloodMoon && !Main.eclipse && bossesActive
                && !Main.pumpkinMoon && !Main.snowMoon && Main.invasionType == 0 && nightMinionEnabled)
                BossEvents.startNightMinionSpawns();

            if (Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                                    Main.snowMoon || Main.invasionSize > 0 && specialMinionEnabled && bossesActive)
                BossEvents.startSpecialMinionSpawns();
        }

        public void Dispose()
        {
            Log.ConsoleInfo("[AutoBoss+] Timers disposed");
            bossTimer.Dispose();
            minionTimer.Dispose();
        }
    }
}