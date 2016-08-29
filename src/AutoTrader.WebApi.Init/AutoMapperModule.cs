using Autofac;
using Autofac.Core;
using Autofac.Integration.Owin;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AutoTrader.WebApi.Init
{
    internal class AutoMapperModule : Autofac.Module
    {
        public Assembly WebProjectAssembly { get; set; }

        public IEnumerable<Assembly> Assemblies
        {
            get
            {
                yield return WebProjectAssembly;
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            new List<Type>
            {
                typeof (ITypeConverter<,>),
                typeof (IValueResolver<,,>),
                typeof (IMappingAction<,>)
                //IMemberValueResolver
            }.ForEach(type =>
                builder.RegisterAssemblyTypes(WebProjectAssembly)
                    .AsClosedTypesOf(type)
                    .AsSelf()
                    .InstancePerLifetimeScope()
                );

            var profiles = from t in Assemblies.SelectMany(assembly => assembly.GetTypes())
                           where typeof(Profile).IsAssignableFrom(t)
                           select (Profile)Activator.CreateInstance(t);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(type =>
                {
                    var owinContext = HttpContext.Current.Request.GetOwinContext();
                    var lifeTimeScope = owinContext.GetAutofacLifetimeScope() ?? (ILifetimeScope)HttpContext.Current.Items[typeof(ILifetimeScope)];
                    return lifeTimeScope.Resolve(type);
                });

                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            config.AssertConfigurationIsValid();

            builder.RegisterInstance(config);

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>().InstancePerLifetimeScope();
        }
    }
}