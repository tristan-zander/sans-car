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