using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoTrader.Service.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Owin;
using Microsoft.Owin.Security.DataProtection;

namespace AutoTrader.WebApi.Init
{
    public static class DependencyResolverInitializer
    {
        public static void ResolveWebApiDependencies(
            Assembly webApiAssembly,
            HttpConfiguration configuration,
            IAppBuilder appBuilder)
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(webApiAssembly).InstancePerRequest();
            builder.RegisterWebApiFilterProvider(configuration);

            RegisterCommonDependencies(builder, webApiAssembly);

            builder.Register<IDataProtectionProvider>(c => appBuilder.GetDataProtectionProvider()).InstancePerRequest();

            IContainer container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            configuration.DependencyResolver = resolver;

            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(configuration);
        }

        private static void RegisterCommonDependencies(ContainerBuilder builder, Assembly webProjectAssembly)
        {
            builder.RegisterModule(new AutoMapperModule { WebProjectAssembly = webProjectAssembly });
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<LogInjectionModule>();
            builder.RegisterModule<EntityFrameworkModule>();

            //https://developingsoftware.com/configuring-autofac-to-work-with-the-aspnet-identity-framework-in-mvc-5
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.RegisterType<UserStore>().As<IUserStore<ApplicationUser, int>>().InstancePerLifetimeScope();
            builder.Register(c => HttpContext.Current.User).InstancePerRequest();
        }
    }
}
