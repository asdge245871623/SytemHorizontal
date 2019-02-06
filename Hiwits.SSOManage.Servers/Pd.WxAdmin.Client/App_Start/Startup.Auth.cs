using Constants;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pd.WxAdmin.Client
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());

            var OAuthBearerOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = new OAuthCookieAndBearerProvider(),
                AccessTokenProvider = new AuthenticationTokenProvider()
                {
                    OnCreate = create,
                    OnReceive = receive
                }
            };

            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }

        public static Action<AuthenticationTokenCreateContext> create = new Action<AuthenticationTokenCreateContext>(c =>
        {
            c.SetToken(c.SerializeTicket());
        });

        public static Action<AuthenticationTokenReceiveContext> receive = new Action<AuthenticationTokenReceiveContext>(c =>
        {
            //解析token
            c.DeserializeTicket(c.Token);

            if (c.Ticket != null)
            {
                //判断Access_Token 是否过期
                if (c.Ticket.Identity.IsAuthenticated && c.Ticket.Properties.ExpiresUtc < DateTime.UtcNow)
                {
                    c.Response.Cookies.Delete("access_token");
                    string accessToken = OAuth2Service.Instance.RefreshAuthorization(c.Ticket.Identity.Name, c.Token);
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        c.Response.Cookies.Append("access_token", accessToken);

                        //重新解析token数据
                        c.DeserializeTicket(accessToken);
                    }
                }

                c.OwinContext.Environment["Properties"] = c.Ticket.Properties;
            }
        });
    }

    public class OAuthCookieAndBearerProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            //从Querystring获取access_token
            //var value = context.Request.Query.Get("access_token");

            //从cookie获取access_token
            var value = context.Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }
           
            return Task.FromResult<object>(null);
        }

        //public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        //{
        //    return base.ValidateIdentity(context);
        //}
    }
}