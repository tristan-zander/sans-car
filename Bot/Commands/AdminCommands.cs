using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    [Group("admin"), RequireOwner]
    [Description("Commands that can only be executed by a server's admin staff.")]
    public class AdminCommands : BaseCommandModule
    {
        private SansDbContext Context { get; }
        private ILogger<BaseDiscordClient> Logger { get; }
        
        [Command("toggle-search-commands"), Aliases("toggle s", "toggle-search")]
        [Description("Toggles whether search commands can be used on your server. Search commands are any special" +
                     " commands that sans car responds to even when there's no bot prefix.")]
        public async Task DisableSearchCommands(CommandContext ctx)
        {
            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id) ?? new Guild
            {
                GuildId = ctx.Guild.Id,
            };
            guild.AllowSearchCommands = !guild.AllowSearchCommands;
            Context.Guilds.Update(guild);
            await Context.SaveChangesAsync();

            await ctx.RespondAsync($"Toggling search commands. New status {guild.AllowSearchCommands}");
        }

        [Command("toggle-quotes"), Aliases("toggle q")]
        [Description("Toggles whether quotes can be used by normal users in your server. Remaining quotes will exist" +
                     "in the database regardless.")]
        public async Task ToggleQuotes(CommandContext ctx)
        {
            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id) ?? new Guild
            {
                GuildId = ctx.Guild.Id,
            };
            guild.AllowQuotes = !guild.AllowQuotes;
            Context.Guilds.Update(guild);
            await Context.SaveChangesAsync();

            await ctx.RespondAsync($"Toggling quotes. New status {guild.AllowQuotes}");
        }

        // This command explicitly must be executed by the owner so bad admins can't wipe server data.
        [RequireOwner]
        public async Task DeleteQuotes(CommandContext ctx)
        {
            
        }
    }
}