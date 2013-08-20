using System.Web;
using System.Web.Optimization;

namespace gFit
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/Libraries/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/Libraries/knockout-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymobile").Include(
                        "~/Scripts/Libraries/jquery.mobile-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/Libraries/jquery.unobtrusive*",
                        "~/Scripts/Libraries/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/Libraries/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));


            bundles.Add(new StyleBundle("~/Content/themes/custom/css").Include(
                        "~/Content/themes/custom/jquery.mobile.structure-{version}.css",
                        "~/Content/themes/custom/jquerymobile_customtheme1.css"));

        }
    }
}