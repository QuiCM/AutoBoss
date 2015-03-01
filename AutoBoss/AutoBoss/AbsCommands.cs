using System.Linq;
using TShockAPI;

namespace AutoBoss
{
	public static class BossCommands
	{
		public static void BossCommand(CommandArgs args)
		{
			if (args.Parameters.Count < 1 || args.Parameters.Count > 3)
			{
				args.Player.SendErrorMessage("Invalid syntax; Use /boss <option>");
				args.Player.SendErrorMessage("Options: toggle, reload");
				return;
			}

			switch (args.Parameters[0])
			{
				case "toggle":
				{
					AutoBoss.Tools.bossesToggled = !AutoBoss.Tools.bossesToggled;

					if (AutoBoss.Tools.bossesToggled)
					{
						if (
							!AutoBoss.config.BossArenas.Any(
								p => TShock.Regions.Regions.Contains(TShock.Regions.GetRegionByName(p.Key)) && p.Value))
						{
							args.Player.SendErrorMessage("Error: Invalid regions encountered; Boss battles disabled");
							return;
						}

						var day = AutoBoss.config.BossToggles[BattleType.Day];
						var night = AutoBoss.config.BossToggles[BattleType.Night];
						var special = AutoBoss.config.BossToggles[BattleType.Special];
						AutoBoss.Timers.StartBosses(day, night, special, true);
					}

					args.Player.SendSuccessMessage((AutoBoss.Tools.bossesToggled ? "Enabled" : "Disabled") +
					                               " automatic boss spawnings");

					break;
				}
				case "reload":
				{
					if (args.Player == TSPlayer.Server)
						AutoBoss.Tools.ReloadConfig(true);
					else
						AutoBoss.Tools.ReloadConfig(false, args.Player);
					args.Player.SendSuccessMessage("Reloaded BossConfig.json");
					break;
				}

				default:
				{
					args.Player.SendErrorMessage("Invalid syntax; Use /boss <option>");
					args.Player.SendErrorMessage("Options: toggle, reload");
					break;
				}
			}
		}
	}
}