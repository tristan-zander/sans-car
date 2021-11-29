// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace Bot.Audio
{
	public class AudioManager
	{
		private readonly Dictionary<ulong, GuildAudioPlayer> _playerDict = new Dictionary<ulong, GuildAudioPlayer>();

		private readonly DiscordClient _parent;

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
