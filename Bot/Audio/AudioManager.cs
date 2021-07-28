using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace Bot.Audio
{
    public class AudioManager
    {
        private Dictionary<ulong, GuildAudioPlayer> _playerDict = new Dictionary<ulong, GuildAudioPlayer>();

        private DiscordClient _parent;

        public AudioManager(DiscordClient client)
        {
            _parent = client;
        }

        public GuildAudioPlayer GetPlayer(ulong guildId)
        {
            return _playerDict[guildId];
        }
    }
}