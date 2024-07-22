using System.Web;
using System.Web.Optimization;

namespace iSynergy
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

            bundles.Add(new ScriptBundle("~/bundles/cryptonia_theme").Include(
                      "~/Content/cryptonia_theme/assets/js/jquery-1.11.2.min.js",
                        "~/Content/cryptonia_theme/assets/js/jquery.easing.min.js",
                        "~/Content/cryptonia_theme/assets/plugins/bootstrap/js/bootstrap.min.js",
                        "~/Content/cryptonia_theme/assets/plugins/pace/pace.min.js",
                        "~/Content/cryptonia_theme/assets/plugins/perfect-scrollbar/perfect-scrollbar.min.js",
                        "~/Content/cryptonia_theme/assets/plugins/viewport/viewportchecker.js",
                        "~/Content/cryptonia_theme/assets/plugins/echarts/echarts-custom-for-dashboard.js",
                        "~/Content/cryptonia_theme/assets/plugins/flot-chart/jquery.flot.js",
                        "~/Content/cryptonia_theme/assets/plugins/flot-chart/jquery.flot.time.js",
                        "~/Content/cryptonia_theme/assets/js/chart-flot.js",
                        "~/Content/cryptonia_theme/assets/plugins/morris-chart/js/raphael-min.js",
                        "~/Content/cryptonia_theme/assets/plugins/morris-chart/js/morris.min.js",
                        "~/Content/cryptonia_theme/assets/js/chart-morris.js",
                        "~/Content/cryptonia_theme/assets/js/scripts.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/cryptonia_theme/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                        "~/Content/cryptonia_theme/assets/plugins/pace/pace-theme-flash.css",
                        "~/Content/cryptonia_theme/assets/plugins/bootstrap/css/bootstrap.min.css",
                        "~/Content/cryptonia_theme/assets/plugins/bootstrap/css/bootstrap-theme.min.css",
                        "~/Content/cryptonia_theme/assets/fonts/font-awesome/css/fontawesome.css",
                        "~/Content/cryptonia_theme/assets/fonts/webfont/cryptocoins.css",
                        "~/Content/cryptonia_theme/assets/css/animate.min.css",
                        "~/Content/cryptonia_theme/assets/plugins/perfect-scrollbar/perfect-scrollbar.css",
                        "~/Content/cryptonia_theme/assets/plugins/jvectormap/jquery-jvectormap-2.0.1.css",
                        "~/Content/cryptonia_theme/assets/plugins/morris-chart/css/morris.css",
                        "~/Content/cryptonia_theme/assets/css/style.css",
                        "~/Content/cryptonia_theme/assets/css/responsive.css"
                      ));

        }
    }
}
