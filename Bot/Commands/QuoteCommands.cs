using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DiscordUser = Data.DiscordUser;

namespace Bot.Commands
{
    // TODO: make sure that admins can execute quote commands regardless of whether it's enabled or not.
    [Group("quote"), Aliases("q", "quotes", "quo")]
    [Description("Manage quotes from your server. Quotes can be listed at any time or posted to specific channels.")]
    public class QuoteCommands : ApplicationCommandModule
    {
        private SansDbContext Database { get; set; }
        private ILogger<QuoteCommands> Logger { get; set; }

        public QuoteCommands(SansDbContext database, ILogger<QuoteCommands> logger)
        {
            Database = database;
            Logger = logger;
        }

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Add Quote")]
        // ReSharper disable once UnusedMember.Global
        public async Task AddQuoteFromDiscordMessage(ContextMenuContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate,
                new DiscordInteractionResponseBuilder());

            var message = await InsertQuote(Database, ctx, ctx.TargetMessage.Author, ctx.TargetMessage.Content);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(message));
        }

        [SlashCommandGroup("quote", "Add and manage quotes for your server.")]
        // ReSharper disable once UnusedType.Global
        public class QuoteSlashCommands : ApplicationCommandModule
        {
            public SansDbContext Database { private get; set; }
            public ILogger<QuoteSlashCommands> Logger { private get; set; }

            public QuoteSlashCommands(SansDbContext database, ILogger<QuoteSlashCommands> logger)
            {
                Logger = logger;
                Database = database;
            }

            [SlashCommand("add", "Add a quote to your guild.")]
            private async Task AddQuote(InteractionContext ctx,
                [Option("author", "The user that authored this quote,")]
                DSharpPlus.Entities.DiscordUser mention,
                [Option("quote", "The quote that you would like to add.")]
                string quotedText)
            {
                Logger.LogInformation("Running slash command");

                var message = await InsertQuote(Database, ctx, mention, quotedText);

                if (message == null)
                {
                    return;
                }

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(message));
            }


            [SlashCommand("search", "Search for quotes based on certain criteria.")]
            [Description("List quotes from the server in an interactive fashion.")]
            public async Task ListQuotes(InteractionContext ctx,
                [Option("search", "Searches for quotes with similar text.")] string search = "",
                [Option("starting_from", "Only show quotes made before this date.")] string from = null,
                [Option("stopping_at", "Only show quotes made after this date.")] string to = null,
                [Option("mentions", "Show only quotes from this user.")] SnowflakeObject user = null)
            {
                var guild = await Database.Guilds.FindAsync(ctx.Guild.Id);

                if (guild == null)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            "There are no quotes in this server! Use \"sans quote add\" to add a quote."));
                    return;
                }

                if (guild.AllowQuotes != true)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            "Sorry, quotes aren't enabled in this server."));
                    return;
                }

                var useFromDate = DateTimeOffset.TryParse(from, out var dateFrom);
                var useToDate = DateTimeOffset.TryParse(to, out var dateTo);
                var useMentions = user != null;

                var query = Database.Quotes
                    .Include(q => q.Guild)
                    .Where(q => q.Guild.GuildId == ctx.Guild.Id
                                && (!useFromDate || q.TimeAdded <= dateFrom)
                                && (!useToDate || q.TimeAdded >= dateTo)
                                && (!useMentions || q.Mentions.Contains(user.Id)));

                Logger.LogDebug("Query for `quote list`: {}", query.ToQueryString());

                List<Quote> quotes;

                if (search != null)
                {
                    // Levenstein Distance can only be up to 255 bytes.
                    var tempQuery =
                        query.OrderBy(q => EF.Functions.TrigramsSimilarityDistance(q.Message, search));

                    Logger.LogDebug("Fuzzy Search Query {}", tempQuery.ToQueryString());

                    quotes = await tempQuery.Take(50).ToListAsync();
                }
                else
                {
                    quotes = await query
                        .OrderByDescending(q => q.TimeAdded)
                        .Take(50).ToListAsync();
                }

                if (quotes.Count <= 0)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            "I couldn't find any quotes that matched. Are you being too specific?")
                            .AsEphemeral(true));
                    return;
                }

                var output = new StringBuilder();

                foreach (var quote in quotes)
                {
                    var quoteUser = await ctx.Client.GetUserAsync(quote.Owner);
                    output.Append($"```{quote.Message.Replace("`", "")}```{quoteUser.Mention} on {quote.TimeAdded:d}\n\n");
                }

                // remove the final 2 end-lines.
                output.Remove(output.Length - 2, 2);

                var embedBuilder = new DiscordEmbedBuilder().WithColor(DiscordColor.Blue);
                var interact = ctx.Client.GetInteractivity();
                var pages = interact.GeneratePagesInEmbed(output.ToString(), SplitType.Line, embedBuilder);

                await interact.SendPaginatedResponseAsync(ctx.Interaction,
                    true, ctx.User, pages, behaviour: PaginationBehaviour.WrapAround,
                    deletion: ButtonPaginationBehavior.DeleteMessage);
            }

            /// <summary>
            /// Delete a quote from the database.
            /// </summary>
            /// <param name="ctx">The slash command context.</param>
            /// <param name="quote">An ID to a pre-existing quote.</param>
            [SlashCommand("delete", "Delete a quote.")]
            public async Task RemoveQuote(InteractionContext ctx,
                [Autocomplete(typeof(QuoteAutocomplete)), Option("quote", "The Id of a quote that you wish to remove.")] string quoteId)
            {
                Guid id;
                if (string.IsNullOrEmpty(quoteId) || !Guid.TryParse(quoteId, out id))
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("Invalid Quote given to /quote delete.\n" +
                                         "Please only pick a quote from the autocomplete or type in a Quote ID.")
                            .AsEphemeral(true));
                    return;
                }

                var quoteToRemove = await Database.Quotes.SingleAsync(q => q.Id == id);
                Database.Quotes.Remove(quoteToRemove);
                await Database.SaveChangesAsync();

                var member = await ctx.Guild.GetMemberAsync(quoteToRemove.Owner);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"{ctx.Member.Mention} removed quote {quoteToRemove.Id:N}\n" +
                                                                        $"Which was originally owned by {member.Mention ?? "unknown"}."));
            }
        }

        /// <summary>
        /// Inserts a quote into the database and does input validation.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="ctx"></param>
        /// <param name="mention">The user that you would like to add as a mention.</param>
        /// <param name="quotedText">The text that you would like to add as the quote's message.</param>
        /// <returns>An embed that you can show to the user.</returns>
        private static async ValueTask<DiscordEmbed> InsertQuote(SansDbContext database, BaseContext ctx,
            SnowflakeObject mention, string quotedText)
        {
            var guild = await database.Guilds
                .Include(g => g.QuoteChannel)
                .SingleOrDefaultAsync(g => g.GuildId == ctx.Guild.Id);

            if (guild == null)
            {
                guild = new Guild
                {
                    GuildId = ctx.Guild.Id
                };
                await database.AddAsync(guild);
            }

            if (guild.AllowQuotes != true)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Quotes are not enabled on this server."));
                return null;
            }

            var mentionList = mention?.Id != null ? new HashSet<ulong> { mention.Id } : new HashSet<ulong>();

            var quote = new Quote
            {
                GuildId = guild.GuildId,
                Guild = guild,
                Message = quotedText,
                TimeAdded = DateTimeOffset.UtcNow,
                Mentions = mentionList,
                Owner = ctx.Member.Id
            };

            await database.Quotes.AddAsync(quote);
            await database.SaveChangesAsync();

            var blamedMember = await ctx.Guild.GetMemberAsync(quote.Owner);
            var message = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Blue)
                .WithUrl("https://sanscar.net")
                .AddField($"{blamedMember.Nickname ?? blamedMember.Username}", $"{quote.Message}")
                .WithFooter($"ID: {quote.Id:N}")
                .WithTimestamp(quote.TimeAdded)
                .WithAuthor($"{blamedMember.Username}#{blamedMember.Discriminator}")
                .Build();

            // TODO: Create an event on Kafka so that other services are aware that the quote has been created.
            if (guild.EnableQuoteChannel && guild.QuoteChannel != null)
            {
                var channel = await ctx.Client.GetChannelAsync(guild.QuoteChannel.Id);
                await channel.SendMessageAsync(message);
            }

            return message;
        }
    }

    public class QuoteAutocomplete : IAutocompleteProvider
    {
        private ILogger<QuoteAutocomplete> Logger { get; set; }

        private SansDbContext Database { get; set; }

        public QuoteAutocomplete()
        { }

        // TODO: Don't get any quotes that are already present on the autocomplete context.
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            Database ??= ctx.Services.GetService(typeof(SansDbContext)) as SansDbContext;
            Logger ??= ctx.Services.GetService(typeof(ILogger<QuoteAutocomplete>)) as ILogger<QuoteAutocomplete>;

            if (Database == null)
            {
                return null;
            }

            var query = Database.Quotes
                .OrderBy(q => q.TimeAdded).Where(q => q.Guild.GuildId == ctx.Guild.Id);

            if (!String.IsNullOrWhiteSpace(ctx.OptionValue.ToString()))
            {
                query = query.OrderByDescending(q => EF.Functions.TrigramsSimilarity(q.Message, ctx.OptionValue.ToString()));
            }

            var quotes = await query
                .Take(15).ToListAsync();

            var choices = quotes.Select(q =>
            {
                string message;
                var member = ctx.Guild.GetMemberAsync(q.Owner).Result;
                string nickname;
                if (member != null)
                {
                    nickname = member.Nickname;
                }
                else
                {
                    var user = ctx.Client.GetUserAsync(q.Owner).Result;
                    nickname = $"{user.Username}#{user.Discriminator}";
                }

                var time = q.LastUpdated?.ToString() ?? q.TimeAdded.ToString();
                if (q.Message.Length < 25)
                {
                    message = $"\"{q.Message[..q.Message.Length]}\" - {nickname} on {time}";
                }
                else
                {
                    message = $"\"{q.Message[..22]}...\" - {nickname} on {time}";
                }
                return new DiscordAutoCompleteChoice(message, q.Id.ToString());
            });

            return choices;
        }
    }
}