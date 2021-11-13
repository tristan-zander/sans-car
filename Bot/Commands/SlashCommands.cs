using Data;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public SansDbContext Database { private get; init; }
        public ILogger<SlashCommands> Logger { private get; init; }

        public SlashCommands(SansDbContext database, ILogger<SlashCommands> logger)
        {
            Database = database;
            Logger = logger;
        }
    }
}