using Hiwits.Authorization.Framework;
using Hiwits.Client.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Hiwits.SSOServer.WebApp.Controllers
{
    [Authorize(Roles ="admin,plsw.admin")]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.User = this.User.Identity.Name;
            ViewBag.Roles = string.Join(",", (this.User as ClaimsPrincipal).Claims.Where(cp => cp.Type.Equals(ProtectedData.roleType)).Select(cp => cp.Value));
            return View();
        }
    }
}