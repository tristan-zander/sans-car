using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Identity;
using NpgsqlTypes;

namespace Data
{
    public class Quote
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The guild that the quote belongs to.
        /// </summary>
        [Required]
        public ulong GuildId { get; set; }
        public Guild Guild { get; set; }

        [Required, MaxLength(1024)] public string Message { get; set; }

        /// <summary>
        /// A list of people that are attributed to the quote. List is Ids of SnowflakeObjects.
        /// </summary>
        public HashSet<ulong> Mentions { get; set; } = new();

        [Required] public DateTimeOffset TimeAdded { get; set; }

        /// <summary>
        /// If the user updated the quote, list the last time it was updated.
        /// </summary>
        public DateTimeOffset? LastUpdated { get; set; }

        /// <summary>
        /// Store old revisions of quotes that were modified.
        /// </summary>
        public List<string> PreviouslyModifiedQuotes { get; set; } = new List<string>();

        /// <summary>
        /// The user's discord Id that submitted (owns) the quote.
        /// </summary>
        [Required]
        public ulong Owner { get; set; }

#nullable enable
        public IdentityUser? OwnerAccount { get; set; }
#nullable disable

        /// <summary>
        /// The ID of the Discord Message being referenced
        /// </summary>
        public ulong? DiscordMessage { get; set; }
    }
}