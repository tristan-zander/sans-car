using System;
using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    [Group("role"), Aliases("r")]
    [Description("Lets the bot manage and assign roles for your users and your moderators.")]
    public class RoleCommands : BaseCommandModule
    {
        private SansDbContext Context { get; }
        private ILogger<BaseDiscordClient> Logger { get; }

        public RoleCommands(SansDbContext ctx, ILogger<BaseDiscordClient> log)
        {
            Context = ctx;
            Logger = log;
        }

        [Command("assign")]
        public async Task AssignRole(CommandContext ctx, DiscordRole role, DiscordUser user)
        {
            throw new NotImplementedException();
        }
    }
}