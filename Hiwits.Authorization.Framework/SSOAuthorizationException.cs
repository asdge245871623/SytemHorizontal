using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiwits.Authorization.Framework
{
    public class LoginAuthorizationException:Exception
    {
        public LoginAuthorizationException() : base("授权服务器验证[ 未通过或授权异常 ]") { }
        public LoginAuthorizationException(string msg) : base($"授权服务器验证:[ {msg} ]") { }

    }

  
}
