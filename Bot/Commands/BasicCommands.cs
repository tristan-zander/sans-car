using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Bot.Commands
{
    public class BasicCommands : BaseCommandModule
    {
        [Command("car")]
        public async Task RespondWithCar(CommandContext ctx)
        {
            try
            {
                var imageFile = File.Open(Path.Combine("/home/gale/Repos/csharp/SansCar/Bot", "Resources/sans-car.jpg"),
                    FileMode.Open);
                var message = new DiscordMessageBuilder()
                    .WithFile(imageFile);
                await ctx.RespondAsync(message);
            } catch (Exception e)
            {
                Console.WriteLine(e);
                await ctx.RespondAsync("Could not send the sans car image for some reason.");
            }
        }
    }
}