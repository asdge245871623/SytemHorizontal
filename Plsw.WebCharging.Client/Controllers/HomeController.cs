using System.Web.Mvc;

namespace Plsw.WebCharging.Client.Controllers
{

    [Hiwits.Client.Authorization.SSOAuthorize(Users ="geqingwei",Roles = "admin,plsw.admin,plsw.charging")]
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