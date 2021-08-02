using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bot.Commands;
using Bot.Convertors;
using Bot.Intermediary_Message_Types;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LavalinkConfiguration = DSharpPlus.Lavalink.LavalinkConfiguration;

namespace Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = RunBot().GetAwaiter().GetResult();

            Task.Delay(-1).Wait();

            // TODO: be able to restart the bot on a whim if you're an admin.
            client.StopAsync().Wait();
        }

        static async Task<DiscordShardedClient> RunBot()
        {
            #region InitialBotConfig

            FullConfiguration projectConfig = null;
            
            var dockerSecret = Environment.GetEnvironmentVariable("DOCKER_SECRET");
            if (dockerSecret != null)
            {
                // Get the project files from docker.
                throw new NotImplementedException("The bot currently isn't meant to be run under docker yet!");
            }
            else
            {
                projectConfig = ProjectConfiguration.ReadConfigFile();
            }

            if (projectConfig.Bot.Token == null)
            {
                throw new Exception("Did not assign environment tokens to TOKEN or DEV_TOKEN.");
            }

            var config = new DiscordConfiguration
            {
                Token = projectConfig.Bot.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            };

            var bot = new DiscordShardedClient(config);

            await bot.UseInteractivityAsync(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.DeleteEmojis,
                Timeout = TimeSpan.FromSeconds(30),
                PaginationBehaviour = PaginationBehaviour.WrapAround,
            });

            var dbContext = new SansDbContext(projectConfig.Database.ConnectionString);

            var services = new ServiceCollection()
                .AddSingleton(bot.Logger)
                .AddDbContext<SansDbContext>()
                .BuildServiceProvider();

            var commands = await bot.UseCommandsNextAsync(
                new CommandsNextConfiguration
                {
                    StringPrefixes = projectConfig.Bot.Prefixes ?? new [] {"sans ", "s?"},
                    Services = services
                });
            
            foreach (var shard in bot.ShardClients.Keys)
            {
                commands[shard]?.RegisterCommands(Assembly.GetExecutingAssembly());
                commands[shard]?.RegisterConverter(new ParseArgsConvertor<MessageArg>());

                #region OnCommandError

                commands[shard].CommandErrored += async (ev, arg) =>
                {
                    bot.Logger.LogError(arg.Exception, "CommandsNext command failed");

                    if (arg.Exception is CommandNotFoundException or ArgumentException)
                    {
                        return;
                    }
                    
                    // TODO: Send the exception to the database.
                    
                    await arg.Context.RespondAsync("The bot ran into an error while trying to execute your command. " +
                                                   "Please try again or contact the developer.");
                };

                #endregion

            }

            var searchCommands = new SearchCommands(bot.Logger, new SansDbContext(projectConfig.Database.ConnectionString));
            bot.MessageCreated += searchCommands.SearchCommandsEvent;

            #endregion InitialBotConfig

            await bot.StartAsync();

            #region LavaLinkConfig

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1", // From your server configuration.
                Port = 2333 // From your server configuration
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                // TODO: Get password from config
                Password = "youshallnotpass", // From your server configuration.
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