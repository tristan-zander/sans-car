using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace Data
{
    public class User
    {
        [Key] public ulong Id { get; set; }

        [Required] public string Name { get; set; }

        public bool HasAccount { get; set; }

        public User(DiscordUser user)
        {
            Id = user.Id;
            Name = user.Username;
            HasAccount = false;
        }

        public User(ulong id, string name)
        {
            Id = id;
            Name = name;
            HasAccount = false;
        }
    }
}