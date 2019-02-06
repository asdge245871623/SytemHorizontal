using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Threading.Tasks;
using Hiwits.Authorization.Framework;

namespace Hiwits.Client.Authorization
{
    public class SSOClientStartup
    {

        public void Configuration(IAppBuilder app)
        {
            // 有关如何配置应用程序的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=316888
            // app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());

            var OAuthBearerOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = new CookieOAuthBearerProvider(),
                AccessTokenProvider = new HiwitsAuthenticationAccessTokenProvider(),
                AccessTokenFormat = new HiwitsSecureDataFormat(),
            };


            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        }
    }
    public class CookieOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            //从QueryString获取accesstoken
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
