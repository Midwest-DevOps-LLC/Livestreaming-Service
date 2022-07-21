using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivestreamTest.Models
{
    public class SpotifyRecentlyPlayedModel
    {
        public List<ItemsClass> items { get; set; } = new List<ItemsClass>();
        public string next { get; set; }
        public CursorsClass cursors { get; set; } = new CursorsClass();
        public int limit { get; set; }
        public string href { get; set; }

        public class ItemsClass
        {
            public TrackClass track { get; set; } = new TrackClass();
            public string played_at { get; set; }
            public ContextClass context { get; set; }

            public class TrackClass
            {
                public AlbumClass album { get; set; } = new AlbumClass();
                public List<ArtistClass> artists { get; set; } = new List<ArtistClass>();
                public List<string> available_markets { get; set; } = new List<string>();
                public int disc_number { get; set; }
                public long duration_ms { get; set; }
                [Newtonsoft.Json.JsonProperty("explicit")]
                public bool Explicit { get; set; }
                public Models.SpotifyMeModel.ExternalUrls external_urls { get; set; } = new SpotifyMeModel.ExternalUrls();
                public string href { get; set; }
                public string id { get; set; }
                public bool is_local { get; set; }
                public string name { get; set; }
                public int popularity { get; set; }
                public string preview_url { get; set; }
                public int track_number { get; set; }
                public string type { get; set; }
                public string uri { get; set; }

                public class AlbumClass
                {
                    public string album_type { get; set; }
                    public List<ArtistClass> artists { get; set; } = new List<ArtistClass>();
                    public List<string> available_markets { get; set; } = new List<string>();
                    public Models.SpotifyMeModel.ExternalUrls external_urls { get; set; } = new SpotifyMeModel.ExternalUrls();
                    public string href { get; set; }
                    public string id { get; set; }
                    public List<Models.SpotifyMeModel.ImagesClass> images { get; set; } = new List<SpotifyMeModel.ImagesClass>();
                    public string name { get; set; }
                    public string release_date { get; set; }
                    public string release_date_precision { get; set; }
                    public int total_tracks { get; set; }
                    public string type { get; set; }
                    public string uri { get; set; }
                }

                public class ArtistClass
                {
                    public Models.SpotifyMeModel.ExternalUrls external_urls { get; set; } = new SpotifyMeModel.ExternalUrls();
                    public string href { get; set; }
                    public string id { get; set; }
                    public string name { get; set; }
                    public string type { get; set; }
                    public string uri { get; set; }
                }
            }

            public class ContextClass
            {
                public string uri { get; set; }
                public Models.SpotifyMeModel.ExternalUrls external_urls { get; set; } = new SpotifyMeModel.ExternalUrls();
                public string href { get; set; }
                public string type { get; set; }
            }

            public class CursorsClass
            {
                public string after { get; set; }
                public string before { get; set; }
            }
        }

        public class CursorsClass
        {
            public string after { get; set; }
            public string before { get; set; }
        }
    }
}
