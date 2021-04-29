using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    [Group("quote"), Aliases("q")]
    [Description("Manage quotes from your server.")]
    public class QuoteCommands : BaseCommandModule
    {
        public SansDbContext Context { get; private set; }
        public ILogger<BaseDiscordClient> Logger { get; private set; }

        public QuoteCommands(SansDbContext context, ILogger<BaseDiscordClient> logger)
        {
            Context = context;
            Logger = logger;
        }

        [Command("add"), Aliases("a")]
        public async Task AddQuote(CommandContext ctx, [RemainingText] string quotedText)
        {
            try
            {
                var user = await Context.Users.FindAsync(ctx.Message.Author.Id);

                if (user == null)
                    user = new User(ctx.Message.Author);

                var guild = await Context.Guilds.FindAsync(ctx.Guild.Id) ?? new Guild
                {
                    GuildId = ctx.Guild.Id,
                    Quotes = new List<Quote>()
                };

                var quote = new Quote
                {
                    DiscordGuild = guild,
                    Message = quotedText,
                    TimeAdded = DateTimeOffset.UtcNow,
                    BlamedUser = user
                };

                var entity = await Context.Quotes.AddAsync(quote);
                await Context.SaveChangesAsync();
                // TODO: Assert that we saved the message.
                Logger.LogInformation($"Added this to the quotes: {entity.Entity}");

                await ctx.RespondAsync($"Successfully added quote: {entity.Entity.Message}");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to add quote to the database.");

                await ctx.RespondAsync("There was an error adding your quote.");
            }
        }

        [Command("list"), Aliases("l")]
        public async Task ListQuotes(CommandContext ctx)
        {
            try
            {
                var quotes = Context.Quotes.Where(q => q.DiscordGuild.GuildId == ctx.Guild.Id).ToList();
                
                var builder = new DiscordMessageBuilder();
                
                
                foreach (var quote in quotes)
                {
                    var user = await ctx.Client.GetUserAsync(quote.BlamedUser.Id);
                    builder.Content += $"{user.Username} said: \"{quote.Message}\"\n";
                }

                await ctx.RespondAsync(builder);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Could not ");
                await ctx.RespondAsync("An error has occurred.");
            }
        }
    }
}