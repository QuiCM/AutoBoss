using System;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Generic;

using Terraria;
using TShockAPI;

namespace Auto_Boss
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

    public class Boss_Timer
    {
        public static Timer boss_Timer  = new Timer(Boss_Tools.boss_Config.Message_Interval * 1000);

        public static bool dayBossEnabled = false;
        public static bool nightBossEnabled = false;
        public static bool specialBossEnabled = false;

        public static bool dayMinionEnabled = false;
        public static bool nightMinionEnabled = false;
        public static bool specialMinionEnabled = false;

        public static bool MinionTimerRunning = false;

        public static bool bossesActive = false;

        internal static timerObj ticker = new timerObj(-1, "day", false, 10);


        public static void boss_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool bossActive = false;
            foreach (KeyValuePair<int, int> pair in Boss_Tools.boss_List)
                if (Main.npc[pair.Key].type == pair.Value && Main.npc[pair.Key].active)
                    bossActive = true;


            bossesActive = bossActive;

            if (!Boss_Tools.Bosses_Toggled)
            {
                Log.ConsoleInfo("[AutoBoss+] Timer Disabled: Boss toggle disabled");
                boss_Timer.Enabled = false;
                Boss_Tools.Bosses_Toggled = false;
                ticker.count = -1;
                return;
            }

            Console.WriteLine(TShock.Players.Count());

            if (TShock.Players[0] == null)
            {
                Log.ConsoleInfo("[AutoBoss+] Timer Disabled: No players online");
                boss_Timer.Enabled = false;
                Boss_Tools.Bosses_Toggled = false;
                ticker.count = -1;
                return;
            }

            if (Main.dayTime && dayBossEnabled)
            {
                ticker.type = "day";
                ticker.maxCount = Boss_Tools.boss_Config.DayTimer_Text.Length - 1;
                ticker.enabled = true;
            }
            if (!Main.dayTime && !Main.raining && !Main.bloodMoon && !Main.eclipse && !Main.pumpkinMoon &&
                !Main.snowMoon && Main.invasionType == 0 && nightBossEnabled)
            {
                ticker.type = "night";
                ticker.maxCount = Boss_Tools.boss_Config.NightTimer_Text.Length - 1;
                ticker.enabled = true;
            }

            if (Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                            Main.snowMoon || Main.invasionType > 0 && specialBossEnabled)
            {
                ticker.type = "special";
                ticker.maxCount = Boss_Tools.boss_Config.SpecialTimer_Text.Length - 1;
                ticker.enabled = true;
            }

            if (ticker.enabled)
            {
                if (!bossActive)
                {
                    ticker.count++;

                    if (ticker.type == "day")
                    {
                        if (Boss_Tools.boss_Config.Enable_DayTimer_Text)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(Boss_Tools.boss_Config.DayTimer_Text[ticker.count],
                                    Color.YellowGreen);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(Boss_Tools.boss_Config.DayTimer_Text[ticker.maxCount],
                                    Color.Crimson);

                                if (dayMinionEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                Boss_Events.start_BossBattle_Day();

                                if (Boss_Tools.boss_Config.Continuous_Boss)
                                    ticker.count = -1;
                                else
                                {
                                    TSPlayer.All.SendMessage(Boss_Tools.boss_Config.DayTimer_Finished,
                                        Color.LightBlue);
                                    boss_Timer.Enabled = false;
                                }
                            }
                        }
                    }
                    if (ticker.type == "night")
                    {
                        if (Boss_Tools.boss_Config.Enable_NightTimer_Text)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(Boss_Tools.boss_Config.NightTimer_Text[ticker.count],
                                    Color.DarkMagenta);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(Boss_Tools.boss_Config.NightTimer_Text[ticker.maxCount],
                                   Color.Crimson);

                                if (nightBossEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                Boss_Events.start_BossBattle_Night();
                                if (Boss_Tools.boss_Config.Continuous_Boss)
                                    ticker.count = -1;
                                else
                                {
                                    TSPlayer.All.SendMessage(Boss_Tools.boss_Config.NightTimer_Finished,
                                        Color.LightBlue);
                                    boss_Timer.Enabled = false;
                                }
                            }
                        }
                    }
                    if (ticker.type == "special")
                    {
                        if (Boss_Tools.boss_Config.Enable_SpecialTimer_Text)
                        {
                            if (ticker.count != ticker.maxCount)
                                TSPlayer.All.SendMessage(Boss_Tools.boss_Config.SpecialTimer_Text[ticker.count],
                                   Color.Orange);

                            else if (ticker.count >= ticker.maxCount)
                            {
                                TSPlayer.All.SendMessage(Boss_Tools.boss_Config.SpecialTimer_Text[ticker.maxCount],
                                    Color.Crimson);

                                if (specialMinionEnabled)
                                    if (!MinionTimerRunning && !minionTimer.Enabled)
                                        minionTimer.Enabled = true;

                                Boss_Events.start_BossBattle_Special();
                                if (Boss_Tools.boss_Config.Continuous_Boss)
                                    ticker.count = -1;
                                else
                                {
                                    boss_Timer.Enabled = false;
                                    TSPlayer.All.SendMessage(Boss_Tools.boss_Config.SpecialTimer_Finished,
                                        Color.LightBlue);
                                }
                            }
                        }
                    }
                }
            }
        }


        public static Timer minionTimer = new Timer(new Random().Next(Boss_Tools.boss_Config.Minions_Spawn_Timer[0], 
            Boss_Tools.boss_Config.Minions_Spawn_Timer[1]) * 1000);

        //(Boss_Tools.boss_Config.Minions_Spawn_Timer[0] / (Boss_Tools.boss_Config.Minions_Spawn_Timer[1] * 1.0)

        public static void Minion_Elapsed_Event(object sender, ElapsedEventArgs args)
        {
            if (Main.dayTime && dayMinionEnabled && bossesActive)
                Boss_Events.start_DayMinion_Spawns();

            if (!Main.dayTime && !Main.raining && !Main.bloodMoon && !Main.eclipse && bossesActive
                && !Main.pumpkinMoon && !Main.snowMoon && Main.invasionType == 0 && nightMinionEnabled)
                Boss_Events.start_NightMinion_Spawns();

            if (Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                                    Main.snowMoon || Main.invasionSize > 0 && specialMinionEnabled && bossesActive)
                Boss_Events.start_SpecialMinion_Spawns();
        }
    }
}