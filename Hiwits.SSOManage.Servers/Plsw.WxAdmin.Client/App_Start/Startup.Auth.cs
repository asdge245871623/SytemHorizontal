using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Plsw.WxAdmin.Client
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());

            var OAuthBearerOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = new CookieOAuthBearerProvider(),
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
            c.DeserializeTicket(c.Token);
            c.OwinContext.Environment["Properties"] = c.Ticket.Properties;
        });
    }

    public class CookieOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            //var value = context.Request.Query.Get("access_token");

            var value = context.Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }

            return Task.FromResult<object>(null);
        }

    }
}