using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LivestreamTest.Controllers
{
    public class LoginController : BaseController
    {
        public LoginController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(MDO.RESTDataEntities.Standard.EmailRegistration.RegistrationStatus? status)
        {
            var statusMessage = "";

            switch (status)
            {
                case MDO.RESTDataEntities.Standard.EmailRegistration.RegistrationStatus.Success:
                    statusMessage = "You were successfully registered";
                    break;
                case MDO.RESTDataEntities.Standard.EmailRegistration.RegistrationStatus.Error:
                    statusMessage = "Something with the registration process went wrong. Try again later";
                    break;
                case MDO.RESTDataEntities.Standard.EmailRegistration.RegistrationStatus.ReSentEmail:
                    statusMessage = "Your registration email was resent. Please check in spam";
                    break;
            }

            ViewBag.Registration = statusMessage;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            if (this.UserSession != null)
            {
                //using (BASIC.TAP.RestApiClient.EmployeeAuthenticationService authService = new BASIC.TAP.RestApiClient.EmployeeAuthenticationService(_config.GetConnectionString("AuthenticationService"), AuthKey, null))
                //{
                //    authService.Logout(UserSession.AuthToken);
                //}
            }
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(Models.LoginModel model)
        {
            var loginRequestor = new MDO.RESTServiceRequestor.Standard.LoginRequestor(this._configuration.GetValue<string>("RESTService"), GetAuth, this._configuration.GetValue<string>("ApplicationGUID"));

            var request = new MDO.RESTDataEntities.Standard.LoginRequest();
            request.Username = model.Username;
            request.Password = model.Password;

            var response = loginRequestor.Login(request);

            if (response.Status == MDO.RESTDataEntities.Standard.APIResponse<MDO.RESTDataEntities.Standard.LoginResponse>.StatusEnum.Complete)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, response.Data.User.Username));
                identity.AddClaim(new Claim(ClaimTypes.Sid, response.Data.Auth));
                identity.AddClaim(new Claim(ClaimTypes.Name, response.Data.User.Username));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = false });
            }

            return Json(response.ValidationModel);
        }

        [HttpPost]
        public IActionResult Register(Models.RegisterModel model)
        {
            var loginRequestor = new MDO.RESTServiceRequestor.Standard.RegistrationRequestor(this._configuration.GetValue<string>("RESTService"), GetAuth, this._configuration.GetValue<string>("ApplicationGUID"));

            var request = new MDO.RESTDataEntities.Standard.RegisterRequest();
            request.Username = model.Username;
            request.Email = model.Email;
            request.Application = this._configuration.GetValue<string>("ApplicationGUID");
            request.Password = model.Password;
            request.RetypePassword = model.RetypePassword;

            var response = loginRequestor.Login(request);

            if (response.Status == MDO.RESTDataEntities.Standard.APIResponse<MDO.RESTDataEntities.Standard.RegistrationResponse>.StatusEnum.Complete)
            {

            }

            return Json(response.ValidationModel);
        }
    }
}