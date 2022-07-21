using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivestreamTest.Models
{
    public class SpotifyMeModel
    {
        public string display_name { get; set; }
        public string email { get; set; }
        public ExternalUrls external_urls { get; set; } = new ExternalUrls();
        public string href { get; set; }
        public string id { get; set; }
        public List<ImagesClass> images { get; set; } = new List<ImagesClass>();
        public string product { get; set; }
        public string type { get; set; }
        public string uri { get; set; }

        public class ExternalUrls
        {
            public string spotify { get; set; }
        }

        public class ImagesClass
        {
            public float height { get; set; }
            public string url { get; set; }
            public float width { get; set; }
        }
    }
}
