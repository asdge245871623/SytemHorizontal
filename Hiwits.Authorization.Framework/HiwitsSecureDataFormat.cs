using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Hiwits.Authorization.Framework
{
    public class HiwitsSecureDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        public string Protect(AuthenticationTicket data)
        {
            var prottData = new ProtectedData()
            {
                properties = new ProtectedData.AuthenticationProperties()
                {
                    expiresUtc = data.Properties.ExpiresUtc,
                    issuedUtc = data.Properties.IssuedUtc
                }
            };

            prottData.claims = data.Identity.Claims.Select(c => new ProtectedData.Claim() { type = c.Type, value = c.Value });

            return  TripleDESCryptoService.encryptToBase64(JsonConvert.SerializeObject(prottData));
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            var prottData = JsonConvert.DeserializeObject<ProtectedData>(TripleDESCryptoService.decryptFromBase64(protectedText));

            var identity = new ClaimsIdentity(prottData.claims.Select(c => new Claim(c.type, c.value)),
                                                AuthenticationType.Beaer, ProtectedData.nameType, ProtectedData.roleType);

            var props = new AuthenticationProperties() { ExpiresUtc = prottData.properties.expiresUtc, IssuedUtc = prottData.properties.issuedUtc };

            return new AuthenticationTicket(identity, props);
        }
    }


    public class ProtectedData
    {
        [JsonIgnore]
        public const string nameType = "au:name";

        [JsonIgnore]
        public const string roleType = "au:role";


        [JsonProperty("c")]
        public IEnumerable<Claim> claims { get; set; }

        [JsonProperty("p")]
        public AuthenticationProperties properties { get; set; }

        public class Claim
        {
            /// <summary>
            /// Type
            /// </summary>
            [JsonProperty("t")]
            public string type { get; set; }

            /// <summary>
            /// Value
            /// </summary>
            [JsonProperty("v")]
            public string value { get; set; }
        }

        public class AuthenticationProperties
        {
            /// <summary>
            /// ExpiresUtc 过期时间
            /// </summary>
            [JsonProperty("eu")]
            public DateTimeOffset? expiresUtc { get; set; }

            /// <summary>
            /// IssuedUtc 发行时间
            /// </summary>
            [JsonProperty("iu")]
            public DateTimeOffset? issuedUtc { get; set; }
        }

    }
}
