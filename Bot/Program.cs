using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bot.Commands;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using LavalinkConfiguration = DSharpPlus.Lavalink.LavalinkConfiguration;

namespace Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = RunBot(args).GetAwaiter().GetResult();

            Task.Delay(-1).Wait();

            // TODO: be able to restart the bot on a whim if you're an admin.
            // Use a Monitor and lock to do this.
            client.StopAsync().Wait();
        }

        static async Task<DiscordShardedClient> RunBot(string[] args)
        {
            #region InitialBotConfig

            var dotnetEnvironment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var isDevelopment = dotnetEnvironment is "Development";
            var isStaging = dotnetEnvironment is "Staging";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
            if (isDevelopment)
            {
                builder.AddUserSecrets<Program>();
            }

            var config = builder.Build();

            var discordConfiguration = new DiscordConfiguration
            {
                Token = config["Bot:Token"],
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            };

            var bot = new DiscordShardedClient(discordConfiguration);

            await bot.UseInteractivityAsync(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.DeleteEmojis,
                Timeout = TimeSpan.FromSeconds(30),
                PaginationBehaviour = PaginationBehaviour.WrapAround,
            });

            var dbContext = new SansDbContext(config["Database:ConnectionString"]);

            var services = new ServiceCollection()
                .AddLogging(loggingOptions =>
                {
                    // yyyy-MM-dd HH:mm:ss zzz
                    loggingOptions.SetMinimumLevel(LogLevel.Debug);
                    loggingOptions.AddConsole();
                    loggingOptions.AddConfiguration(config);
                })
                .AddDbContext<SansDbContext>(options =>
                {
                    options.EnableDetailedErrors();
                    if (isDevelopment)
                    {
                        options.EnableSensitiveDataLogging();
                    }
                    options.UseNpgsql(config["Database:ConnectionString"], optionsBuilder =>
                    {
                        optionsBuilder.UseFuzzyStringMatch();
                        optionsBuilder.UseTrigrams();
                    });
                })
                .BuildServiceProvider();

            var botConfig = config.GetSection("Bot").Get<BotConfiguration>();
            var commands = await bot.UseCommandsNextAsync(
                new CommandsNextConfiguration
                {
                    StringPrefixes = botConfig.Prefixes ?? new[] { "sans ", "s?" },
                    Services = services
                });

            var slashConf = new SlashCommandsConfiguration
            {
                Services = services
            };
            var slashExt = await bot.UseSlashCommandsAsync(slashConf);

            if (isDevelopment && ulong.TryParse(config["DevelopmentGuild"], out var guildId))
            {
                slashExt.RegisterCommands<SlashCommands>(guildId);
                slashExt.RegisterCommands<QuoteCommands>(guildId);
            }
            else
            {
                slashExt.RegisterCommands<SlashCommands>();
                slashExt.RegisterCommands<QuoteCommands>();
            }

            foreach (var shard in bot.ShardClients.Keys)
            {
                commands[shard]?.RegisterCommands(Assembly.GetExecutingAssembly());

                slashExt[shard].SlashCommandErrored += (sender, eventArgs) =>
                {
                    bot.Logger.LogError(eventArgs.Exception, "Slash command failed");
                    return Task.CompletedTask;
                };

                #region OnCommandError

                commands[shard].CommandErrored += async (ev, arg) =>
                {
                    switch (arg.Exception)
                    {
                        case CommandNotFoundException or ArgumentException:
                            return;
                        case DbUpdateConcurrencyException e:
                            {
                                foreach (var entry in e.Entries)
                                {
                                    var proposedValues = entry.CurrentValues;
                                    var databaseValues = await entry.GetDatabaseValuesAsync();

                                    if (databaseValues == null)
                                    {
                                        // We probably did an update with a Database entry that doesn't exist here.
                                        entry.OriginalValues.SetValues(proposedValues);

                                        continue;
                                    }

                                    // Otherwise prioritize the Database values.
                                    entry.OriginalValues.SetValues(databaseValues);
                                }

                                await dbContext.SaveChangesAsync();
                                break;
                            }
                    }

                    bot.Logger.LogError(arg.Exception, "CommandsNext command failed");

                    // TODO: Send the exception to the Database.

                    await arg.Context.RespondAsync(
                        $"The bot ran into an error while trying to execute your command.\n```{arg.Exception.Message}```");
                };

                #endregion
            }

            var searchCommands = new SearchCommands(bot.Logger, dbContext);
            bot.MessageCreated += searchCommands.SearchCommandsEvent;

            #endregion InitialBotConfig

            await bot.StartAsync();

            #region LavaLinkConfig

            var lavalinkSettings = config.GetSection("Lavalink").Get<Data.LavalinkConfiguration>();
            var endpoint = new ConnectionEndpoint
            {
                Hostname = lavalinkSettings.Address, // From your server configuration.
                Port = lavalinkSettings.Port // From your server configuration
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                // TODO: Get password from config
                Password = lavalinkSettings.Password, // From your server configuration.
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = await bot.UseLavalinkAsync();
            var linkResults = lavalink.Values.Select(link => link.ConnectAsync(lavalinkConfig));
            foreach (var linkResult in linkResults)
            {
                await linkResult;
            }

            #endregion

            /* TODO: Add OAuth to the website and implement this.
            await bot.UpdateStatusAsync(
                new DiscordActivity("Type \"sans help\" for help.", ActivityType.Playing));
            */

            return bot;
        }
    }
}