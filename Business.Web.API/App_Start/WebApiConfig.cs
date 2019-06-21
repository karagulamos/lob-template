using System.Web.Http;
using Newtonsoft.Json;
using Swashbuckle.Application;

namespace Business.Web.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableSwagger(c => c.SingleApiVersion("v1", "Business.Web.API"))
                  .EnableSwaggerUi();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            SetJsonSettings(config);
        }

        private static void SetJsonSettings(HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var formatter = config.Formatters.JsonFormatter;

            //formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            formatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }
    }
}
