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

    public class BossTimer
    {
        public BossTimer() { }

        public Timer bossTimer = new Timer(AutoBoss.bossConfig.MessageInterval * 1000);

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

            if (Main.dayTime && dayBossEnabled)
            {
                ticker.type = "day";
                ticker.maxCount = AutoBoss.bossConfig.DayTimerText.Length - 1;
                ticker.enabled = true;
            }
            if (!Main.dayTime && !Main.raining && !Main.bloodMoon && !Main.eclipse && !Main.pumpkinMoon &&
                !Main.snowMoon && Main.invasionType == 0 && nightBossEnabled)
            {
                ticker.type = "night";
                ticker.maxCount = AutoBoss.bossConfig.NightTimerText.Length - 1;
                ticker.enabled = true;
            }

            if (Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                            Main.snowMoon || Main.invasionType > 0 && specialBossEnabled)
            {
                ticker.type = "special";
                ticker.maxCount = AutoBoss.bossConfig.SpecialTimerText.Length - 1;
                ticker.enabled = true;
            }

            if (ticker.enabled)
            {
                if (!bossActive)
                {
                    ticker.count++;

                    if (ticker.type == "day" && Main.dayTime)
                    {
                        if (AutoBoss.bossConfig.EnableDayTimerText)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(AutoBoss.bossConfig.DayTimerText[ticker.count],
                                    Color.YellowGreen);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(AutoBoss.bossConfig.DayTimerText[ticker.maxCount],
                                    Color.Crimson);

                                if (dayMinionEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                BossEvents.startBossBattleDay();

                                if (AutoBoss.bossConfig.ContinuousBoss)
                                    ticker.count = -1;
                                else
                                {
                                    TSPlayer.All.SendMessage(AutoBoss.bossConfig.DayTimerFinished,
                                        Color.LightBlue);
                                    bossTimer.Enabled = false;
                                }
                            }
                        }
                    }
                    if (ticker.type == "night" && !Main.dayTime && !Main.raining && !Main.bloodMoon && 
                        !Main.eclipse && !Main.pumpkinMoon && !Main.snowMoon && Main.invasionType < 1)
                    {
                        if (AutoBoss.bossConfig.EnableNightTimerText)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(AutoBoss.bossConfig.NightTimerText[ticker.count],
                                    Color.DarkMagenta);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(AutoBoss.bossConfig.NightTimerText[ticker.maxCount],
                                   Color.Crimson);

                                if (nightBossEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                BossEvents.startBossBattleNight();
                                if (AutoBoss.bossConfig.ContinuousBoss)
                                    ticker.count = -1;
                                else
                                {
                                    TSPlayer.All.SendMessage(AutoBoss.bossConfig.NightTimerFinished,
                                        Color.LightBlue);
                                    bossTimer.Enabled = false;
                                }
                            }
                        }
                    }
                    if (ticker.type == "special" && Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                            Main.snowMoon || Main.invasionType > 0)
                    {
                        if (AutoBoss.bossConfig.EnableSpecialTimerText)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(AutoBoss.bossConfig.SpecialTimerText[ticker.count],
                                   Color.Orange);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(AutoBoss.bossConfig.SpecialTimerText[ticker.maxCount],
                                    Color.Crimson);

                                if (specialMinionEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                BossEvents.startBossBattleSpecial();
                                if (AutoBoss.bossConfig.ContinuousBoss)
                                    ticker.count = -1;
                                else
                                {
                                    bossTimer.Enabled = false;
                                    TSPlayer.All.SendMessage(AutoBoss.bossConfig.SpecialTimerFinished,
                                        Color.LightBlue);
                                }
                            }
                        }
                    }
                }
            }
        }


        public Timer minionTimer = new Timer(new Random().Next(AutoBoss.bossConfig.MinionsSpawnTimer[0], 
            AutoBoss.bossConfig.MinionsSpawnTimer[1]) * 1000);

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
    }
}