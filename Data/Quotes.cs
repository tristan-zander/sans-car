using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace Data
{
    public class Quote
    {
        [Key] public Guid QuoteId { get; } = Guid.NewGuid();

        /// <summary>
        /// The guild that the quote belongs to.
        /// </summary>
        [Required]
        public Guild Guild { get; set; }

        [Required, MaxLength(1024)] public string Message { get; set; }

        /// <summary>
        /// A list of people that are attributed to the quote.
        /// </summary>
        public List<User> Mentions { get; set; }

        [Required] public DateTimeOffset TimeAdded { get; set; }

        /// <summary>
        /// If the user updated the quote, list the last time it was updated.
        /// </summary>
        public DateTimeOffset? LastUpdated { get; set; }

        /// <summary>
        /// Store old revisions of quotes that were modified.
        /// </summary>
        public List<string> PreviouslyModifiedQuotes { get; set; }

        /// <summary>
        /// The user that submitted (owns) the quote.
        /// </summary>
        [Required]
        public User Owner { get; set; }

        /// <summary>
        /// The ID of the Discord Message being referenced
        /// </summary>
        public ulong? DiscordMessage { get; set; }
    }
}