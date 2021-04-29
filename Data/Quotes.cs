using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace Data
{
    public class Quote
    {
        [Key] public Guid QuoteId { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guild DiscordGuild { get; set; }
        [Required, MaxLength(500)]
        public string Message { get; set; }
        
        [Required]
        public DateTimeOffset TimeAdded { get; set; }
        // The user that added the quote to the server.
        
        [Required]
        public User BlamedUser { get; set; }
    }
}