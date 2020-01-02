using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ASP_NET_FilRouge
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Sellers menu",
               url: "Sellers/Crud/{id}",
               defaults: new { controller = "Sellers", action = "Crud", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Customers menu",
                url: "Customers/Crud/{id}",
                defaults: new { controller = "Customers", action = "Crud", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Orders menu",
                url: "Orders/Crud/{id}",
                defaults: new { controller = "Orders", action = "Crud", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Bicycles menu",
                url: "Bicycles/Crud/{id}",
                defaults: new { controller = "Bicycles", action = "Crud", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Bicycles create",
                url: "Bicycles/Create/{id}",
                defaults: new { controller = "Bicycles", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Sellers index",
                url: "Sellers/Index/{id}",
                defaults: new { controller = "Sellers", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Customers index",
                url: "Customers/Index/{id}",
                defaults: new { controller = "Customers", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Orders index",
                url: "Orders/Index/{id}",
                defaults: new { controller = "Orders", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Bicycles index",
                url: "Bicycles/Index/{id}",
                defaults: new { controller = "Bicycles", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
