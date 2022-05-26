using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyOrg.AppName.Bootstrapper.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

         
        }
    }
}