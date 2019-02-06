using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Hiwits.Client.Authorization
{
    public class SSOAuthorizeAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                httpContext.Response.Redirect(FormsAuthentication.LoginUrl + "?ReturnUrl=" +
                    HttpUtility.UrlEncode(httpContext.Request.Url.AbsoluteUri));
            }

            return base.AuthorizeCore(httpContext);
        }
    }
}
