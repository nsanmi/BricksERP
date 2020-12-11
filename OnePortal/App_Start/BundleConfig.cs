using System.Web;
using System.Web.Optimization;

namespace OnePortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/libs/jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/libs/jquery/plugins/validate/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/themeplugins").Include(
                        "~/Content/libs/jquery/plugins/metisMenu/jquery.metisMenu*", 
                        "~/Content/libs/jquery/plugins/pace/pace*", 
                        "~/Content/libs/jquery/plugins/slimscroll/jquery.slimscroll*",
                        "~/Content/libs/theme/js/inspinia*"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/libs/other/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/libs/bootstrap/js/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/themecss").Include(
                      "~/Content/libs/bootstrap/css/bootstrap.css",
                      "~/Content/libs/font-awesome/css/font-awesome.css",
                      "~/Content/libs/animate/animate.css",
                      "~/Content/libs/theme/css/inspinia.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/site.css"));
        }
    }
}
