using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;
using System.Web.Http.Cors;
using System.Web.Http;

namespace AutoTrader.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configure Web API to use only bearer token authentication.
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //todo: add this when you finish back office!!!
            //config.Filters.Add(new AuthorizeAttribute());

            //config.Filters.Add(new ModelValidationErrorHandlerFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "swagger_root",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger")
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // enable requests from everywhere
            config.EnableCors(new EnableCorsAttribute("*", "*", "*") { PreflightMaxAge = 60 * 60 * 24 });
            // When converting c# properties to JSON, use camelCase
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            // json dates will be in UTC
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }
    }
}
