using System.Web.Http;
using WebApiPerformance.Filters;

namespace WebApiPerformance
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // JIL
            //config.Formatters.RemoveAt(0);
            //config.Formatters.Insert(0, new JilFormatter());
        }
    }
}
