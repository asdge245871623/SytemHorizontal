using System.Web.Mvc;

namespace Pd.WxAdmin.Client.Controllers
{
    [Hiwits.Client.Authorization.SSOAuthorize(Roles = "admin,pd.admin,pd.wx")]
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