using Constants;
using DotNetOpenAuth.OAuth2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Hiwits.SSOServer.WebApp.Controllers
{

    public class AccountController : Controller
    {
        private static ConcurrentDictionary<string, string> _tokenDir = new ConcurrentDictionary<string, string>();

        private void InitializeWebServerClient()
        {
            var authorizationServerUri = new Uri(Paths.AuthorizationServerBaseAddress);
            var authorizationServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(authorizationServerUri, Paths.AuthorizePath),
                TokenEndpoint = new Uri(authorizationServerUri, Paths.TokenPath)
            };
            _webServerClient = new WebServerClient(authorizationServer, Clients.Client1.Id, Clients.Client1.Secret);
        }

        private WebServerClient _webServerClient;

        // GET: Account
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "")
        {
            var tokenCookie = Request.Cookies["access_token"];
            if (tokenCookie == null)
                return View();

            string accessToken = Request.Cookies["access_token"].Value;
            string refreshToken = "";

            if (_tokenDir.TryRemove(accessToken, out refreshToken))
            {
                var state = new AuthorizationState
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                try
                {
                    if (_webServerClient.RefreshAuthorization(state))
                    {
                        _tokenDir[state.AccessToken] = state.RefreshToken;
                        Response.SetCookie(new HttpCookie("access_token", state.AccessToken));

                        if (string.IsNullOrWhiteSpace(returnUrl))
                            return Redirect("~/Home");

                        return Redirect(returnUrl);
                    }
                }
                catch (Exception ex)
                {
                    Response.Cookies.Remove("access_token");
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string returnUrl = "")
        {
            InitializeWebServerClient();

            //var returnUrl = Request.QueryString["ReturnUrl"];

            var state = _webServerClient.ExchangeUserCredentialForToken(username, password, scopes: new string[] { "bio" });

            _tokenDir[state.AccessToken] = state.RefreshToken;

            Response.Cookies.Add(new HttpCookie("access_token", state.AccessToken));
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return Redirect("~/Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LoginOut()
        {
            Response.Cookies.Remove("access_token");
            return View();
        }
    }
}