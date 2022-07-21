using LivestreamTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LivestreamTest.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {

            //var loginRequestor = new MDO.RESTServiceRequestor.Standard.LoginRequestor("https://api.midwestdevops.com/", "", "825d53c9-df35-451f-b668-76b467e5f54b");

            //var response = loginRequestor.Login(request);

            //if (response.Status == MDO.RESTDataEntities.Standard.APIResponse<MDO.RESTDataEntities.Standard.LoginResponse>.StatusEnum.Complete)
            //{
            //    //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            //    //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, response.Data.User.Username));
            //    //identity.AddClaim(new Claim(ClaimTypes.Sid, response.Data.Auth));
            //    //identity.AddClaim(new Claim(ClaimTypes.Name, response.Data.User.Username));

            //    //var principal = new ClaimsPrincipal(identity);
            //    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = false });
            //}


            if (string.IsNullOrEmpty(GetAuth) == false)
            {

                MDO.RESTServiceRequestor.Standard.ThirdPartyRequest thirdPartyRequest = new MDO.RESTServiceRequestor.Standard.ThirdPartyRequest("https://api.midwestdevops.com/", AuthToken);
                var r = thirdPartyRequest.GetThirdParty(2);

                var clientGUID = SpotifyHandler.Clients.AddClient(r.Data.ThirdPartyUsers.FirstOrDefault().ApiKey);

                HttpContext.Session.SetString("SpotifyGUID", clientGUID);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
