// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

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
		public Channel? VoiceChannel { get; set; }
#nullable disable
	}
}
