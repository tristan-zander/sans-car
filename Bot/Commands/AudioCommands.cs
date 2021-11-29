// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
	public static class AudioCommandErrors
	{
		public const string CouldNotConnect = "Could not connect to the voice channel.";

		public const string LavaLinkNotSetup =
			"The developer hasn't configured the server for audio playback. Use \"sans contact\" if this is wrong.";

		public const string AudioServerNotConnected = "Could not connect to the audio server.";

	}

	public class AudioCommands : BaseCommandModule
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public ILogger<BaseDiscordClient> Logger { private get; set; }

		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		// public GuildAudioPlayer Player { private get; set; }

		[Command, Aliases("j")]
		[Description("Joins the voice channel that you are currently in.")]
		public async Task Join(CommandContext ctx, DiscordChannel channel)
		{
			var lava = ctx.Client.GetLavalink();
			if (!lava.ConnectedNodes.Any())
			{
				await ctx.RespondAsync(AudioCommandErrors.LavaLinkNotSetup);
				return;
			}

			var node = lava.ConnectedNodes.Values.First();

			if (channel.Type != ChannelType.Voice)
			{
				await ctx.RespondAsync("Not a valid voice channel.");
				return;
			}

			await node.ConnectAsync(channel);
			await ctx.RespondAsync($"Joined {channel.Name}!");
		}

		[Command]
		public async Task Join(CommandContext ctx)
		{
			var channel = ctx.Member.VoiceState?.Channel;
			if (channel == null)
			{
				await ctx.RespondAsync("Please connect to a voice channel before the bot joins.");
				return;
			}

			await Join(ctx, channel);
		}

		[Command("leave"), Aliases("dc")]
		[Description("Leaves the voice channel and stops the audio playback session.")]
		public async Task Leave(CommandContext ctx, DiscordChannel channel)
		{
			var lava = ctx.Client.GetLavalink();
			if (!lava.ConnectedNodes.Any())
			{
				await ctx.RespondAsync(AudioCommandErrors.LavaLinkNotSetup);
				return;
			}

			var node = lava.ConnectedNodes.Values.First();

			if (channel.Type != ChannelType.Voice)
			{
				await ctx.RespondAsync("Not a valid voice channel.");
				return;
			}

			var conn = node.GetGuildConnection(channel.Guild);
			if (conn == null)
			{
				await ctx.RespondAsync(AudioCommandErrors.AudioServerNotConnected);
				return;
			}

			await conn.DisconnectAsync();
			await ctx.RespondAsync($"Left {channel.Name}!");
		}

		[Command("leave")]
		public async Task Leave(CommandContext ctx)
		{
			var channel = ctx.Guild.CurrentMember.VoiceState?.Channel;
			if (channel == null)
			{
				await ctx.RespondAsync("The bot isn't currently in a voice channel.");
			}

			await Leave(ctx, channel);
		}

		[Command("play"), Aliases("p")]
		[Description("Plays a song from a link. Only YouTube links are currently known to be supported.\n" +
					 "Searching by title will be added in the near future.")]
		public async Task Play(CommandContext ctx, [RemainingText] string search)
		{
			if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
			{
				await ctx.RespondAsync("You are not in a voice channel.");
				return;
			}

			var lava = ctx.Client.GetLavalink();
			var node = lava.ConnectedNodes.Values.First();

			var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

			if (conn == null)
			{
				await ctx.RespondAsync(AudioCommandErrors.AudioServerNotConnected);
				return;
			}

			var loadResult = await node.Rest.GetTracksAsync(search);
			{
			}

			if (loadResult.LoadResultType is LavalinkLoadResultType.LoadFailed or LavalinkLoadResultType.NoMatches)
			{
				await ctx.RespondAsync($"Track search failed for {search}.");
				return;
			}

			var track = loadResult.Tracks.First();

			await conn.PlayAsync(track);

			await ctx.RespondAsync($"Now playing {track.Title}!");
		}

		[Command("pause"), Aliases("stop")]
		[Description("Pauses audio playback.")]
		public async Task Pause(CommandContext ctx)
		{
			if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
			{
				await ctx.RespondAsync("You are not in a voice channel.");
				return;
			}

			var lava = ctx.Client.GetLavalink();
			var node = lava.ConnectedNodes.Values.First();
			var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

			if (conn == null)
			{
				await ctx.RespondAsync(AudioCommandErrors.AudioServerNotConnected);
				return;
			}

			if (conn.CurrentState.CurrentTrack == null)
			{
				await ctx.RespondAsync("There are no tracks loaded.");
				return;
			}

			await conn.PauseAsync();
		}
		/* TODO: Implement playing and queuing multiple urls.
        [Command("play")]
        public async Task Play(CommandContext ctx, params Uri[] uris)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Could not connect to audio server.");
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(search);
            {
            }

            if (loadResult.LoadResultType is LavalinkLoadResultType.LoadFailed or LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}.");
                return;
            }

            var track = loadResult.Tracks.First();

            await conn.PlayAsync(track);

            await ctx.RespondAsync($"Now playing {track.Title}!");
        } 
        */
	}
}
