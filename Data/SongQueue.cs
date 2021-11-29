// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

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
