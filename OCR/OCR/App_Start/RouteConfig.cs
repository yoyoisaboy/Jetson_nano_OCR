using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OCR
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            // 添加這一行確保 Camera 控制器的路由能夠正確匹配
            routes.MapRoute(
                name: "Camera",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Camera", action = "StartRecognition", id = UrlParameter.Optional }
            );
        }
    }
}
