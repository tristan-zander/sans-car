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


    }
}