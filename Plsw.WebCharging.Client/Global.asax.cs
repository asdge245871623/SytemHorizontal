using Microsoft.Owin;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: OwinStartup(typeof(Hiwits.Client.Authorization.SSOClientStartup))]
namespace Plsw.WebCharging.Client
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
