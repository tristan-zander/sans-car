// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
	public class BasicCommands : BaseCommandModule
	{
		public ILogger<BaseDiscordClient> Logger { private get; set; }

		[Command("ping"), Description("See how long it takes to communicate with the bot.")]
		public async Task Ping(CommandContext ctx)
		{
			await ctx.RespondAsync($"Ping time: {ctx.Client.Ping}ms");
		}
	}
}
