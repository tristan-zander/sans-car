// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace Bot.Audio
{
	public class GuildAudioPlayer
	{
		public LavalinkExtension Lavalink { get; private set; }

		private readonly DiscordMember _currentMember;

		public LinkedList<SongInfo> SongQueue { get; private set; }

		public GuildAudioPlayer(DiscordClient bot, DiscordGuild guild)
		{
			_currentMember = guild.CurrentMember;
			SongQueue = new LinkedList<SongInfo>();
		}

		public void QueueSong(string search)
		{

		}

		public void QueueSong(Uri search)
		{

		}

		public DiscordMessage SongQueueToDiscordMessage()
		{
			throw new NotImplementedException();
		}

		public bool IsInVoiceChannel => _currentMember?.VoiceState != null;
	}
}
