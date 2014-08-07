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
        private readonly Timer _bossTimer = new Timer {Interval = 1000*AutoBoss.config.MessageInterval, Enabled = true};

        private readonly Timer _minionTimer = new Timer {Interval = 1000, Enabled = true};

        private int _minionTicks;
        public static int minionTime;
        public static int minionSpawnCount;

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

            if (!_bossTimer.Enabled)
                _bossTimer.Enabled = true;

            if (!initial) return;
            if (_initialized) return;
            _bossTimer.Elapsed += BossTimerElapsed;
            _minionTimer.Elapsed += MinionTimerElapsed;
            _initialized = true;
        }

        private void MinionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!AutoBoss.Tools.bossesToggled)
            {
                _minionTicks = 0;
                return;
            }
            if (!_bossActive)
            {
                _minionTicks = 0;
                return;
            }

            _minionTicks++;
            if (_minionTicks != minionTime) return;

            switch (_ticker.type)
            {
                case "day":
                    if (AutoBoss.config.MinionToggles["day"])
                        BossEvents.StartMinionSpawns(BossEvents.SelectMinions("day"));
                    break;
                case "special":
                    if (AutoBoss.config.MinionToggles["special"])
                        BossEvents.StartMinionSpawns(BossEvents.SelectMinions("special"));
                    break;
                case "night":
                    if (AutoBoss.config.MinionToggles["night"])
                        BossEvents.StartMinionSpawns(BossEvents.SelectMinions("night"));
                    break;
            }
            _minionTicks = 0;
        }

        private void BossTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _lastBossState = _bossActive;
            _bossActive = AutoBoss.bossList.Any(p => Main.npc[p.Key].type == p.Value && Main.npc[p.Key].active);

            if (TShock.Utils.ActivePlayers() == 0)
            {
                lock(AutoBoss.bossList)
                    foreach (var pair in AutoBoss.bossList)
                    {
                        TSPlayer.Server.StrikeNPC(pair.Value, 9999, 1f, 1);
                        AutoBoss.bossList.Remove(pair.Key);
                    }
                AutoBoss.bossCounts.Clear();
                return;
            }

            if (_lastBossState && !_bossActive)
            {
                switch (_ticker.type)
                {
                    case "day":
                        TShock.Utils.Broadcast(AutoBoss.config.DayTimerFinished, Color.LightBlue);
                        break;
                    case "night":
                        TShock.Utils.Broadcast(AutoBoss.config.NightTimerFinished, Color.LightBlue);
                        break;
                    case "special":
                        TShock.Utils.Broadcast(AutoBoss.config.SpecialTimerFinished, Color.LightBlue);
                        break;
                }
            }

            if (!AutoBoss.Tools.bossesToggled)
            {
                Log.ConsoleInfo("[AutoBoss+] Timer Disabled: Boss toggle disabled");
                _bossTimer.Enabled = false;
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
                    if (_ticker.count < _ticker.maxCount["day"])
                        TSPlayer.All.SendMessage(AutoBoss.config.DayTimerText[_ticker.count],
                            Color.GreenYellow);

                    else if (_ticker.count >= _ticker.maxCount["day"])
                    {
                        TSPlayer.All.SendMessage(AutoBoss.config.DayTimerText[_ticker.maxCount["day"]],
                            Color.Crimson);

                        if (AutoBoss.config.ContinuousBoss)
                            _ticker.count = -1;
                        else
                            AutoBoss.Tools.bossesToggled = false;

                        BossEvents.StartBossBattleDay();
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

                        if (AutoBoss.config.ContinuousBoss)
                            _ticker.count = -1;
                        else
                            AutoBoss.Tools.bossesToggled = false;

                        BossEvents.StartBossBattleNight();
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

                    if (AutoBoss.config.ContinuousBoss)
                        _ticker.count = -1;
                    else
                        AutoBoss.Tools.bossesToggled = false;

                    BossEvents.StartBossBattleSpecial();
                }
            }
        }
    }
}