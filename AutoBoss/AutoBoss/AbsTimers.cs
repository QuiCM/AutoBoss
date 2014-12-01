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
        public BattleType type;

        public readonly Dictionary<BattleType, int> maxCount = new Dictionary<BattleType, int>
        {
            {BattleType.Day, AutoBoss.config.DayTimerText.Length - 1},
            {BattleType.Night, AutoBoss.config.NightTimerText.Length - 1},
            {BattleType.Special, AutoBoss.config.SpecialTimerText.Length - 1}
        };

        public TimerObj(int c, BattleType t)
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

        private readonly TimerObj _ticker = new TimerObj(-1, BattleType.Day);

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
                case BattleType.Day:
                    if (AutoBoss.config.MinionToggles[BattleType.Day])
                        BossEvents.StartMinionSpawns(BossEvents.SelectMinions(BattleType.Day));
                    break;
                case BattleType.Special:
                    if (AutoBoss.config.MinionToggles[BattleType.Special])
                        BossEvents.StartMinionSpawns(BossEvents.SelectMinions(BattleType.Special));
                    break;
                case BattleType.Night:
                    if (AutoBoss.config.MinionToggles[BattleType.Night])
                        BossEvents.StartMinionSpawns(BossEvents.SelectMinions(BattleType.Night));
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
                    case BattleType.Day:
                        TShock.Utils.Broadcast(AutoBoss.config.DayTimerFinished, Color.LightBlue);
                        break;
                    case BattleType.Night:
                        TShock.Utils.Broadcast(AutoBoss.config.NightTimerFinished, Color.LightBlue);
                        break;
                    case BattleType.Special:
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
                _ticker.type = BattleType.Day;

            if (!Main.dayTime && !Main.bloodMoon && !Main.eclipse && !Main.pumpkinMoon &&
                !Main.snowMoon && Main.invasionType == 0 && _nightBossEnabled)
                _ticker.type = BattleType.Night;

            if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon ||
                Main.snowMoon || Main.invasionType > 0 && _specialBossEnabled)
                _ticker.type = BattleType.Special;

            _ticker.count++;

            if (_ticker.type == BattleType.Day)
            {

                if (AutoBoss.config.EnableDayTimerText)
                {
                    if (_ticker.count < _ticker.maxCount[BattleType.Day])
                        TSPlayer.All.SendMessage(AutoBoss.config.DayTimerText[_ticker.count],
                            Color.GreenYellow);

                    else if (_ticker.count >= _ticker.maxCount[BattleType.Day])
                    {
                        TSPlayer.All.SendMessage(AutoBoss.config.DayTimerText[_ticker.maxCount[BattleType.Day]],
                            Color.Crimson);

                        if (AutoBoss.config.ContinuousBoss)
                            _ticker.count = -1;
                        else
                            AutoBoss.Tools.bossesToggled = false;

                        BossEvents.StartBossBattle(BattleType.Day);
                    }
                }
            }

            if (_ticker.type == BattleType.Night)
            {
                if (AutoBoss.config.EnableNightTimerText)
                {
                    if (_ticker.count != _ticker.maxCount[BattleType.Night])
                        TSPlayer.All.SendMessage(AutoBoss.config.NightTimerText[_ticker.count],
                            Color.DarkMagenta);

                    else if (_ticker.count >= _ticker.maxCount[BattleType.Night])
                    {
                        TSPlayer.All.SendMessage(AutoBoss.config.NightTimerText[_ticker.maxCount[BattleType.Night]],
                            Color.Crimson);

                        if (AutoBoss.config.ContinuousBoss)
                            _ticker.count = -1;
                        else
                            AutoBoss.Tools.bossesToggled = false;

                        BossEvents.StartBossBattle(BattleType.Night);
                    }
                }
            }

            if (_ticker.type != BattleType.Special) return;
            
            if (AutoBoss.config.EnableSpecialTimerText)
            {
                if (_ticker.count != _ticker.maxCount[BattleType.Special])
                    TSPlayer.All.SendMessage(AutoBoss.config.SpecialTimerText[_ticker.count],
                        Color.Orange);

                else if (_ticker.count >= _ticker.maxCount[BattleType.Special])
                {
                    TSPlayer.All.SendMessage(AutoBoss.config.SpecialTimerText[_ticker.maxCount[BattleType.Special]],
                        Color.Crimson);

                    if (AutoBoss.config.ContinuousBoss)
                        _ticker.count = -1;
                    else
                        AutoBoss.Tools.bossesToggled = false;

                    BossEvents.StartBossBattle(BattleType.Special);
                }
            }
        }
    }
}