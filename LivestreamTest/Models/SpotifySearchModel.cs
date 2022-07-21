using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivestreamTest.Models
{
    public class SpotifySearchModel
    {
        public TracksClass tracks { get; set; } = new TracksClass();

        public class TracksClass
        {
            public string href { get; set; }
            public List<Models.SpotifyRecentlyPlayedModel.ItemsClass.TrackClass> items { get; set; } = new List<SpotifyRecentlyPlayedModel.ItemsClass.TrackClass>();
        }
    }
}
