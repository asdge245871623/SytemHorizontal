using System.Web.Mvc;

namespace Plsw.WxAdmin.Client.Controllers
{
    [Hiwits.Client.Authorization.SSOAuthorize(Roles = "admin,plsw.admin,plsw.wx")]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.User = this.User.Identity.Name;

            return View();
        }
    }
}