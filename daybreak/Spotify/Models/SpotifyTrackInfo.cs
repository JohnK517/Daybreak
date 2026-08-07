using System;
using System.Collections.Generic;
using System.Text;

namespace daybreak.Spotify.Models
{
    public class SpotifyTrackInfo
    {
        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public string AlbumArtUrl { get; set; } = "";

        public bool IsPlaying { get; set; }

        public int DurationMs { get; set; }

        public int ProgressMs { get; set; }
    }
}
