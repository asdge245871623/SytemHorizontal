using Hiwits.Authorization.Framework;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Hiwits.AuthorizationServer.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;

            if (Request.HttpMethod == "POST")
            {
                var isPersistent = !string.IsNullOrEmpty(Request.Form.Get("isPersistent"));

                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Signin")))
                {
                    authentication.SignIn(
                        new AuthenticationProperties { IsPersistent = isPersistent },
                        new ClaimsIdentity(new[] {
                            new Claim(ClaimsIdentity.DefaultNameClaimType, Request.Form["username"])
                        }, AuthenticationType.Application));
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return View();
        }

    }
}