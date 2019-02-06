using Hiwits.Client.Authorization;
using System.Web;
using System.Web.Mvc;

namespace Hiwits.SSOServer.WebApp.Controllers
{

    public class AccountController : Controller
    {
        // GET: Account
        [HttpGet]
        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(string returnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
                return Redirect(returnUrl);

            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string returnUrl = "")
        {
            var tokenRelf = ClientConfiguration.Instance.ExchangeUserCredentialForToken(username, password);

            Response.SetCookie(new HttpCookie("access_token", tokenRelf.AccessToken));

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return Redirect("~/Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LoginOut()
        {
            Response.Cookies.Remove("access_token");
            return View();
        }

        [AllowAnonymous]
        public ActionResult NoPermission()
        {
            return View();
        }
    }
}