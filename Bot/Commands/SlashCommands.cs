using System.ComponentModel;
using System.Threading.Tasks;
using Data;
using DSharpPlus.Entities;
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

        [SlashCommand("car", "sans car sans car")]
        [Description("Send the original sans car image.")]
        public async Task SendSansCar(InteractionContext ctx)
        {
            await using (var image = MediaResources.GetImage("sans-car.jpg"))
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddFile(image));
            }
        }
    }
}