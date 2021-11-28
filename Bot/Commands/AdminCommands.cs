using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    // TODO: Require owner
    [SlashCommandGroup("admin", "Admin commands that admins can use to configure the bot."), RequireOwner]
    [Description("Commands that can only be executed by a server's admin staff or owner.")]
    public class AdminCommands : ApplicationCommandModule
    {
        private SansDbContext Context { get; }
        private ILogger<AdminCommands> Logger { get; }

        public AdminCommands(SansDbContext db, ILogger<AdminCommands> log)
        {
            Context = db;
            Logger = log;
        }

        [SlashCommand("use_quotes", "Enable or disable quotes in your server.")]
        public async Task QuoteSetting(InteractionContext ctx, [Option("enable", "Set whether quotes are enabled.")] bool enabled,
            [Option("channel", "Set a channel to post new quotes to.")] DiscordChannel quoteChannel = null)
        {
            if (quoteChannel != null && quoteChannel.Type != ChannelType.Text)
            {
                await ctx.CreateResponseAsync(
                    new DiscordInteractionResponseBuilder().WithContent(
                        "Sorry, the quote channel must be a text channel. Please try again."));
                return;
            }
            
            var guild = await Context.Guilds.SingleAsync(g => g.GuildId == ctx.Guild.Id);

            guild.AllowQuotes = enabled;
            guild.QuoteChannel = quoteChannel?.Id;

            Context.Update(guild);
            await Context.SaveChangesAsync();
            var enabledText = guild.AllowQuotes ? "Enabled" : "Disabled";
            var postQuotesText = guild.QuoteChannel == null ? "The quote channel was not set and is not enabled." : $"Quotes will be posted to {quoteChannel.Mention}";
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"{enabledText} quotes in this server. {postQuotesText}").AsEphemeral(true));
        }
    }
}