using System;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Generic;

using Terraria;
using TShockAPI;

namespace Auto_Boss
{
    /* This is a small class that I can store values inside of, that I can use later. */
    public class timerObj
    {
        /* Length is a numerical value that the timerObj has
         * type is a string value: "day", "night" or "special"
         * enabled is a boolean: true or false
         */

        public int length;
        public string type;
        public bool enabled;

        public timerObj(int l, string t, bool e)
        {
            length = l;
            type = t;
            enabled = e;
        }
    }

    public class Boss_Timer
    {
        public static Timer boss_Timer = new Timer(Boss_Tools.boss_Config.Message_Interval * 1000);
        public static bool dayBossEnabled = false;
        public static bool nightBossEnabled = false;
        public static bool specialBossEnabled = false;

        public static bool MinionTimerRunning = false;

        /* This dictionary is what allows the timer to function the way I want it to. 
         * The first timerObj is the one that counts up. It's maximum value is the text appended by .Length in the second
         * timerObj. For example:
         * Boss_Tools.boss_Config.NightTimer_Text is a string[]. In the default config it holds 8 values, so it's length is 8
         * This means that the timer can tick through it 8 - 1 (= 7) times (because of 0-based indexes)
         * When the value of the timerObj on the left reaches its maximum value, bosses are spawned.
         */
        internal static Dictionary<timerObj, timerObj> ticker = new Dictionary<timerObj, timerObj>()
        {
            {
                new timerObj(0, "night", false), new timerObj(Boss_Tools.boss_Config.NightTimer_Text.Length, "night", false)
            },
            {
                new timerObj(0, "day", false), new timerObj(Boss_Tools.boss_Config.DayTimer_Text.Length, "day", false)
            },
            {
                new timerObj(0, "special", false), new timerObj(Boss_Tools.boss_Config.SpecialTimer_Text.Length, "special", false)
            }
        };


        /* Starts the timer */
        public static void Run()
        {
            boss_Timer.Enabled = true;
            boss_Timer.Elapsed += boss_Timer_Elapsed;
        }

        /* What happens when the tick (Boss_Tools.boss_Config defined option)seconds passes */
        public static void boss_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            /* To stop the timer while bosses are still spawned. */
            if (Boss_Events.Bosses_Active)
            {
                foreach (NPC boss in Boss_Tools.boss_List)
                    if (!Main.npc.Contains(boss))
                        Boss_Tools.boss_List.Remove(boss);

                if (Boss_Tools.boss_List.Count < 1)
                {
                    Boss_Events.Bosses_Active = false;
                    EndBattle();
                    TSPlayer.All.SendInfoMessage("[AutoBoss+] debug: Battle ended due to boss list being empty");
                }
                return;
            }
            /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

            /* Disable the timer if the toggle is turned off */ 
            if (!Boss_Tools.Bosses_Toggled)
            {
                Log.ConsoleInfo("[AutoBoss+] Timer Disabled: Boss toggle disabled");
                boss_Timer.Enabled = false;
                boss_Timer.Elapsed -= boss_Timer_Elapsed;
                Boss_Tools.Bosses_Toggled = false;
                resetTicks();
                return;
            }
            /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

            /* Disable the timer if there are no players online */ 
            if (TShock.Players[0] == null && TShock.Players.Length == 0)
            {
                Log.ConsoleInfo("[AutoBoss+] Timer Disabled: No players online!");
                boss_Timer.Enabled = false;
                boss_Timer.Elapsed -= boss_Timer_Elapsed;
                Boss_Tools.Bosses_Toggled = false;
                resetTicks();
                return;
            }
            /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

            foreach (KeyValuePair<timerObj, timerObj> pair in ticker)
            {
                /* Enable the keys if it's the correct time for them */
                if (Main.dayTime && pair.Key.type == "day" && !pair.Key.enabled && dayBossEnabled)
                    pair.Key.enabled = true;

                if (!Main.dayTime && pair.Key.type == "night" && !pair.Key.enabled && nightBossEnabled)
                    pair.Key.enabled = true;

                if (Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon || Main.invasionSize > 0
                                    && pair.Key.type == "special" && !pair.Key.enabled && specialBossEnabled)
                    pair.Key.enabled = true;
                /* this allows the value of the key to count up */


                /* Check if the key is enabled,  before doing anything with it */
                if (pair.Key.enabled)
                {
                    /* -1 length means the battle has ended. */
                    if (pair.Key.length == -1)
                    {
                        EndBattle();
                        pair.Key.length++;
                    }
                    else
                        pair.Key.length++;

                    /* if the length of the key is greater than the length of the value - 1, then the key has counted as far as
                     * it can, so I reset it. */
                    if (pair.Key.length > pair.Value.length - 1)
                    {
                        pair.Key.length = -1;
                        pair.Key.enabled = false;
                    }

                    switch (pair.Key.type.ToLower())
                    {
                        case "day":
                            {
                                if (Main.dayTime)
                                {
                                    /* it's day time and the length of the key is equal to (but not greater than)
                                     * the length of the value - 1. So I start the battle */
                                    if (pair.Key.length == pair.Value.length - 1)
                                    {
                                        if (Boss_Tools.boss_Config.Enable_DayTimer_Text)
                                            TSPlayer.All.SendMessage(Boss_Tools.boss_Config.DayTimer_Text[pair.Key.length],
                                                CustomColours.msgColor);

                                        if (dayBossEnabled)
                                            Boss_Events.start_BossBattle_Day();

                                        if (!MinionTimerRunning)
                                            startMinionTimer();
                                    }
                                    else
                                    {
                                        /* This is what sends the countdown message to players 
                                         * pair.Key.Length is a numerical value, and ...DayTimer_Text is a string[]
                                         * So using ...DayTimer_Text[pair.Key.Value] sends the message in the
                                         * pair.Key.Value position of the string array. Eg, if pair.Key.Value is 4, then the
                                         * 5th message in the array is sent (because of 0 based indexing)
                                         */
                                        if (Boss_Tools.boss_Config.Enable_DayTimer_Text)
                                            TSPlayer.All.SendMessage(Boss_Tools.boss_Config.DayTimer_Text[pair.Key.length],
                                                CustomColours.msgColor);
                                    }
                                }
                                else
                                {
                                    /* The key is enabled, but it's not day time, so I disable the key and reset its count */
                                    pair.Key.enabled = false;
                                    resetTicks(true, false, false);
                                }

                                break;
                            }
                        case "night":
                            {
                                if (!Main.dayTime)
                                {
                                    if (pair.Key.length == pair.Value.length - 1)
                                    {
                                        if (Boss_Tools.boss_Config.Enable_NightTimer_Text)
                                            TSPlayer.All.SendMessage(Boss_Tools.boss_Config.NightTimer_Text[pair.Key.length],
                                             CustomColours.msgColor);

                                        if (nightBossEnabled)
                                        Boss_Events.start_BossBattle_Night();

                                        if (!MinionTimerRunning)
                                            startMinionTimer();
                                    }
                                    else
                                    {
                                        if (Boss_Tools.boss_Config.Enable_NightTimer_Text)
                                            TSPlayer.All.SendMessage(Boss_Tools.boss_Config.NightTimer_Text[pair.Key.length],
                                                CustomColours.msgColor);
                                    }
                                }
                                else
                                {
                                    pair.Key.enabled = false;
                                    resetTicks(false, true, false);
                                }
                                break;
                            }
                        case "special":
                            {
                                if (Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                                    Main.snowMoon || Main.invasionSize > 0)
                                {
                                    if (pair.Key.length == pair.Value.length - 1)
                                    {
                                        if (Boss_Tools.boss_Config.Enable_SpecialTimer_Text)
                                            TSPlayer.All.SendMessage(Boss_Tools.boss_Config.SpecialTimer_Text[pair.Key.length],
                                                   CustomColours.msgColor);

                                        if (specialBossEnabled)
                                        Boss_Events.start_BossBattle_Special();

                                        if (!MinionTimerRunning)
                                            startMinionTimer();
                                    }
                                    else
                                    {
                                        if (Boss_Tools.boss_Config.Enable_SpecialTimer_Text)
                                            TSPlayer.All.SendMessage(Boss_Tools.boss_Config.SpecialTimer_Text[pair.Key.length],
                                                CustomColours.msgColor);
                                    }
                                }
                                else
                                {
                                    pair.Key.enabled = false;
                                    resetTicks(false, false, true);
                                }
                            }
                            break;
                    }
                }
            }
        }

        public static void EndBattle()
        {
            Boss_Tools.Kill_Bosses();
            Boss_Tools.Kill_Minions();
        }

        public static void resetTicks(bool day = true, bool night = true, bool special = true)
        {
            foreach (KeyValuePair<timerObj, timerObj> t in ticker)
            {
                if (day)
                    if (t.Key.type == "day" && t.Key.length > 0)
                        t.Key.length = 0;
                if (night)
                    if (t.Key.type == "night" && t.Key.length > 0)
                        t.Key.length = 0;
                if (special)
                    if (t.Key.type == "special" && t.Key.length > 0)
                        t.Key.length = 0;
            }
        }
        
        public static void startMinionTimer()
        {
            MinionTimerRunning = true;

            Random rndTime = new Random();
            Timer minionTimer = new Timer(rndTime.Next
                (Boss_Tools.boss_Config.Minions_Spawn_Timer[0] / Boss_Tools.boss_Config.Minions_Spawn_Timer[1])
                * 1000);
            minionTimer.Enabled = true;
            minionTimer.Elapsed += Minion_Elapsed_Event;
        }

        public static void Minion_Elapsed_Event(object sender, ElapsedEventArgs args)
        {
            if (Boss_Events.Bosses_Active && Main.dayTime)
                Boss_Events.start_DayMinion_Spawns();

            if (Boss_Events.Bosses_Active && !Main.dayTime)
                Boss_Events.start_NightMinion_Spawns();

            if (Boss_Events.Bosses_Active && Main.raining || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                                    Main.snowMoon || Main.invasionSize > 0)
                Boss_Events.start_SpecialMinion_Spawns();
        }
    }
}
