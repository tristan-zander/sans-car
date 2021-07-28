using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net;

namespace Data
{
    public class User
    {
        /// <summary>
        /// Analogous to the Discord user's ID snowflake.
        /// </summary>
        [Key] public ulong Id { get; init; }

        public User(DiscordUser user)
        {
            Id = user.Id;
        }

        public User(ulong id)
        {
            Id = id;
        }
    }
}