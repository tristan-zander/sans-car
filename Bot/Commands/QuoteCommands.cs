using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    // TODO: make sure that admins can execute quote commands regardless of whether it's enabled or not.
    [Group("quote"), Aliases("q", "quotes", "quo")]
    [Description("Manage quotes from your server. Quotes can be listed at any time or posted to specific channels.")]
    public class QuoteCommands : BaseCommandModule
    {
        private SansDbContext Context { get; }
        private ILogger<QuoteCommands> Logger { get; }

        public QuoteCommands(SansDbContext context, ILogger<QuoteCommands> logger)
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
            await ctx.TriggerTypingAsync();

            var user = await Context.Users.FindAsync(mention.Id);
            if (user == null)
            {
                user = new User(mention);
                await Context.Users.AddAsync(user);
            }

            var guild = await Context.Guilds
                .Include(g => g.QuoteChannel)
                .SingleOrDefaultAsync(g => g.GuildId == ctx.Guild.Id);

            if (guild == null)
            {
                guild = new Guild
                {
                    GuildId = ctx.Guild.Id
                };
                await Context.AddAsync(guild);
            }

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
                Owner = user,
                DiscordMessage = ctx.Message.Id,
            };

            var entity = await Context.Quotes.AddAsync(quote);
            await Context.SaveChangesAsync();

            var blamedUser = await ctx.Client.GetUserAsync(quote.Owner.Id);
            var blamedMember = await ctx.Guild.GetMemberAsync(quote.Owner.Id);
            var message = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Blue)
                .WithUrl("https://sanscar.net")
                .AddField($"{blamedMember.Nickname}", $"{quote.Message}")
                .WithFooter($"ID: {quote.QuoteId}")
                .WithTimestamp(quote.TimeAdded)
                .WithAuthor($"{blamedUser.Username}#{blamedUser.Discriminator}")
                .Build();
            // TODO: Move logic to something that listens on pg_notify
            if (guild.EnableQuoteChannel && guild.QuoteChannel != null)
            {
                var channel = await ctx.Client.GetChannelAsync(guild.QuoteChannel.Id);
                await channel.SendMessageAsync(message);
            }

            await ctx.RespondAsync(message);
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

            var quotes = Context.Quotes
                .Include(q => q.Owner)
                .Include(q => q.Guild)
                .Where(q => q.Guild.GuildId == ctx.Guild.Id)
                .OrderByDescending(q => q.TimeAdded)
                .Take(50).ToList();

            if (quotes.Count <= 0)
            {
                await ctx.RespondAsync("Looks like you don't have any quotes on this server yet :(");
                return;
            }

            var output = new StringBuilder();

            foreach (var quote in quotes)
            {
                var user = await ctx.Client.GetUserAsync(quote.Owner.Id);
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
        [Description(
            "Remove quote(s) on your server. The search can match a portion of the quote so you don't have to " +
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

            // Has to pull in every quote from the database into memory iirc
            // Probably has some poor performance consequences.
            // TODO: See if Postgres has a way to fuzz text
            var possibleQuotes = Context.Quotes
                .Include(q => q.Owner)
                .Where(q => q.Guild.GuildId == ctx.Guild.Id)
                .AsEnumerable()
                .OrderByDescending(q => Fuzz.WeightedRatio(search, q.Message))
                .Take(3)
                .ToList();

            var firstQuote = possibleQuotes.First();
            var bestFuzz = Fuzz.WeightedRatio(search, firstQuote.Message);
            if (bestFuzz > 80)
            {
                var blamedUser = await ctx.Client.GetUserAsync(firstQuote.Owner.Id);

                // TODO ask to see if the user has permissions to call this command.
                if (ctx.Message.Author.IsBot || blamedUser != ctx.Message.Author)
                {
                    await ctx.RespondAsync("You do not have permission to remove quotes that are not yours.");
                }

                var embed = new DiscordEmbedBuilder().WithDescription(
                        "Are you sure this is the message that you want to delete?")
                    .AddField(blamedUser.Mention ?? "Unknown user", $"```{firstQuote.Message}```")
                    .Build();

                var msg = await ctx.RespondAsync(embed);

                var emoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                await msg.CreateReactionAsync(emoji);

                // TODO: put an explicit time limit on this.
                var reactionRes = await msg.WaitForReactionAsync(ctx.Message.Author, emoji);

                if (!reactionRes.TimedOut)
                {
                    Context.Quotes.Remove(firstQuote);
                    await Context.SaveChangesAsync();

                    var modifiedEmbed = new DiscordEmbedBuilder(embed)
                        .WithDescription("Okay! The quote has been deleted.").ClearFields().Build();
                    await msg.ModifyAsync(modifiedEmbed);
                }

                // ERROR: Throws 403
                await msg.DeleteAllReactionsAsync("User has reached a decision on quote removal.");
            }
            else
            {
                // TODO: give this option a place to actually remove options.
                var builder = new StringBuilder();

                foreach (var quote in possibleQuotes)
                {
                    var ratio = Fuzz.WeightedRatio(search, quote.Message);
                    builder.AppendLine($"Fuzz level: {ratio}, Message: ```{quote.Message}```");
                }

                await ctx.RespondAsync(builder.ToString());
            }
        }

        [Command, Description("Edit a previously added quote.")]
        public async Task Edit(CommandContext ctx,
            [Description("The ID or a text copy of the quote that you want to change.")]
            Quote quote,
            [RemainingText, Description("The new quote message.")]
            string newQuoteMessage)
        {
            await ctx.RespondAsync("Hey, I found your quote!");
        }


        [Command("disable"), RequireOwner]
        [Description("Disables whether regular users can use the quotes feature.")]
        public async Task DisableQuotes(CommandContext ctx)
        {
            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id);
            if (guild == null)
            {
                guild = new Guild
                {
                    GuildId = ctx.Guild.Id,
                    AllowQuotes = true,
                };
                await Context.Guilds.AddAsync(guild);
            }

            guild.AllowQuotes = false;

            await Context.SaveChangesAsync();

            await ctx.RespondAsync("Disabling quotes (existing quotes will not be deleted).");
        }

        [Command("enable"), RequireOwner]
        [Description("Enables the quotes feature for regular users.")]
        public async Task EnableQuotes(CommandContext ctx)
        {
            var guild = await Context.Guilds.FindAsync(ctx.Guild.Id);
            if (guild == null)
            {
                guild = new Guild
                {
                    GuildId = ctx.Guild.Id,
                    AllowQuotes = true,
                };
                await Context.Guilds.AddAsync(guild);
            }

            guild.AllowQuotes = true;

            await Context.SaveChangesAsync();

            await ctx.RespondAsync("Enabling quotes feature.");
        }

        // TODO: Phase this out. Enums would be a better way of dealing with this.
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

    public class QuoteConvertor : IArgumentConverter<Quote>
    {
        public SansDbContext Database { get; init; }

        public Task<Optional<Quote>> ConvertAsync(string value, CommandContext ctx)
        {
            if (Guid.TryParse(value, out var quoteId))
            {
                // Include all objects from other tables like this?
                var quote = Database.Quotes
                    .Include(i => i.Guild)
                    .Include(i => i.Owner)
                    .First(q => q.QuoteId == quoteId);
                return Task.FromResult(Optional.FromValue(quote));
            }

            return Task.FromResult(Optional.FromNoValue<Quote>());
        }
    }
}