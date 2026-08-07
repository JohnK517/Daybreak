using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SpotifyAPI.Web;
using daybreak.Spotify.Models;

namespace daybreak.Spotify.Services
{
    public class SpotifyService
    {
        private readonly SpotifyClient spotify;

        public SpotifyService(SpotifyClient spotify)
        {
            this.spotify = spotify;
        }
        public async Task<SpotifyTrackInfo?> GetCurrentTrackAsync()
        {
            var playback = await spotify.Player.GetCurrentPlayback();

            if (playback == null)
                return null;

            if (playback.Item is not FullTrack track)
                return null;

            return new SpotifyTrackInfo
            {
                Title = track.Name,

                Artist = string.Join(", ",
                    track.Artists.Select(a => a.Name)),

                Album = track.Album.Name,

                AlbumArtUrl = track.Album.Images.FirstOrDefault()?.Url ?? "",

                IsPlaying = playback.IsPlaying,

                DurationMs = track.DurationMs,

                ProgressMs = playback.ProgressMs
            };
        }
        public async Task PlayPauseAsync(bool play)
        {
            if (play)
            {
                await spotify.Player.ResumePlayback();
            }
            else
            {
                await spotify.Player.PausePlayback();
            }
        }


        public async Task NextAsync()
        {
            await spotify.Player.SkipNext();
        }


        public async Task PreviousAsync()
        {
            var playback = await spotify.Player.GetCurrentPlayback();

            if (playback == null)
                return;

            if (playback.ProgressMs > 3000)
            {
                await spotify.Player.SeekTo(
                    new PlayerSeekToRequest(0));
            }
            else
            {
                await spotify.Player.SkipPrevious();
            }
        }
    }
}
