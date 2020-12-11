using System.Web.Mvc;
using System.Web.Routing;

namespace OnePortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.MapRoute(
                name: "DefaultStep",
                url: "{controller}/{action}/{id}/{step_id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, step_id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
