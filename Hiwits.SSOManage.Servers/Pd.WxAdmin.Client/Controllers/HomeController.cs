using Pd.WxAdmin.Client.filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Pd.WxAdmin.Client.Controllers
{
    [SSOAuthorize]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var username = this.User.Identity.Name;

            ParameterExpression number = Expression.Parameter(typeof(int), "number");

            BlockExpression myBlock = Expression.Block(
                new[] { number },
                Expression.Assign(number, Expression.Constant(2)),
                Expression.AddAssign(number, Expression.Constant(6)),
                Expression.DivideAssign(number, Expression.Constant(2)));

            Expression<Func<int>> myAction = Expression.Lambda<Func<int>>(myBlock);
            Console.WriteLine(myAction.Compile()());

            return View();
        }
    }
}