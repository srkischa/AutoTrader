using AutoTrader.WebApi.Init;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Swashbuckle.Application;
using System.Web.Http;

[assembly: OwinStartup(typeof(AutoTrader.WebApi.Startup))]
namespace AutoTrader.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.EnableSwagger(c => c.SingleApiVersion("v1", "API for Innsio CRM"))
           .EnableSwaggerUi();

            DependencyResolverInitializer.ResolveWebApiDependencies(this.GetType().Assembly, config, app);

            //app.Use<InvalidAuthenticationMiddleware>();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}