using System.Web;
using System.Web.Optimization;

namespace MyUni.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));


            #region CSS Bundles

            //
            // AdminLTE theme
            //
            bundles.Add(new StyleBundle("~/adminltecss").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/ionicons.css",
                "~/Content/AdminLTE.css",
                "~/Content/skins/skin-blue-light.css",
                "~/Content/Site.css"
                ));

            //
            // Bootstrap date time picker
            //
            bundles.Add(new StyleBundle("~/datetimepickercss").Include(
               "~/Content/boostrap-datetimepicker.css"
               ));

            #endregion
            

            #region Script Bundles

            //
            // Admin LTE scripts
            //
            bundles.Add(new ScriptBundle("~/bundles/adminltejs").Include(
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/adminlte.js"
                      ));

            //
            // Bootstrap date time picker scripts
            //
            bundles.Add(new ScriptBundle("~/bundles/datetimepickerjs").Include(
                "~/Scripts/moment.js",
                "~/Scripts/bootstrap-datetimepicker.js"
                ));

            #endregion

        }
    }
}
