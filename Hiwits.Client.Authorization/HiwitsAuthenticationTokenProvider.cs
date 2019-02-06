using DotNetOpenAuth.OAuth2;
using Microsoft.Owin.Security.Infrastructure;
using Hiwits.Authorization.Cache.SqlServer;
using System;

namespace Hiwits.Client.Authorization
{

    public class HiwitsAuthenticationAccessTokenProvider : AuthenticationTokenProvider
    {
        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
            //判断 Token是否通过授权


            if (context.Ticket.Identity.IsAuthenticated)
            {
                //如果未通过则调用refreshToken进行刷新AccressToken Ps. 这一步需要一个公共缓存库例如redis
                if (context.Ticket.Properties.ExpiresUtc < DateTime.UtcNow)
                {
                    var tokenRelf = UserTokenRelationshipSvc.Instance.Get(context.Ticket.Identity.Name);

                    if (tokenRelf != null)
                    {
                        var state = new AuthorizationState
                        {
                            AccessToken = tokenRelf.AccessToken,
                            RefreshToken = tokenRelf.RefreshToken
                        };

                        try
                        {
                            if (ClientConfiguration.Instance.webServerClient.RefreshAuthorization(state))
                            {
                                tokenRelf.AccessToken = state.AccessToken;
                                tokenRelf.RefreshToken = state.RefreshToken;

                                //重新设置cookie
                                context.Response.Cookies.Append("access_token", state.AccessToken);

                                //更新ticket过期时间
                                context.Ticket.Properties.ExpiresUtc = state.AccessTokenExpirationUtc;
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }

                }

            }

            context.OwinContext.Environment["Properties"] = context.Ticket.Properties;
        }
    }

}