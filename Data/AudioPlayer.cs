using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class AudioPlayer
    {
        [Key] public Guild Guild { get; set; }
        public bool IsActive { get; set; }
        public SongQueue SongQueue { get; set; }
        // The voice channel that the bot sits in.
        public Channel? VoiceChannel { get; set; }
    }
}