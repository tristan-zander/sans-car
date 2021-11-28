// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace Data
{
	public class Guild
	{
		[Key]
		public ulong GuildId { get; set; }

		/// <summary>
		/// Enables the usage of commands that can appear any time in a search, such as "sans car" or "kylo ren"
		/// </summary>
		public bool AllowSearchCommands { get; set; } = true;
		public bool AllowQuotes { get; set; } = true;
		public List<Quote> Quotes { get; set; } = new List<Quote>();
		public bool EnableQuoteChannel { get; set; } = false;
		public Channel? QuoteChannel { get; set; }
		public bool AllowAudio { get; set; } = true;
		public AudioPlayer? AudioPlayer { get; set; }
	}
}
