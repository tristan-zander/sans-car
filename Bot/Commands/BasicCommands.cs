using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    public class BasicCommands : BaseCommandModule
    {
        public ILogger<BaseDiscordClient> Logger { private get; set; }


        [Command("car")]
        public async Task RespondWithCar(CommandContext ctx)
        {
            try
            {
                var imageFile = MediaResources.GetImage("sans-car.jpg");
                var message = new DiscordMessageBuilder()
                    .WithFile(imageFile);
                await ctx.RespondAsync(message);
            } catch (Exception e)
            {
                Logger.LogError(e, "Failed to respond to \'sans car\'");
                await ctx.RespondAsync("Could not send the sans car image for some reason.");
            }
        }
    }
}