using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MyUni.Web.Infrastructure
{
    public static class CustomHtmlHelpers
    {
        public static MvcHtmlString DateControl<T, U>(this HtmlHelper<T> htmlHelper, Expression<Func<T, U>> propertyExpression)
        {
            var modelMetaData = ModelMetadata.FromLambdaExpression(propertyExpression, htmlHelper.ViewData);
            var propertyName = modelMetaData.PropertyName;
            var test = "";

            var divTag = new TagBuilder("div");
            divTag.MergeAttribute("id", string.Format("{0}Div", propertyName));
            divTag.AddCssClass("input-group date");

            var calendarAddonIconTag = new TagBuilder("span");
            calendarAddonIconTag.AddCssClass("glyphicon glyphicon-calendar");

            var calendarAddonSpanTag = new TagBuilder("span");
            calendarAddonSpanTag.AddCssClass("input-group-addon add-on");
            calendarAddonSpanTag.InnerHtml = calendarAddonIconTag.ToString();

            var txtBox = FormTextBoxFor(htmlHelper, propertyExpression, placeHolder: "Enrollment date is required");

            divTag.InnerHtml = string.Format("{0}{1}", txtBox, calendarAddonSpanTag);

            return MvcHtmlString.Create(divTag.ToString());

        }

        public static MvcHtmlString SubmitButton(this HtmlHelper htmlHelper, string buttonName, string buttonText, string buttonCss = "", string iconCss = "")
        {
            var btn = new TagBuilder("button");
            btn.MergeAttribute("type", "submit");

            if (!string.IsNullOrEmpty(buttonName))
            {
                btn.MergeAttribute("id", buttonName);
                btn.MergeAttribute("name", buttonName);
            }

            btn.AddCssClass(string.Format("btn {0}", buttonCss));

            if (string.IsNullOrEmpty(iconCss))
            {
                btn.InnerHtml = buttonText;

                return MvcHtmlString.Create(btn.ToString());
            }

            var iTag = new TagBuilder("i");
            iTag.AddCssClass(string.Format("fa {0}", iconCss));

            btn.InnerHtml = string.Format("{0} {1}", iTag, buttonText);

            return MvcHtmlString.Create(btn.ToString());

        }

        public static MvcHtmlString LinkButton(this HtmlHelper htmlHelper, string url, string buttonText, string buttonCss = "", string iconCss = "")
        {
            var btn = new TagBuilder("a");
            btn.MergeAttribute("href", string.IsNullOrEmpty(url) ? "#" : url);

            btn.AddCssClass(string.Format("btn {0}", buttonCss));

            if (string.IsNullOrEmpty(iconCss))
            {
                btn.InnerHtml = buttonText;

                return MvcHtmlString.Create(btn.ToString());
            }

            var iTag = new TagBuilder("i");
            iTag.AddCssClass(string.Format("fa {0}", iconCss));

            btn.InnerHtml = string.Format("{0} {1}", iTag, buttonText);

            return MvcHtmlString.Create(btn.ToString());
        }

        public static MvcHtmlString FormTextBoxFor<T, U>(this HtmlHelper<T> htmlHelper, Expression<Func<T, U>> propertyExpression, string textBoxCss = "", string placeHolder = "")
        {
            return htmlHelper.TextBoxFor(propertyExpression, new { @class = string.Format("form-control {0}", textBoxCss), placeHolder = placeHolder });
        }
    }
}