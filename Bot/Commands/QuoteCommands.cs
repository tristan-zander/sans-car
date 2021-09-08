using System;
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
    // TODO: make sure that admins can execute quote commands regardless of whether it's enabled or not.
    [Group("quote"), Aliases("q", "quotes", "quo")]
    [Description("Manage quotes from your server. Quotes can be listed at any time or posted to specific channels.")]
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
        [Description("Add a quote that can be accessed from this server.")]
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

                var guildAndChannel = (from g in Context.Guilds
                        join c in Context.Channels on g.QuoteChannel.Id equals c.Id
                        select new {g = g, c = c}).First();
                var guild = guildAndChannel.g;
                guild.QuoteChannel = guildAndChannel.c;
                
                /*
                if (guild == null)
                {
                    guild = new Guild
                    {
                        GuildId = ctx.Guild.Id,
                        Quotes = new List<Quote>(),
                    };
                }
                */

                if (guild.AllowQuotes != true)
                {
                    await ctx.RespondAsync("Sorry, quotes aren't enabled in this server.");
                    return;
                }

                var quote = new Quote
                {
                    Guild = guild,
                    Message = quotedText,
                    TimeAdded = DateTimeOffset.UtcNow,
                    BlamedUser = user
                };
                
                var entity = await Context.Quotes.AddAsync(quote);
                await Context.SaveChangesAsync();
                
                if (guild.EnableQuoteChannel && guild.QuoteChannel != null)
                {
                    var blamedUser = await ctx.Client.GetUserAsync(quote.BlamedUser.Id);
                    var embed = new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Blue)
                        .WithUrl("https://sanscar.net")
                        .AddField($"{blamedUser.Username}#{blamedUser.Discriminator}", $"{quote.Message}")
                        .WithTimestamp(quote.TimeAdded)
                        .Build();
                    var channel = await ctx.Client.GetChannelAsync(guild.QuoteChannel.Id);
                    await channel.SendMessageAsync(embed);
                }

                await ctx.RespondAsync($"Successfully added quote: ```{entity.Entity.Message.Replace("`", "")}```");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to add quote to the database");

                await ctx.RespondAsync("There was an error adding your quote.");
            }
        }

        // TODO add functionality for "sans quote list --show ids"
        // Might be useful for reporting quotes or removing them on masse
        [Command("list"), Aliases("l")]
        [Description("List quotes from the server in an interactive fashion.")]
        public async Task ListQuotes(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id);

            if (guild == null)
            {
                await ctx.RespondAsync("There are no quotes in this server! Use \"sans quote add\" to add a quote.");
                return;
            }

            if (guild.AllowQuotes != true)
            {
                await ctx.RespondAsync("Sorry, quotes aren't enabled in this server.");
                return;
            }

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
            var pages = interact.GeneratePagesInEmbed(output.ToString(), SplitType.Line, embedBuilder);

            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
        }

        [Command("remove"), Aliases("delete", "d", "r")]
        [Description("Remove quote(s) on your server. The search can match a portion of the quote so you don't have to " +
                     "type it out verbatim.\n" +
                     "Ex. the quote \"The imposter is sus\" can be found and removed just by typing \"sus\"")]
        public async Task RemoveQuote(CommandContext ctx, [RemainingText] string search)
        {
            await ctx.TriggerTypingAsync();

            if (await CheckQuoteAvailability(ctx) != true)
            {
                return;
            }

            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id);

            if (guild == null)
            {
                await ctx.RespondAsync("There are no quotes in this server! Use \"sans quote add\" to add a quote.");
                return;
            }

            if (guild.AllowQuotes != true)
            {
                await ctx.RespondAsync("Sorry, quotes aren't enabled in this server.");
                return;
            }

            if (search.Length <= 0)
            {
                await ctx.RespondAsync("Usage: \"sans quote remove [quote message]\".");
                return;
            }

            var possibleQuotes = (from quote in Context.Quotes
                    join user in Context.Users on quote.BlamedUser.Id equals user.Id
                    select new Quote
                    {
                        QuoteId = quote.QuoteId, Guild = quote.Guild,
                        Message = quote.Message, BlamedUser = user,
                        TimeAdded = quote.TimeAdded
                    })
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
                    .AddField(blamedUser.Mention ?? "Unknown user", $"```{quote.Message}```")
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

        [Command("disable"), RequireOwner]
        [Description("Disables whether regular users can use the quotes feature.")]
        public async Task DisableQuotes(CommandContext ctx)
        {
            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id) ?? new Guild
            {
                GuildId = ctx.Guild.Id,
                AllowQuotes = false,
            };
            guild.AllowQuotes = false;
            var updated = Context.Guilds.Update(guild);
            await Context.SaveChangesAsync();

            await ctx.RespondAsync("Disabling quotes (existing quotes will not be deleted).");
        }
        
        [Command("enable"), RequireOwner]
        [Description("Enables the quotes feature for regular users.")]
        public async Task EnableQuotes(CommandContext ctx)
        {
            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id) ?? new Guild
            {
                GuildId = ctx.Guild.Id,
                AllowQuotes = true,
            };
            guild.AllowQuotes = true;
            var updated = Context.Guilds.Update(guild);
            await Context.SaveChangesAsync();

            await ctx.RespondAsync("Enabling quotes feature.");
        }

        /// <summary>
        /// Checks whether or not the server/user can execute quote commands. Note: This function also handles replying
        /// to the user (so don't do it twice).
        /// </summary>
        /// <param name="ctx">The parent command's command context</param>
        /// <returns>Whether or not the guild is allowed to use quotes or already has quotes.</returns>
        private async ValueTask<bool> CheckQuoteAvailability(CommandContext ctx)
        {
            var dbGuild = await Context.Guilds.FindAsync(ctx.Guild.Id);

            if (dbGuild == null)
            {
                await ctx.RespondAsync("There are no quotes in this server! Use \"sans quote add\" to add a quote.");
                return false;
            }

            if (dbGuild.AllowQuotes != true)
            {
                await ctx.RespondAsync("Sorry, quotes aren't enabled in this server.");
                return false;
            }

            return true;
        }
    }
}