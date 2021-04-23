
Imports System.Web
Imports System.Web.Optimization

Namespace WebLabelPrint
   Public Class BundleConfig
      ' For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
      Public Shared Sub RegisterBundles(bundles As BundleCollection)
         bundles.Add(New ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"))

         bundles.Add(New ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui-{version}.js"))

         bundles.Add(New ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.unobtrusive*", "~/Scripts/jquery.validate*"))

         ' Use the development version of Modernizr to develop with and learn from. Then, when you're
         ' ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
         bundles.Add(New ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"))

         bundles.Add(New StyleBundle("~/Content/css").Include("~/Content/normalize.css", "~/Content/site.css"))

         bundles.Add(New StyleBundle("~/Content/themes/base/css").Include("~/Content/themes/base/jquery.ui.core.css",
            "~/Content/themes/base/jquery.ui.resizable.css", _
            "~/Content/themes/base/jquery.ui.selectable.css", _
            "~/Content/themes/base/jquery.ui.accordion.css", _
            "~/Content/themes/base/jquery.ui.autocomplete.css", _
            "~/Content/themes/base/jquery.ui.button.css", _
            "~/Content/themes/base/jquery.ui.dialog.css", _
            "~/Content/themes/base/jquery.ui.slider.css", _
            "~/Content/themes/base/jquery.ui.tabs.css", _
            "~/Content/themes/base/jquery.ui.datepicker.css", _
            "~/Content/themes/base/jquery.ui.progressbar.css", _
            "~/Content/themes/base/jquery.ui.theme.css"))
      End Sub
   End Class
End Namespace