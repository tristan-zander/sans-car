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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            client.StopAsync().Wait();
        }

        static async Task<DiscordShardedClient> RunBot(string[] args)
        {
            #region InitialBotConfig

            var dotnetEnvironment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var isDevelopment = dotnetEnvironment is "Development";

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
                .AddSingleton(bot.Logger)
                .AddSingleton(dbContext)
                .BuildServiceProvider();

            var botConfig = config.GetSection("Bot").Get<BotConfiguration>();
            var commands = await bot.UseCommandsNextAsync(
                new CommandsNextConfiguration
                {
                    StringPrefixes = botConfig.Prefixes ?? new [] {"sans ", "s?"},
                    Services = services
                });
            
            foreach (var shard in bot.ShardClients.Keys)
            {
                commands[shard]?.RegisterCommands(Assembly.GetExecutingAssembly());

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
                                    // We probably did an update with a database entry that doesn't exist here.
                                    entry.OriginalValues.SetValues(proposedValues);
                                    
                                    continue;
                                }
                                
                                // Otherwise prioritize the database values.
                                entry.OriginalValues.SetValues(databaseValues);
                            }

                            await dbContext.SaveChangesAsync();
                            break;
                        }
                    }
                    
                    bot.Logger.LogError(arg.Exception, "CommandsNext command failed");

                    // TODO: Send the exception to the database.
                    
                    await arg.Context.RespondAsync("The bot ran into an error while trying to execute your command. " +
                                                   "Please try again or contact the developer.");
                };

                #endregion

            }

            var searchCommands = new SearchCommands(bot.Logger, dbContext);
            bot.MessageCreated += searchCommands.SearchCommandsEvent;

            #endregion InitialBotConfig

            await bot.StartAsync();

            #region LavaLinkConfig

            var Lavalink = config.GetSection("Lavalink").Get<Data.LavalinkConfiguration>();
            var endpoint = new ConnectionEndpoint
            {
                Hostname = Lavalink.Address, // From your server configuration.
                Port = Lavalink.Port // From your server configuration
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                // TODO: Get password from config
                Password = Lavalink.Password, // From your server configuration.
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