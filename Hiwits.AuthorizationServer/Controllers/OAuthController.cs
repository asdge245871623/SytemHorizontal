using Hiwits.Authorization.Framework;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Hiwits.AuthorizationServer.Controllers
{
    public class OAuthController : Controller
    {
        public ActionResult Authorize()
        {
            if (Response.StatusCode != 200)
            {
                return View("AuthorizeError");
            }

            var authentication = HttpContext.GetOwinContext().Authentication;
            var ticket = authentication.AuthenticateAsync(AuthenticationType.Application).Result;
            var identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                authentication.Challenge(AuthenticationType.Application);
                return new HttpUnauthorizedResult();
            }

            var scopes = (Request.QueryString.Get("scope") ?? "").Split(' ');

            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Grant")))
                {
                    identity = new ClaimsIdentity(identity.Claims, "Bearer", ProtectedData.nameType, ProtectedData.roleType);
                    foreach (var scope in scopes)
                    {
                        identity.AddClaim(new Claim(ProtectedData.roleType, scope));
                    }
                    authentication.SignIn(identity);
                }
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Login")))
                {
                    authentication.SignOut(AuthenticationType.Application);
                    authentication.Challenge(AuthenticationType.Application);
                    return new HttpUnauthorizedResult();
                }
            }


            //TagBuilder li = new TagBuilder(tagName: "li");

            return View();
        }
    }
}