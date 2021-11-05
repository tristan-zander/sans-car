using System;
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

namespace Bot.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public SansDbContext Database { private get; set; }
        public ILogger<SlashCommands> Logger { private get; set; }

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Add Quote")]
        public async Task AddQuoteFromDiscordMessage(ContextMenuContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate,
                new DiscordInteractionResponseBuilder());

            var message = await GetQuote(Database, ctx, ctx.TargetMessage.Author, ctx.TargetMessage.Content);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(message));
        }

        [SlashCommandGroup("quote", "Add and manage quotes for your server.")]
        public class QuoteSlashCommands : ApplicationCommandModule
        {
            public SansDbContext Database { private get; set; }
            public ILogger<QuoteSlashCommands> Logger { private get; set; }

            [SlashCommand("add", "Add a quote to your guild.")]
            private async Task AddQuoteFull(InteractionContext ctx,
                [Option("author", "The user that authored this quote,")]
                DiscordUser mention,
                [Option("quote", "The quote that you would like to add.")]
                string quotedText)
            {
                Logger.LogInformation("Running slash command");

                var message = await GetQuote(Database, ctx, mention, quotedText);

                if (message == null)
                {
                    return;
                }

                try
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(message));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(e.Message));
                }
            }


            // TODO add functionality for "sans quote list --show ids"
            // Might be useful for reporting quotes or removing them on masse
            [SlashCommand("list", "Get a paginated list of quotes.")]
            [Description("List quotes from the server in an interactive fashion.")]
            public async Task ListQuotes(InteractionContext ctx)
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

                var quotes = Database.Quotes
                    .Include(q => q.Owner)
                    .Include(q => q.Guild)
                    .Where(q => q.Guild.GuildId == ctx.Guild.Id)
                    .OrderByDescending(q => q.TimeAdded)
                    .Take(50).ToList();

                if (quotes.Count <= 0)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            "Looks like you don't have any quotes on this server yet :("));
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
               
                // TODO: Get the interactivity extension and do all of this manually.
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AsEphemeral(true).WithContent("Sending it now."));
                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, PaginationBehaviour.WrapAround,
                    ButtonPaginationBehavior.DeleteMessage);
            }
        }


        private static async ValueTask<DiscordEmbed> GetQuote(SansDbContext database, BaseContext ctx,
            DiscordUser mention, string quotedText)
        {
            var user = await database.Users.FindAsync(mention.Id);
            if (user == null)
            {
                user = new User(mention);
                await database.Users.AddAsync(user);
            }

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

            var quote = new Quote
            {
                Guild = guild,
                Message = quotedText,
                TimeAdded = DateTimeOffset.UtcNow,
                Owner = user,
            };

            var entity = await database.Quotes.AddAsync(quote);
            await database.SaveChangesAsync();

            var blamedMember = await ctx.Guild.GetMemberAsync(quote.Owner.Id);
            var message = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Blue)
                .WithUrl("https://sanscar.net")
                .AddField($"{blamedMember.Nickname ?? blamedMember.Username}", $"{quote.Message}")
                .WithFooter($"ID: {quote.QuoteId:N}")
                .WithTimestamp(quote.TimeAdded)
                .WithAuthor($"{blamedMember.Username}#{blamedMember.Discriminator}")
                .Build();

            // TODO: Move logic to something that listens on pg_notify
            if (guild.EnableQuoteChannel && guild.QuoteChannel != null)
            {
                var channel = await ctx.Client.GetChannelAsync(guild.QuoteChannel.Id);
                await channel.SendMessageAsync(message);
            }

            return message;
        }
    }
}