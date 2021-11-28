
using System;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class AudioPlayer
    {
        [Key]
        public Guid Id { get; set; } = new Guid();
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime LastActiveTime { get; set; }
#nullable enable
        public SongQueue? SongQueue { get; set; }
        // The voice channel that the bot sits in.
        public ulong? VoiceChannel { get; set; }
#nullable disable
    }
}