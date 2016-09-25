using System;
using System.Reflection;
using Autofac;
using AutoTrader.Service;
using AutoTrader.Service.Identity;
using Microsoft.AspNet.Identity;
using Module = Autofac.Module;

namespace AutoTrader.WebApi.Init
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<UserStore>().As<IUserStore<ApplicationUser, int>>().InstancePerLifetimeScope();
            builder.RegisterType<UserIdentityManagerService>().As<IUserIdentityManagerService>().InstancePerLifetimeScope();

            RegisterServiceLayer(builder, typeof(UserService).Assembly);
            RegisterServiceLayer(builder, typeof(UserIdentityManagerService).Assembly);

            builder.RegisterType<CurrentUserProvider>()
                .AsImplementedInterfaces()
                .InstancePerRequest();

            builder.RegisterType<AutoTraderConfigurationSettings>()
                .AsImplementedInterfaces()
                .InstancePerRequest();

            builder.RegisterType<IdentityMessageService>()
                .AsImplementedInterfaces()
                .InstancePerRequest();
        }

        private static void RegisterServiceLayer(ContainerBuilder builder, Assembly serviceAssembly)
        {
            builder.RegisterAssemblyTypes(serviceAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
