﻿using System.Web;
using System.Web.Optimization;

namespace LETS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jQueryLib/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jQueryLib/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/AngularJsLib/angular.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/typeahead").IncludeDirectory(
                        "~/Scripts/TypeAhead", "*.js", true));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/BootstrapJsLib/bootstrap.js",
                      "~/Scripts/BootstrapJsLib/respond.js",
                      "~/Scripts/BootstrapJsLib/bootstrap-filestyle.js",
                      "~/Scripts/mdl/material.js"));

            bundles.Add(new ScriptBundle("~/bundles/shared").IncludeDirectory(
                      "~/Scripts/Shared", "*.js", true));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/theme/bootstrap.css",
                      "~/Content/mdl/material.css",
                      //"~/Content/fontawesome/less/font-awesome.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/customSharedCSS").IncludeDirectory(
                      "~/Content/lets/shared", "*.css", true));

        }
    }
}
