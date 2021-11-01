using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Builders;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    interface ISearchCommand
    {
        public string GetName();

        public string[] GetAliases();

        public void SetLogger(ILogger<BaseDiscordClient> logger);

        public Task Execute(DiscordClient sender, MessageCreateEventArgs args);
    }


    // Remove this thing eventually. It's just a meme thing after all.
    public class SearchCommands : BaseCommandModule
    {
        public ILogger<BaseDiscordClient> Logger { private get; init; }
        public SansDbContext Context { private get; init; }

        private Dictionary<string, ISearchCommand> _commands = new Dictionary<string, ISearchCommand>();

        public SearchCommands(ILogger<BaseDiscordClient> logger, SansDbContext db)
        {
            Logger = logger;
            Context = db;

            // TODO: use reflection or source generation to get all the search commands based on the assembly.
            AddCommand<SansCarSearchCommand>(_commands);
            AddCommand<KyloRenSearchCommand>(_commands);
        }

        private void AddCommand<T>(IDictionary<string, ISearchCommand> array) where T : ISearchCommand, new()
        {
            var command = new T();
            command.SetLogger(Logger);

            foreach (var alias in command.GetAliases())
            {
                array.Add(alias.ToUpper().Trim(), command);
            }
        }

        public async Task SearchCommandsEvent(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (args.Author.IsBot) return;

            var commands = _commands.Keys.Where(commName => args.Message.Content.ToUpper().Contains(commName)).ToArray();

            if (!commands.Any())
            {
                return;
            }

            var guild = (await Context.Guilds.FindAsync(args.Guild.Id))?.AllowSearchCommands ?? true;
            if (!guild) return;

            foreach (var commName in commands)
            {
                try
                {
                    await _commands[commName].Execute(sender, args);
                }
                // TODO: Send this to the database for auditing. Look into Serilog or just put it on the database.
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to execute command {Command}", commName);
                    await args.Message.RespondAsync("I was supposed to send you a message, but I failed somewhere. Please try again or contact the developer.");
                }
            }
        }
    }

    public class SansCarSearchCommand : ISearchCommand
    {
        private ILogger<BaseDiscordClient> _logger;
        public string GetName()
        {
            return "Sans car";
        }

        public string[] GetAliases()
        {
            return new[] { "sans car", "sanscar" };
        }

        public void SetLogger(ILogger<BaseDiscordClient> logger)
        {
            _logger = logger;
        }

        public async Task Execute(DiscordClient sender, MessageCreateEventArgs args)
        {
            await using var imageFile = MediaResources.GetImage("sans-car.jpg");
            var message = new DiscordMessageBuilder()
                .WithFile(imageFile);
            await args.Message.RespondAsync(message);
        }
    }


    public class KyloRenSearchCommand : ISearchCommand
    {
        private ILogger<BaseDiscordClient> _logger;

        public string GetName()
        {
            return "Bring me the girl";
        }

        public string[] GetAliases()
        {
            return new string[]
            {
                "bring me the girl",
                "kylo ren"
            };
        }

        public void SetLogger(ILogger<BaseDiscordClient> logger)
        {
            _logger = logger;
        }

        public async Task Execute(DiscordClient sender, MessageCreateEventArgs args)
        {
            await using var imageFile = MediaResources.GetImage("bring-me-the-girl.png");
            var message = new DiscordMessageBuilder()
                .WithFile(imageFile);
            await args.Message.RespondAsync(message);
        }
    }
}