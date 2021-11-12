using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net;

namespace Data
{
    public class DiscordUser
    {
        /// <summary>
        /// Analogous to the Discord user's ID snowflake.
        /// </summary>
        [Key] public ulong Id { get; init; }

        public DiscordUser(DSharpPlus.Entities.DiscordUser user)
        {
            Id = user.Id;
        }

        public DiscordUser(ulong id)
        {
            Id = id;
        }

        public DiscordUser()
        {
        }
    }
}