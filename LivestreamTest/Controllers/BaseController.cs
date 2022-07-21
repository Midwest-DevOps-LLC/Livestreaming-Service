using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace LivestreamTest.Controllers
{
    public class BaseController : Controller
    {
        internal IConfiguration _configuration;
    
        public BaseController (IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MDO.RESTDataEntities.Standard.UserSession _userSession;

        public MDO.RESTDataEntities.Standard.UserSession UserSession
        {
            get
            {
                if (_userSession == null)
                {
                    var auth = AuthToken;

                    if (string.IsNullOrEmpty(auth) == false)
                    {
                        MDO.RESTServiceRequestor.Standard.SessionRequest sessionRequest = new MDO.RESTServiceRequestor.Standard.SessionRequest(this._configuration.GetValue<string>("RESTService"), auth, this._configuration.GetValue<string>("ApplicationGUID"));
                        var response = sessionRequest.Verify();

                        _userSession = response.Data;
                    }
                }

                return _userSession;
            }
        }

        public string GetAuth
        {
            get
            {
                if (UserSession == null)
                    return "";
                else
                    return UserSession.Token;
            }
        }

        public string AuthToken => this.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).FirstOrDefault();

        public string ControllerName
        {
            get
            {
                return this.ControllerContext.RouteData.Values["controller"].ToString();
            }
        }

        public string ActionName
        {
            get
            {
                return this.ControllerContext.RouteData.Values["action"].ToString();
            }
        }

        private bool IsAnonymousPage
        {
            get
            {
                if (ControllerName == "Home")
                    return true;

                if (ControllerName == "Login")
                    return true;

                if (ControllerName == "Registration")
                    return true;

                if (ControllerName == "Review" && (ActionName == "Index" || ActionName == "View"))
                    return true;

                return false;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (IsAnonymousPage == false)
            {
                if (AuthToken == null || UserSession == null)
                {
                    context.Result = new RedirectToActionResult("Logout", "Login", new { Message = "SessionExpired" });
                    return;
                }
            }

            ViewBag.UserSession = UserSession;
            base.OnActionExecuting(context);
        }

        //public MDO.RESTDataEntities.APIResponse<T> RedirectIfSessionIsntValid<T>(MDO.RESTDataEntities.APIResponse<T> response)
        //{
        //    if (response.IsSessionValid == false)
        //    {
        //        Response.Redirect("/Login/Logout");
        //        return response;
        //    }
        //    else
        //    {
        //        return response;
        //    }
        //}
    }
}