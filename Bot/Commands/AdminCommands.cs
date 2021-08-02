using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    // TODO: Require owner
    [Group("admin"), RequireOwner]
    [Description("Commands that can only be executed by a server's admin staff or owner.")]
    public class AdminCommands : BaseCommandModule
    {
        private SansDbContext Context { get; }
        private ILogger<BaseDiscordClient> Logger { get; }

        public AdminCommands(SansDbContext db, ILogger<BaseDiscordClient> log)
        {
            Context = db;
            Logger = log;
        }

        [Command]
        [Description("Shortcut to saying \"sans help admin\"")]
        public async Task Help(CommandContext ctx)
        {
            var _rawArg = "admin";
            var context = ctx.CommandsNext.CreateContext(ctx.Message, ctx.Prefix, ctx.CommandsNext.FindCommand("help", out _rawArg), "admin " + ctx.RawArgumentString);
            await ctx.CommandsNext.ExecuteCommandAsync(context);
        }
        
        [Command("toggle-search-commands"), Aliases("stoggle", "toggle-search")]
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

            await ctx.RespondAsync($"Toggling search commands. New status: {guild.AllowSearchCommands}.");
        }

        [Command("toggle-quotes"), Aliases("qtoggle")]
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

            await ctx.RespondAsync($"Toggling quotes. New status: {guild.AllowQuotes}.");
        }

        // This command explicitly must be executed by the owner so bad admins can't wipe server data.
        [Command("delete-quotes"),Aliases("qdelete"), RequireOwner]
        [Description("(Unimplemented) Delete all quotes for your server. Quotes will remain in the database for a " +
                     "certain period of time before being deleted but will be unable to be accessed.")]
        public async Task DeleteQuotes(CommandContext ctx)
        {
            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id);
            if (guild == null || guild.Quotes.Count <= 0)
            {
                await ctx.RespondAsync("There are no quotes on your server.");
                return;
            }

            await ctx.RespondAsync("This feature is currently unimplemented. Please contact the developer for more details.");
            // Send an event to the server to delete your quotes after a few days.
        }

        [Command("set-quote-channel"), Aliases("qcset")]
        [Description("Set the channel to which quotes will be posted whenever someone adds one.")]
        public async Task SetQuoteChannel(CommandContext ctx, DiscordChannel chan)
        {
            var channel = await Context.Channels.FindAsync(chan.Id);

            if (channel == null)
            {
                channel = new Channel {Id = chan.Id};
                await Context.Channels.AddAsync(channel);
            }

            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id) ?? new Guild
            {
                GuildId = ctx.Guild.Id
            };

            guild.QuoteChannel = channel;

            Context.Guilds.Update(guild);
            await Context.SaveChangesAsync();

            var enabled = guild.EnableQuoteChannel ? "Enabled" : "Disabled";
            await ctx.RespondAsync($"Setting the quote channel to {chan}. Quote channel status is: {enabled}.");
        }
        
        [Command("toggle-quote-channel"), Aliases("qctoggle")]
        [Description("Toggle posting quotes to a certain channel when added.")]
        public async Task ToggleQuoteChannel(CommandContext ctx)
        {
            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id) ?? new Guild
            {
                GuildId = ctx.Guild.Id
            };
            guild.EnableQuoteChannel = !guild.EnableQuoteChannel;

            if (guild.EnableQuoteChannel && guild.QuoteChannel == null)
            {
                await ctx.RespondAsync("Please set a quote channel using \"sans admin set-quote-channel <channel>\".");
            }
            
            Context.Guilds.Update(guild);
            await Context.SaveChangesAsync();

            var enabled = guild.EnableQuoteChannel ? "Enabled" : "Disabled";
            await ctx.RespondAsync($"Setting the quote channel status to {enabled}.");
        }
    }
}