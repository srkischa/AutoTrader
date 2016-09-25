using AutoTrader.WebApi.Init;
using AutoTrader.WebApi.Middleware;
using AutoTrader.WebApi.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Swashbuckle.Application;
using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

[assembly: OwinStartup(typeof(AutoTrader.WebApi.Startup))]
namespace AutoTrader.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            //todo: add GlobalExceptionHandler
            //config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            config.EnableSwagger(c => c.SingleApiVersion("v1", "API for AutoTrader"))
           .EnableSwaggerUi();

            DependencyResolverInitializer.ResolveWebApiDependencies(GetType().Assembly, config, app);

            app.Use<InvalidAuthenticationMiddleware>();

            var issuer = "http://localhost:59822";//remove hardcore url
            ConfigureOAuthTokenGeneration(app, issuer);
            ConfigureOAuthTokenConsumption(app, issuer);

            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app, string issuer)
        {
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app, string issuer)
        {
            var options = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(5),
                Provider = new AutoTraderAuthorizationServerProvider(),
                AccessTokenFormat = new CustomJwtFormat(issuer)
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}