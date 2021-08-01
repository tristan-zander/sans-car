#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Guild
    {
        [Key]
        public ulong GuildId { get; set; }

        /// <summary>
        /// Enables the usage of commands that can appear any time in a search, such as "sans car" or "kylo ren"
        /// </summary>
        public bool AllowSearchCommands { get; set; } = true;
        public bool AllowQuotes { get; set; } = true;
        public List<Quote> Quotes { get; set; } = new List<Quote>();
        public bool AllowAudio { get; set; } = true;
        public AudioPlayer? AudioPlayer { get; set; }
    }
}