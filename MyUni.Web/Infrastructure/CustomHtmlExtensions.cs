using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MyUni.Web.Infrastructure
{
    public static class CustomHtmlExtensions
    {
        //public static MvcHtmlString GetActionLink<T,U>(this HtmlHelper htmlHelper, Expression<Func<T,U>> actionMethodExpression, object parameters) where T:Controller where U:ActionResult
        //{
        //    var actionMethodName = ExpressionHelper.GetExpressionText(actionMethodExpression);

        //    var controllerClass = typeof (T).Name;
        //    var controller = controllerClass.Substring(0,
        //        controllerClass.IndexOf("Controller", StringComparison.CurrentCulture));

        //    return htmlHelper.Action(actionMethodName, controller, parameters);
        //}

        //public static void Test<T, U>(HtmlHelper<T> htmlHelper, Func<T, U> expression, object parameters) where T : Controller where U : ActionResult
        //{
            
        //    Expression.Lambda<Func<T,U>>()
        //    //var actionMethodName = ExpressionHelper.GetExpressionText(expression);

        //    //htmlHelper.Action(actionMethodName, parameters);
        //}
    }
}