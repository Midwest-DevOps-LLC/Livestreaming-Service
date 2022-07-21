using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LivestreamTest
{
    public static class EmoteHandler
    {
        private static EmotesResponse _Emotes { get; set; }
        public static EmotesResponse Emotes
        {
            get
            {
                if (_Emotes == null)
                {
                    _Emotes = GetTwitchEmotes();
                }

                return _Emotes;
            }
        }

        private static EmotesResponse GetTwitchEmotes()
        {
            var authToken = GetTwitchAuth();

            MDO.RESTServiceRequestor.Standard.RestClient restClient = new MDO.RESTServiceRequestor.Standard.RestClient("https://api.twitch.tv/helix/chat/emotes/global");
            restClient.Headers.Add("Authorization", "Bearer " + authToken);
            restClient.Headers.Add("Client-Id", "tqnbabjjogacqjuwldc0pegc1u8o7g");

            var code = restClient.GetRequest();

            var deserial = JsonConvert.DeserializeObject<EmotesResponse>(code);

            return deserial;
        }

        private static string GetTwitchAuth()
        {
            try
            {
                MDO.RESTServiceRequestor.Standard.RestClient restClient = new MDO.RESTServiceRequestor.Standard.RestClient("https://id.twitch.tv/oauth2/token");

                Dictionary<string, string> query = new Dictionary<string, string>();
                query.Add("client_secret", "k7z4ayj9q195zpfudz1fhewd4sfsvl");
                query.Add("client_id", "tqnbabjjogacqjuwldc0pegc1u8o7g");
                query.Add("grant_type", "client_credentials");

                var queryParms = restClient.GenerateQueryParms(query);

                restClient.SetPath(queryParms);



                var request = restClient.PostRequest("");

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<AppTokenResponse>(request);

                return response.access_token;
            }
            catch (WebException wex)
            {
                var resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();

                dynamic obj = JsonConvert.DeserializeObject(resp);
                var messageFromServer = obj.error.message;
            }
            catch (Exception ex)
            {

            }

            return "";
        }

        public class EmotesResponse
        {
            public List<DataClass> data { get; set; } = new List<DataClass>();

            public class DataClass
            {
                public string id { get; set; }
                public string name { get; set; }
                public ImagesClass images { get; set; } = new ImagesClass();

                public class ImagesClass
                {
                    public string url_1x { get; set; }
                    public string url_2x { get; set; }
                    public string url_4x { get; set; }
                }
            }
        }

        class AppTokenResponse
        {
            public string access_token { get; set; }
            public long expires_in { get; set; }
            public string token_type { get; set; }
        }
    }
}
