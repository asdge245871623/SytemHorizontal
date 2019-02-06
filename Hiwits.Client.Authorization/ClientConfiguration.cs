using DotNetOpenAuth.OAuth2;
using Hiwits.Authorization.Cache.SqlServer;
using Hiwits.Authorization.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiwits.Client.Authorization
{

    public class ClientConfiguration
    {
        private static ClientConfiguration _instance { get; set; }

        public static ClientConfiguration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ClientConfiguration();

                return _instance;
            }
        }

        public AuthorizationServer authServer { get; set; }

        public AuthorizationServerDescription authServerDescription { get; protected set; }

        public WebServerClient webServerClient { get; protected set; }

        public string Id { get; set; }

        public string Secret { get; set; }

        public ClientConfiguration()
        {
            try
            {
                string configPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\app.sso.client.config";
                var appSettings = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = configPath }, ConfigurationUserLevel.None).AppSettings;

                string baseAddress = appSettings.Settings["BaseAddress"].Value,
                       authorizePath = appSettings.Settings["AuthorizePath"].Value,
                       tokenPath = appSettings.Settings["TokenPath"].Value;


                if (string.IsNullOrWhiteSpace(baseAddress) || string.IsNullOrWhiteSpace(authorizePath) ||
                                                                            string.IsNullOrWhiteSpace(tokenPath))
                    throw new MissingFieldException($"app.sso.client 未配置授权服务器信息： BaseAddress:{baseAddress},AuthorizePath:{authorizePath},TokenPath:{tokenPath}");


                this.authServer = new AuthorizationServer()
                {
                    BaseAddress = baseAddress,
                    AuthorizePath = authorizePath,
                    TokenPath = tokenPath
                };

                this.Id = appSettings.Settings["ClientId"].Value;
                this.Secret = appSettings.Settings["ClientSecret"].Value;

                if (string.IsNullOrWhiteSpace(this.Id) || string.IsNullOrWhiteSpace(this.Secret))
                    throw new MissingFieldException($"app.sso.client 未配置单点客户端信息： ClientId:{this.Id},ClientSecret:{this.Secret}");

                //授权服务器设置
                var authorizationServerUri = new Uri(authServer.BaseAddress);
                this.authServerDescription = new AuthorizationServerDescription
                {
                    AuthorizationEndpoint = new Uri(authorizationServerUri, authServer.AuthorizePath),
                    TokenEndpoint = new Uri(authorizationServerUri, authServer.TokenPath)
                };

                this.webServerClient = new WebServerClient(this.authServerDescription, this.Id, this.Secret);

            }
            catch (MissingFieldException ex)
            {
                throw ex;
            }
        }


        public UserTokenRelationship ExchangeUserCredentialForToken(string username, string password)
        {
            //从这里配置角色权限或者直接从授权服务器里配置（建议从授权服务器） ExchangeUserCredentialForToken scopes 可以不传。
            IEnumerable<string> scopes = new List<string>() { "plsw.admin" };

            return ExchangeUserCredentialForToken(username, password, scopes);
        }

        public UserTokenRelationship ExchangeUserCredentialForToken(string username, string password, IEnumerable<string> scopes)
        {
            UserTokenRelationship tokenRelf = null;
            try
            {
                var state = this.webServerClient.ExchangeUserCredentialForToken(username, password, scopes);
                if (state == null)
                    throw new LoginAuthorizationException();


                //授权Token数据库持久化
                tokenRelf = UserTokenRelationshipSvc.Instance.Get(username);

                if (tokenRelf == null)
                {
                    tokenRelf = new UserTokenRelationship() { Name = username };
                    UserTokenRelationshipSvc.Instance.Add(tokenRelf);
                }

                tokenRelf.AccessToken = state.AccessToken;
                tokenRelf.RefreshToken = state.RefreshToken;
                UserTokenRelationshipSvc.Instance.Update(tokenRelf);

                return tokenRelf;
            }
            catch (LoginAuthorizationException ex)
            {
                //抛出登录失败异常
                throw ex;
            }

        }

    }

    public class AuthorizationServer
    {
        public string BaseAddress { get; set; }

        public string AuthorizePath { get; set; }

        public string TokenPath { get; set; }
    }

    public class AuthorizationTokenRelationship
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
