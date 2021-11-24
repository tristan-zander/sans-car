using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    [Keyless]
    public class DiscordUser
    {
        /// <summary>
        /// Analogous to the Discord user's ID snowflake.
        /// </summary>
        public ulong Id { get; set; }

        public DiscordUser(DSharpPlus.Entities.DiscordUser user)
        {
            Id = user.Id;
        }

        public DiscordUser(ulong id)
        {
            Id = id;
        }
    }
}