#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Guild
    {
        [Key]
        public ulong GuildId { get; set; }

        public bool AllowQuotes { get; set; } = true;
        public List<Quote> Quotes { get; set; } = new List<Quote>();
        public bool AllowAudio = true;
        public AudioPlayer? AudioPlayer { get; set; }
    }
}