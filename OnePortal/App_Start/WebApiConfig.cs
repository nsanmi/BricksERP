using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace OnePortal
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}/{id}",
           new { id = RouteParameter.Optional });
           
            //config.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}
