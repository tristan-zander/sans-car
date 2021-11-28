using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
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

    [Group("audio")]
    public class AudioCommands : ApplicationCommandModule
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public ILogger<BaseDiscordClient> Logger { private get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        // public GuildAudioPlayer Player { private get; set; }

        [SlashCommand("join", "Have the bot join the voice channel that you're in.")]
        [Description("Joins the voice channel that you are currently in.")]
        public async Task Join(InteractionContext ctx, DiscordChannel channel = null)
        {
            channel ??= ctx.Member.VoiceState.Channel;

            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        AudioCommandErrors.LavaLinkNotSetup));
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        "Not a valid voice channel."));
                return;
            }

            await node.ConnectAsync(channel);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(
                    $"Joined {channel.Name}!"));
        }

        [SlashCommand("disconnect", "Leave the current voice channel and stop audio playback.")]
        [Description("Leaves the voice channel and stops the audio playback session.")]
        public async Task Leave(InteractionContext ctx, DiscordChannel channel)
        {
            channel ??= ctx.Guild.CurrentMember.VoiceState?.Channel;
            if (channel == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        "The bot isn't currently in a voice channel."));
                return;
            }

            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        AudioCommandErrors.LavaLinkNotSetup));
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        "Not a valid voice channel."));
                return;
            }

            var conn = node.GetGuildConnection(channel.Guild);
            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        AudioCommandErrors.AudioServerNotConnected));
                return;
            }

            await conn.DisconnectAsync();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(
                    $"Left {channel.Name}!"));
        }

        [SlashCommand("play", "Play a song or video in a voice channel.")]
        [Description("Plays a song from a link. Only YouTube links are currently known to be supported.\n" +
                     "Searching by title will be added in the near future.")]
        public async Task Play(InteractionContext ctx, string search)
        {
            if (ctx.Member.VoiceState?.Channel == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        "You are not in a voice channel."));
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        AudioCommandErrors.AudioServerNotConnected));
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(search);
            {
            }

            if (loadResult.LoadResultType is LavalinkLoadResultType.LoadFailed or LavalinkLoadResultType.NoMatches)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Track search failed for {search}."));
                return;
            }

            var track = loadResult.Tracks.First();

            await conn.PlayAsync(track);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(
                    $"Now playing {track.Title}!"));
        }

        [SlashCommand("pause", "Pause audio playback")]
        [Description("Pauses audio playback.")]
        public async Task Pause(InteractionContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        "You are not in a voice channel."));
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        AudioCommandErrors.AudioServerNotConnected));
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        "There are no tracks loaded."));
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