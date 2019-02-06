using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Plsw.WxAdmin.Client.filters
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