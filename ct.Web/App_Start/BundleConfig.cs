﻿using System.Web;
using System.Web.Optimization;

namespace ct.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").IncludeDirectory("~/app", "*.js", true));

            bundles.Add(new StyleBundle("~/Content/css")
                    //.Include("~/Content/bootstrap/bootstrap.css")
                    .Include("~/Content/bootstrap/bootswatch.flatly.min.css")
                    //.Include("~/Content/bootstrap/bootstrap-theme.css")
                    .Include("~/Content/site.css"));

        }
    }
}