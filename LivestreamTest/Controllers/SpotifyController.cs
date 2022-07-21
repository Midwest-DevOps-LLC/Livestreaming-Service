using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LivestreamTest.Controllers
{
    public class SpotifyController : Controller
    {
        public IActionResult Index()
        {
            var model = new Models.SpotifyIndexModel();

            //var spotifyClientGUID = HttpContext.Session.GetString("SpotifyGUID");

            //model.ClientGUID = spotifyClientGUID;

            MDO.RESTServiceRequestor.Standard.ThirdPartyRequest thirdPartyRequest = new MDO.RESTServiceRequestor.Standard.ThirdPartyRequest("https://api.midwestdevops.com/", "");
            var r = thirdPartyRequest.GetThirdParty(2);

            var clientGUID = SpotifyHandler.Clients.AddClient(r.Data.ThirdPartyUsers.FirstOrDefault().ApiKey);

            HttpContext.Session.SetString("SpotifyGUID", clientGUID);

            model.ClientGUID = clientGUID;

            return View(model);
        }

        public IActionResult Callback()
        {
            var model = new Models.SpotifyIndexModel();

            try
            {
                var query = HttpContext.Request.Query;

                var code = "";
                var error = "";

                if (query.ContainsKey("code"))
                {
                    code = query["code"];
                    var guid = SpotifyHandler.Clients.AddClient(code);

                    HttpContext.Session.SetString("SpotifyGUID", guid);

                    model.ClientGUID = code;
                    model.LoginSuccess = Models.SpotifyIndexModel.LoginSuccessEnum.Success;

                    return RedirectToAction("Index");
                }
                else
                {
                    error = query["error"];

                    model.LoginSuccess = Models.SpotifyIndexModel.LoginSuccessEnum.Declined;

                    return View("Index", model);
                }
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

            model.LoginSuccess = Models.SpotifyIndexModel.LoginSuccessEnum.UnknownError;

            return View("Index", model);
        }

        public IActionResult Login()
        {
            var scopes = "user-read-private user-read-email user-read-recently-played user-read-playback-state user-top-read user-modify-playback-state user-read-currently-playing user-read-playback-position";

            var link = $"https://accounts.spotify.com/authorize?response_type=code&client_id={SpotifyHandler.CLIENT_ID}&scope={WebUtility.UrlEncode(scopes)}&redirect_uri={WebUtility.UrlEncode("https://theserverofservers.ddns.net/Spotify/Callback")}";

            return Redirect(link);
        }

        [HttpGet]
        public IActionResult Me()
        {
            var spotifyClientGUID = HttpContext.Session.GetString("SpotifyGUID");

            if (string.IsNullOrEmpty(spotifyClientGUID))
            {
                return Unauthorized();
            }

            var model = SpotifyHandler.APIEndPoint.GetMe(spotifyClientGUID);

            return PartialView("_Me", model);
        }

        [HttpGet]
        public IActionResult RecentlyPlayed()
        {
            var spotifyClientGUID = HttpContext.Session.GetString("SpotifyGUID");

            if (string.IsNullOrEmpty(spotifyClientGUID))
            {
                return Unauthorized();
            }

            var model = SpotifyHandler.APIEndPoint.RecentlyPlayed(spotifyClientGUID);

            return PartialView("_RecentlyPlayed", model);
        }        
        
        [HttpGet]
        public IActionResult CurrentlyPlaying()
        {
            var spotifyClientGUID = HttpContext.Session.GetString("SpotifyGUID");

            if (string.IsNullOrEmpty(spotifyClientGUID))
            {
                return Unauthorized();
            }

            var model = SpotifyHandler.APIEndPoint.CurrentlyPlayingTrack(spotifyClientGUID);

            return PartialView("_CurrentlyPlaying", model);
        }

        [HttpPost]
        public IActionResult AddItemToQueue(string uri)
        {
            var spotifyClientGUID = HttpContext.Session.GetString("SpotifyGUID");

            if (string.IsNullOrEmpty(spotifyClientGUID))
            {
                return Unauthorized();
            }

            var model = SpotifyHandler.APIEndPoint.AddTrackToEndOfQueue(spotifyClientGUID, uri);

            return NoContent();
        }

        [HttpPost]
        public IActionResult Search(string q, string type)
        {
            var spotifyClientGUID = HttpContext.Session.GetString("SpotifyGUID");

            if (string.IsNullOrEmpty(spotifyClientGUID))
            {
                return Unauthorized();
            }

            SpotifyHandler.APIEndPoint.QueryType queryType = SpotifyHandler.APIEndPoint.QueryType.track;

            if (type.ToLower() == "artist")
            {
                queryType = SpotifyHandler.APIEndPoint.QueryType.artist;
            }

            var model = SpotifyHandler.APIEndPoint.Search(spotifyClientGUID, q, queryType);

            return PartialView("_Search", model);
        }
    }
}
