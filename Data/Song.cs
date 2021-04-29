using System;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Song
    {
        [Key] public string SongId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public TimeSpan Length { get; set; }
        [Required]
        public Uri Uri { get; set; }
    }
}