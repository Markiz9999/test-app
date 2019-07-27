using System.Web;
using System.Web.Optimization;

namespace WebApplication1
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/customers").Include(
                "~/Scripts/Home/customers.js"));

            bundles.Add(new ScriptBundle("~/bundles/customer").Include(
                "~/Scripts/Home/customer.js"));

            bundles.Add(new StyleBundle("~/Content/Account/css").Include(
                "~/fonts/font-awesome/css/all.css",
                "~/Content/site.css",
                "~/Content/Account/login.css"));

            bundles.Add(new StyleBundle("~/Content/Customers/css").Include(
                "~/fonts/font-awesome/css/all.css",
                "~/Content/site.css",
                "~/Content/Home/Header.css",
                "~/Content/Home/Account.css",
                "~/Content/Home/Customers.css"));

            bundles.Add(new StyleBundle("~/Content/CustomerRoles/css").Include(
                "~/fonts/font-awesome/css/all.css",
                "~/Content/site.css",
                "~/Content/Home/Header.css",
                "~/Content/Home/Account.css",
                "~/Content/Home/CustomerRoles.css"));

            bundles.Add(new StyleBundle("~/Content/Customer/css").Include(
                "~/fonts/font-awesome/css/all.css",
                "~/Content/site.css",
                "~/Content/Home/customer.css",
                "~/Content/Home/Header.css",
                "~/Content/Home/Account.css"));
        }
    }
}
