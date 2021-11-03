using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using NpgsqlTypes;

namespace Data
{
    public class Quote
    {
        [Key] public Guid QuoteId { get; set; } = Guid.NewGuid();

        [Required]
        public Guild Guild { get; set; }
        [Required, MaxLength(1024)]
        public string Message { get; set; }

        [Required]
        public DateTimeOffset TimeAdded { get; set; }
        
        [Required]
        public DateTimeOffset LastUpdated { get; set; }

        /// <summary>
        /// The user that submitted the quote.
        /// </summary>
        [Required]
        public User BlamedUser { get; set; }

        /// <summary>
        /// The ID of the Discord Message being referenced
        /// </summary>
        public ulong? DiscordMessage { get; set; }

        public Quote()
        {
        }

        public Quote(Quote other, User user = null, Guild guild = null)
        {
            QuoteId = other.QuoteId;
            Guild = guild ?? other.Guild;
            Message = other.Message;
            TimeAdded = other.TimeAdded;
            BlamedUser = user ?? other.BlamedUser;
        }

        // TODO: Escape the message contents.
        public static string NormalizeMessage()
        {
            throw new NotImplementedException();
        }
    }
}
