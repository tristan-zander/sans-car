#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Guild
    {
        [Key]
        public ulong GuildId { get; set; }
        public List<Quote> Quotes { get; set; } = new List<Quote>();
        
        public AudioPlayer? AudioPlayer { get; set; }
    }
}