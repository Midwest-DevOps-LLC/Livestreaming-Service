using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivestreamTest.Models
{
    public class SpotifyCurrentlyPlayingModel
    {
        public long timestamp { get; set; }
        public Models.SpotifyRecentlyPlayedModel.ItemsClass.ContextClass context { get; set; } = new SpotifyRecentlyPlayedModel.ItemsClass.ContextClass();
        public long progress_ms { get; set; }
        public SpotifyRecentlyPlayedModel.ItemsClass.TrackClass item { get; set; } = new SpotifyRecentlyPlayedModel.ItemsClass.TrackClass();
        public string currently_playing_type { get; set; }
        public ActionsClass actions { get; set; } = new ActionsClass();
        public bool is_playing { get; set; }

        public class ActionsClass
        {
            public DisallowsClass disallows { get; set; } = new DisallowsClass();

            public class DisallowsClass
            {
                public bool resuming { get; set; }
            }
        }
    }
}
