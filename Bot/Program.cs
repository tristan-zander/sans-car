﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Bot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bot
{
    class Program 
    {
        static void Main(string[] args)
        {
            var res = RunBot().GetAwaiter().GetResult();
            Environment.Exit(res);
        }

        static async Task<int> RunBot()
        {
            #if DEBUG
            var workingDir = Path.GetFullPath("./", Directory.GetCurrentDirectory());
            var envPath = Path.Combine(workingDir, ".env");
            DotEnv.Load(envPath);
            
            Console.WriteLine($"Working Directory: {workingDir}");
            var token = Environment.GetEnvironmentVariable("DEV_TOKEN");
            #else
            // Get it from Docker otherwise.
            var token = Environment.GetEnvironmentVariable("TOKEN");
            #endif

            if (token == null)
            {
                throw new Exception("Did not assign environment tokens to TOKEN or DEV_TOKEN.");
            }
            
            var config = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            };

            var bot = new DiscordShardedClient(config);

            var services = new ServiceCollection()
                .AddSingleton(bot.Logger)
                .BuildServiceProvider();

            var commands = await bot.UseCommandsNextAsync(
                new CommandsNextConfiguration
                {
                    StringPrefixes = new[] {"sans ", "sans"},
                    Services = services
                });
            // Do I need to register commands for every single CommandsNextExtension?
            foreach (var shard in bot.ShardClients.Keys)
                commands[shard]?.RegisterCommands(Assembly.GetExecutingAssembly());

            bot.MessageCreated += async (sender, args) =>
            {
                // TODO: better API for the search commands
                var m = args.Message.Content.ToLower().Trim();
                
                // Check the start of it to avoid colliding with the standard command.
                if (m.Contains("sans car") && !m.StartsWith("sans car"))
                {
                    var img = MediaResources.GetImage("sans-car.jpg");
                    var message = new DiscordMessageBuilder()
                        .WithFile(img);
                    await args.Message.RespondAsync(message);
                } else if (m.Contains("bring me the girl"))
                {
                    var img = MediaResources.GetImage("bring-me-the-girl.png");
                    var message = new DiscordMessageBuilder()
                        .WithFile(img);
                    await args.Message.RespondAsync(message);
                }
            };

            await bot.StartAsync();
            await Task.Delay(-1);
            
            return 0;
        }
    }
}