using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class SongQueue
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        public Queue<Song> Queue { get; set; } = new Queue<Song>();

        public Song CurrentSong => Queue.Peek();
    }
}