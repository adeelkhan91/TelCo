using System.Web.Http;

namespace TelcoAPIService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
            name: "route1",
                routeTemplate: "",
                defaults: new { controller = "Base", action = "Get" }
            );

            /*config.Routes.MapHttpRoute(
            name: "route2",
                routeTemplate: "api/{*controller}",
                defaults: new { controller = "Base", action = "Get" }
            );*/

            config.Routes.MapHttpRoute(
            name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Formatters.XmlFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));
        }
    }
}
