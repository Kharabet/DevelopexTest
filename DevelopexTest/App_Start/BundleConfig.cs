using System.Web;
using System.Web.Optimization;

namespace DevelopexTest
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var cssCommon = new[]
            {
                "~/Content/bootstrap.css",
                "~/Content/site.css",
            }; 

            var scriptsCommon = new[]
            {
            
                 "~/Scripts/jquery-3.3.1.js",
                "~/Scripts/jquery.signalR-2.3.0.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/index.js",
                
            };

            bundles.Add(new ScriptBundle("~/bundles/js/").Include(scriptsCommon));
            bundles.Add(new StyleBundle("~/bundles/css").Include(cssCommon));
        }
    }
}
