using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace Data
{
    public class Channel
    {
        [Key] public ulong Id { get; set; }

        public DiscordChannel ToDiscordChannel()
        {
            throw new NotImplementedException();
        }
    }
}