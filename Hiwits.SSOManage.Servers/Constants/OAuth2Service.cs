using DotNetOpenAuth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
   public class OAuth2Service
    {
        private static OAuth2Service _Instance { get; set; }

        public static OAuth2Service Instance {
            get
            {
                if (_Instance == null)
                    _Instance = new OAuth2Service();
                return _Instance;
            }
        }

        private static WebServerClient _webServerClient;

        public OAuth2Service()
        {
            InitializeWebServerClient();
        }

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

        public string RefreshAuthorization(string identityName,string accessToken)
        {
            //根据参数identityName 从redis 获取到JSON (accessToken,reflashToken)
            //使用NewtonSoft.Json解析成AuthorizationState
            var state = new AuthorizationState //替换成refreshToken
            {
                AccessToken = accessToken,
                RefreshToken = "aasd" 
            };

            //比较accessToken 是否和redis中取出的一致、如果accessToken不一致，表示被第二人从别处登录
            //返回accessToken ="":
            try
            {
                if (_webServerClient.RefreshAuthorization(state))
                {
                    //更新redis 用户Token  键值对
                    // 更新操作
                    accessToken = state.AccessToken;
                }
            }
            catch (Exception ex)
            {
                accessToken = "";
            }

            return accessToken;
        }

    }
}
