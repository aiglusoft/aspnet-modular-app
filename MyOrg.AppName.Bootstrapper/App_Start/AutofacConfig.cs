using Autofac;
using Autofac.Integration.WebApi;
using MyOrg.AppName.Shared.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace MyOrg.AppName.Bootstrapper.App_Start
{

    public class AutofacConfig
    {
        private static IContainer container;

        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }


        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            //Register your Web API controllers.  
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.AddEventSourcing();

 
            //Set the dependency resolver to be Autofac.  
            container = builder.Build();

            return container;
        }
    }
}