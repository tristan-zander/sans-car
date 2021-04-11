using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;

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
            var envPath = Path.Combine("/home/gale/Repos/csharp/SansCar/Bot/", ".env");
            DotEnv.Load(envPath);

            /*
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
                */

            #if DEBUG
            var token = Environment.GetEnvironmentVariable("DEV_TOKEN");
            #else
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

            var commands = await bot.UseCommandsNextAsync(new CommandsNextConfiguration{StringPrefixes = new[] {"sans ", "__"} });
            // Do I need to register commands for every single CommandsNextExtension?
            foreach (var command in commands.Values)
            {
                command.RegisterCommands(Assembly.GetExecutingAssembly());
            }

            await bot.StartAsync();
            await Task.Delay(-1);

            return 0;
        }
    }
}