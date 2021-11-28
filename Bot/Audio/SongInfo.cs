// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System;
using System.Linq;
using DSharpPlus.Lavalink;

namespace Bot.Audio
{
	public class SongInfo
	{
		public string Name { get; private set; }
		public LavalinkLoadResult Info { get; private set; }

		public SongInfo(LavalinkLoadResult res)
		{
			switch (res.LoadResultType)
			{
				case LavalinkLoadResultType.LoadFailed:
					throw new Exception("The specified song failed to load.");
				case LavalinkLoadResultType.NoMatches:
					throw new Exception("The specified song had no matches.");
				default:
					// Do nothing
					break;
			}

			Info = res;

			// TODO: add better ways to filter through tracks.
			Name = res.Tracks.First().Title;
		}
	}
}
