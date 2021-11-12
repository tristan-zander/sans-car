using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Data
{
    public class DiscordChannel
    {
        [Key] public ulong Id { get; set; }
    }
}