using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Terraria;
using TShockAPI;

namespace AutoBoss
{
    public class TimerObj
    {
        public int count;
        public string type;

        public readonly Dictionary<string, int> maxCount = new Dictionary<string, int>
        {
            {"day", AutoBoss.config.DayTimerText.Length - 1},
            {"night", AutoBoss.config.NightTimerText.Length - 1},
            {"special", AutoBoss.config.SpecialTimerText.Length - 1}
        };

        public TimerObj(int c, string t)
        {
            count = c;
            type = t;
        }
    }

    public class BossTimer
    {
        public readonly Timer bossTimer = new Timer {Interval = 1000*AutoBoss.config.MessageInterval, Enabled = true};

        private bool _dayBossEnabled;
        private bool _nightBossEnabled;
        private bool _specialBossEnabled;

        private bool _initialized;

        private readonly TimerObj _ticker = new TimerObj(-1, "day");

        private bool _lastBossState;
        private bool _bossActive;

        public void StartBosses(bool day, bool night, bool special, bool initial = false)
        {
            _dayBossEnabled = day;
            _nightBossEnabled = night;
            _specialBossEnabled = special;

            if (!bossTimer.Enabled)
                bossTimer.Enabled = true;

            if (!initial) return;
            if (_initialized) return;
            bossTimer.Elapsed += BossTimerElapsed;
            _initialized = true;
        }

        private void BossTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _lastBossState = _bossActive;
            _bossActive = AutoBoss.bossList.Any(p => Main.npc[p.Key].type == p.Value && Main.npc[p.Key].active);

            if (_lastBossState && !_bossActive)
                TShock.Utils.Broadcast(AutoBoss.config.DayTimerFinished, Color.LightBlue);

            if (!AutoBoss.Tools.bossesToggled)
            {
                Log.ConsoleInfo("[AutoBoss+] Timer Disabled: Boss toggle disabled");
                bossTimer.Enabled = false;
                AutoBoss.Tools.bossesToggled = false;
                _ticker.count = -1;
                return;
            }

            if (_bossActive)
                return;

            if (Main.dayTime && _dayBossEnabled)
                _ticker.type = "day";

            if (!Main.dayTime && !Main.bloodMoon && !Main.eclipse && !Main.pumpkinMoon &&
                !Main.snowMoon && Main.invasionType == 0 && _nightBossEnabled)
                _ticker.type = "night";

            if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                Main.snowMoon || Main.invasionType > 0 && _specialBossEnabled)
                _ticker.type = "special";

            _ticker.count++;

            if (_ticker.type == "day")
            {
                if (AutoBoss.config.EnableDayTimerText)
                {
                    if (_ticker.count != _ticker.maxCount["day"])
                        TSPlayer.All.SendMessage(AutoBoss.config.DayTimerText[_ticker.count],
                            Color.GreenYellow);

                    else if (_ticker.count >= _ticker.maxCount["day"])
                    {
                        TSPlayer.All.SendMessage(AutoBoss.config.DayTimerText[_ticker.maxCount["day"]],
                            Color.Crimson);

                        BossEvents.StartBossBattleDay();

                        if (AutoBoss.config.ContinuousBoss)
                            _ticker.count = -1;
                        else
                            AutoBoss.Tools.bossesToggled = false;
                    }
                }
            }

            if (_ticker.type == "night")
            {
                if (AutoBoss.config.EnableNightTimerText)
                {
                    if (_ticker.count != _ticker.maxCount["night"])
                        TSPlayer.All.SendMessage(AutoBoss.config.NightTimerText[_ticker.count],
                            Color.DarkMagenta);

                    else if (_ticker.count >= _ticker.maxCount["night"])
                    {
                        TSPlayer.All.SendMessage(AutoBoss.config.NightTimerText[_ticker.maxCount["night"]],
                            Color.Crimson);

                        BossEvents.StartBossBattleNight();
                        if (AutoBoss.config.ContinuousBoss)
                            _ticker.count = -1;
                        else
                            AutoBoss.Tools.bossesToggled = false;
                    }
                }
            }

            if (_ticker.type != "special") return;

            if (AutoBoss.config.EnableSpecialTimerText)
            {
                if (_ticker.count != _ticker.maxCount["special"])
                    TSPlayer.All.SendMessage(AutoBoss.config.SpecialTimerText[_ticker.count],
                        Color.Orange);

                else if (_ticker.count >= _ticker.maxCount["special"])
                {
                    TSPlayer.All.SendMessage(AutoBoss.config.SpecialTimerText[_ticker.maxCount["special"]],
                        Color.Crimson);

                    BossEvents.StartBossBattleSpecial();
                    if (AutoBoss.config.ContinuousBoss)
                        _ticker.count = -1;
                    else
                        AutoBoss.Tools.bossesToggled = false;
                }
            }
        }
    }
}