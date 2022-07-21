using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Linq;

namespace LivestreamTest
{
    public static class SpotifyHandler
    {
        public const string CLIENT_ID = "e2a70db47e3a4557ad2d1d7bad871a83";
        public const string CLIENT_SECRET = "0e069101abcb474886411aef86ca8e01";

        public static ClientsClass Clients = new ClientsClass();

        #region ResponseClasses

        public class TokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public long expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
        }

        public class RefreshTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public long expires_in { get; set; }
            public string scope { get; set; }
        }

        #endregion

        public class ClientsClass //Remember a client could have multiple tokens
        {
            private List<ClientToken> ClientTokens { get; set; } = new List<ClientToken>();

            public string AddClient(string refreshToken)
            {
                if (ClientTokens.Any(x => x.RefreshToken == refreshToken))
                {
                    return ClientTokens.Where(x => x.RefreshToken == refreshToken).FirstOrDefault().GUID;
                }

                var tokenResponse = RefreshToken(refreshToken);

                if (tokenResponse == null)
                    return "";

                string guid = Guid.NewGuid().ToString();

                var newClientToken = new ClientToken();
                newClientToken.ClientCode = "";
                newClientToken.RefreshToken = refreshToken;
                newClientToken.Token = tokenResponse.access_token;
                newClientToken.ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in);
                newClientToken.GUID = guid;

                ClientTokens.Add(newClientToken);

                return guid;
            }

            //public bool Add(ClientToken item)
            //{
            //    if (ClientTokens.Any(x => x.ClientCode == item.ClientCode))
            //    {
            //        return false;
            //    }

            //    ClientTokens.Add(item);

            //    return true;
            //}

            public ClientToken GetClient(string GUID)
            {
                return ClientTokens.FirstOrDefault(x => x.GUID == GUID);
            }

            public string GetClientToken(string GUID)
            {
                var foundClient = GetClient(GUID);

                if (foundClient == null)
                    return null;

                if (foundClient.ExpiresAt < DateTime.UtcNow) //Expired
                {
                    var refreshToken = RefreshToken(foundClient.RefreshToken);

                    foundClient.Token = refreshToken.access_token;
                    foundClient.ExpiresAt = DateTime.UtcNow.AddSeconds(refreshToken.expires_in);
                }
                
                return foundClient.Token;
            }

            //public TokenResponse GetToken(string clientCode)
            //{
            //    try
            //    {
            //        RESTServiceRequestor.RestClient restClient = new RESTServiceRequestor.RestClient("https://accounts.spotify.com/api/token");
            //        restClient.OverrideContentType = "application/x-www-form-urlencoded";

            //        NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            //        outgoingQueryString.Add("grant_type", "authorization_code");
            //        outgoingQueryString.Add("code", clientCode);
            //        outgoingQueryString.Add("redirect_uri", "https://theserverofservers.ddns.net/Spotify/Callback");
            //        outgoingQueryString.Add("client_id", CLIENT_ID);
            //        outgoingQueryString.Add("client_secret", CLIENT_SECRET);

            //        var response = restClient.PostRequest(outgoingQueryString.ToString());

            //        var deserialized = JsonConvert.DeserializeObject<TokenResponse>(response);

            //        return deserialized;
            //    }
            //    catch (WebException wex)
            //    {
            //        var resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();

            //        dynamic obj = JsonConvert.DeserializeObject(resp);
            //        var messageFromServer = obj.error.message;
            //        var r = 1;
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //    return null;
            //}

            private RefreshTokenResponse RefreshToken(string refreshToken)
            {
                try
                {
                    MDO.RESTServiceRequestor.Standard.RestClient restClient = new MDO.RESTServiceRequestor.Standard.RestClient("https://accounts.spotify.com/api/token");
                    restClient.OverrideContentType = "application/x-www-form-urlencoded";

                    restClient.Headers.Add("Authorization", $"Basic {Base64Encode($"{CLIENT_ID}:{CLIENT_SECRET}")}");

                    NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);

                    outgoingQueryString.Add("grant_type", "refresh_token");
                    outgoingQueryString.Add("refresh_token", refreshToken);
                    //outgoingQueryString.Add("client_id", CLIENT_ID);
                    //outgoingQueryString.Add("client_secret", CLIENT_SECRET);

                    var response = restClient.PostRequest(outgoingQueryString.ToString());

                    var deserialized = JsonConvert.DeserializeObject<RefreshTokenResponse>(response);

                    return deserialized;
                }
                catch (WebException wex)
                {
                    var resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();

                    dynamic obj = JsonConvert.DeserializeObject(resp);
                    var messageFromServer = obj.error.message;
                    var r = 1;
                }
                catch (Exception ex)
                {

                }

                return null;
            }
        }

        public class ClientToken
        {
            public string GUID { get; set; }
            public string ClientCode { get; set; }
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public DateTime ExpiresAt { get; set; }
        }


        public static class APIEndPoint
        {
            public static Models.SpotifyMeModel GetMe(string clientGUID)
            {
                try
                {
                    var clientToken = Clients.GetClientToken(clientGUID);

                    if (clientToken == null)
                        return null;

                    MDO.RESTServiceRequestor.Standard.RestClient restClient = new MDO.RESTServiceRequestor.Standard.RestClient("https://api.spotify.com/v1/me");
                    //restClient.OverrideContentType = "application/x-www-form-urlencoded";

                    restClient.Headers.Add("Authorization", $"Bearer {clientToken}");

                    NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);

                    var response = restClient.GetRequest();

                    var deserialized = JsonConvert.DeserializeObject<Models.SpotifyMeModel>(response);

                    return deserialized;
                }
                catch (WebException wex)
                {
                    var resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();

                    dynamic obj = JsonConvert.DeserializeObject(resp);
                    var messageFromServer = obj.error.message;
                    var r = 1;
                }
                catch (Exception ex)
                {

                }

                return null;
            }            
            
            public static Models.SpotifyRecentlyPlayedModel RecentlyPlayed(string clientGUID)
            {
                try
                {
                    var clientToken = Clients.GetClientToken(clientGUID);

                    if (clientToken == null)
                        return null;

                    MDO.RESTServiceRequestor.Standard.RestClient restClient = new MDO.RESTServiceRequestor.Standard.RestClient("https://api.spotify.com/v1/me/player/recently-played");
                    //restClient.OverrideContentType = "application/x-www-form-urlencoded";

                    restClient.Headers.Add("Authorization", $"Bearer {clientToken}");

                    NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);

                    var response = restClient.GetRequest();

                    var deserialized = JsonConvert.DeserializeObject<Models.SpotifyRecentlyPlayedModel>(response);

                    return deserialized;
                }
                catch (WebException wex)
                {
                    var resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();

                    dynamic obj = JsonConvert.DeserializeObject(resp);
                    var messageFromServer = obj.error.message;
                    var r = 1;
                }
                catch (Exception ex)
                {

                }

                return null;
            }

            public static Models.SpotifyCurrentlyPlayingModel CurrentlyPlayingTrack(string clientGUID)
            {
                try
                {
                    var clientToken = Clients.GetClientToken(clientGUID);

                    if (clientToken == null)
                        return null;

                    MDO.RESTServiceRequestor.Standard.RestClient restClient = new MDO.RESTServiceRequestor.Standard.RestClient("https://api.spotify.com/v1/me/player/currently-playing");
                    //restClient.OverrideContentType = "application/x-www-form-urlencoded";

                    restClient.Headers.Add("Authorization", $"Bearer {clientToken}");

                    NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);

                    var response = restClient.GetRequest();

                    var deserialized = JsonConvert.DeserializeObject<Models.SpotifyCurrentlyPlayingModel>(response);

                    return deserialized;
                }
                catch (WebException wex)
                {
                    var resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();

                    dynamic obj = JsonConvert.DeserializeObject(resp);
                    var messageFromServer = obj.error.message;
                    var r = 1;
                }
                catch (Exception ex)
                {

                }

                return null;
            }
            
            public static bool AddTrackToEndOfQueue(string clientGUID, string songURI)
            {
                try
                {
                    var clientToken = Clients.GetClientToken(clientGUID);

                    if (clientToken == null)
                        return false;

                    MDO.RESTServiceRequestor.Standard.RestClient restClient = new MDO.RESTServiceRequestor.Standard.RestClient("https://api.spotify.com/v1/me/player/queue?uri="+WebUtility.UrlEncode(songURI));
                    restClient.OverrideContentType = "application/json";

                    restClient.Headers.Add("Authorization", $"Bearer {clientToken}");

                    NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                    //outgoingQueryString.Add("uri", songURI);

                    var response = restClient.PostRequest(outgoingQueryString.ToString());

                    return true;
                }
                catch (WebException wex)
                {
                    var resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();

                    dynamic obj = JsonConvert.DeserializeObject(resp);
                    //var messageFromServer = obj.error.message;
                    var r = 1;
                }
                catch (Exception ex)
                {

                }

                return false;
            }

            public enum QueryType
            {
                track,
                artist
            }

            public static Models.SpotifySearchModel Search(string clientGUID, string q, QueryType type)
            {
                try
                {
                    var clientToken = Clients.GetClientToken(clientGUID);

                    if (clientToken == null)
                        return null;

                    MDO.RESTServiceRequestor.Standard.RestClient restClient = new MDO.RESTServiceRequestor.Standard.RestClient("https://api.spotify.com/v1/search");
                    //restClient.OverrideContentType = "application/x-www-form-urlencoded";

                    restClient.Headers.Add("Authorization", $"Bearer {clientToken}");

                    var query = new Dictionary<string, string>();
                    query.Add("q", q);
                    query.Add("type", type.ToString());

                    var queryParms = restClient.GenerateQueryParms(query);

                    restClient.BaseUrl += queryParms;

                    NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);

                    var response = restClient.GetRequest();

                    var deserialized = JsonConvert.DeserializeObject<Models.SpotifySearchModel>(response);

                    return deserialized;
                }
                catch (WebException wex)
                {
                    var resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();

                    dynamic obj = JsonConvert.DeserializeObject(resp);
                    var messageFromServer = obj.error.message;
                    var r = 1;
                }
                catch (Exception ex)
                {

                }

                return null;
            }
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
