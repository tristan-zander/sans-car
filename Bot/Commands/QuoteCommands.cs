using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using FuzzySharp;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    [Group("quote"), Aliases("q", "quotes", "quo")]
    [Description("Manage quotes from your server.")]
    public class QuoteCommands : BaseCommandModule
    {
        private SansDbContext Context { get; }
        private ILogger<BaseDiscordClient> Logger { get; }

        public QuoteCommands(SansDbContext context, ILogger<BaseDiscordClient> logger)
        {
            Context = context;
            Logger = logger;
        }
        
        [Command("add"), Aliases("a")]
        public async Task AddQuote(CommandContext ctx, string quotedText)
        {
            await AddQuoteFull(ctx, ctx.Message.Author, quotedText);
        }

        [Command("add")]
        public async Task AddQuote(CommandContext ctx, string quotedText, DiscordMember mention)
        {
            await AddQuoteFull(ctx, mention, quotedText);
        }
        
        private async Task AddQuoteFull(CommandContext ctx, DiscordUser mention, string quotedText)
        {
            try
            {
                await ctx.TriggerTypingAsync();
                var user = await Context.Users.FindAsync(mention.Id) ?? new User(mention);
                
                var guild = await Context.Guilds.FindAsync(ctx.Guild.Id) ?? new Guild
                {
                    GuildId = ctx.Guild.Id,
                    Quotes = new List<Quote>()
                };

                var quote = new Quote
                {
                    Guild = guild,
                    Message = quotedText,
                    TimeAdded = DateTimeOffset.UtcNow,
                    BlamedUser = user
                };

                var entity = await Context.Quotes.AddAsync(quote);
                await Context.SaveChangesAsync();
                
                // TODO assert that the quote was successfully created.

                await ctx.RespondAsync($"Successfully added quote: ```{entity.Entity.Message.Replace("`", "")}```");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to add quote to the database.");

                await ctx.RespondAsync("There was an error adding your quote.");
            }
        }

        // TODO add functionality for "sans quote list --show ids"
        [Command("list"), Aliases("l")]
        public async Task ListQuotes(CommandContext ctx)
        {
            try
            {
                await ctx.TriggerTypingAsync();
                
                // var quotes = Context.Quotes.Where(q => q.Guild.GuildId == ctx.Guild.Id).Take(10).ToList();
                var quotesQuery = (from quote in Context.Quotes
                    where quote.Guild.GuildId == ctx.Guild.Id
                    join user in Context.Users on quote.BlamedUser.Id equals user.Id
                    orderby quote.TimeAdded descending 
                    select new Quote
                    {
                        Message = quote.Message, BlamedUser = user, TimeAdded = quote.TimeAdded,
                        QuoteId = quote.QuoteId, Guild = quote.Guild
                    }).Take(50).ToList();

                if (quotesQuery.Count <= 0)
                {
                    await ctx.RespondAsync("Looks like you don't have any quotes on this server yet :(");
                    return;
                }

                var output = new StringBuilder();

                foreach (var quote in quotesQuery)
                {
                    var user = await ctx.Client.GetUserAsync(quote.BlamedUser.Id);
                    output.Append($"```{quote.Message.Replace("`", "")}```{user.Mention} on {quote.TimeAdded:d}\n\n");
                }
                // remove the final 2 endlines.
                output.Remove(output.Length - 2, 2);

                var embedBuilder = new DiscordEmbedBuilder().WithColor(DiscordColor.Blue);
                var interact = ctx.Client.GetInteractivity();
                var pages= interact.GeneratePagesInEmbed(output.ToString(), SplitType.Line, embedBuilder);

                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Could not ");
                await ctx.RespondAsync("An error has occurred.");
            }
        }

        [Command("remove"), Aliases("delete", "d", "r")]
        public async Task RemoveQuote(CommandContext ctx, [RemainingText] string search)
        {
            try
            {
                await ctx.TriggerTypingAsync();

                var possibleQuotes = (from quote in Context.Quotes
                    join user in Context.Users on quote.BlamedUser.Id equals user.Id
                    select new Quote{ QuoteId = quote.QuoteId, Guild = quote.Guild,
                        Message = quote.Message, BlamedUser = user,
                        TimeAdded = quote.TimeAdded})
                    .AsEnumerable()
                    .OrderByDescending(q => Fuzz.WeightedRatio(search, q.Message)).Take(3).ToList();
                
                var bestFuzz = Fuzz.WeightedRatio(search, possibleQuotes[0].Message);
                if (bestFuzz > 80)
                {
                    var quote = possibleQuotes[0];
                    var blamedUser = await ctx.Client.GetUserAsync(quote.BlamedUser.Id);
                
                    // TODO ask to see if the user has permissions to call this command.
                    if (ctx.Message.Author.IsBot || blamedUser != ctx.Message.Author)
                    {
                        await ctx.RespondAsync("You do not have permission to remove quotes that are not yours.");
                    }

                    var embed = new DiscordEmbedBuilder().WithDescription(
                        "Are you sure this is the message that you want to delete?")
                        .AddField(blamedUser.Mention ?? "Unknown user", $"```{quote.Message}```" )
                        .Build();

                    var msg = await ctx.RespondAsync(embed);

                    var emoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                    await msg.CreateReactionAsync(emoji);

                    var reactionRes = await msg.WaitForReactionAsync(ctx.Message.Author, emoji);

                    if (!reactionRes.TimedOut)
                    {
                        var removed = Context.Quotes.Remove(quote);
                        await Context.SaveChangesAsync();
                        
                        var modifiedEmbed = new DiscordEmbedBuilder(embed)
                            .WithDescription("Okay! The quote has been deleted.").ClearFields().Build();
                        await msg.ModifyAsync(modifiedEmbed);
                    }
                    
                    await msg.DeleteAllReactionsAsync();
                }
                else
                {
                    var builder = new StringBuilder();

                    foreach (var quote in possibleQuotes)
                    {
                        var ratio = Fuzz.WeightedRatio(search, quote.Message);
                        builder.AppendLine($"Fuzz level: {ratio}, Message: ```{quote.Message}```");
                    }

                    await ctx.RespondAsync($"{builder.ToString()}");
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "Could not remove quote.");
                await ctx.RespondAsync("Could not find a quote that matched.");
            }
        }
    }
}