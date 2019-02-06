using Microsoft.Owin.Security.Infrastructure;
using System;

namespace Hiwits.AuthorizationServer
{

    public class HiwitsAuthenticationRefreshTokenProvider : AuthenticationTokenProvider
    {
        public override void Create(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMonths(1);
            context.SetToken(context.SerializeTicket());
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }

    }

    public class HiwitsAuthenticationAccessTokenProvider : AuthenticationTokenProvider
    {
        public override void Create(AuthenticationTokenCreateContext context)
        {
            //context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddSeconds(100);
            context.SetToken(context.SerializeTicket());
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }
    }

}