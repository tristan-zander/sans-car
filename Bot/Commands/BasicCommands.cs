using System.Threading.Tasks;
using Bot.Convertors;
using Bot.Intermediary_Message_Types;
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

        [Command]
        public async Task Test(CommandContext ctx, [RemainingText] MessageArg message)
        {
            await ctx.RespondAsync($"Test succeeded: {message.Content}");
        }
    }
}