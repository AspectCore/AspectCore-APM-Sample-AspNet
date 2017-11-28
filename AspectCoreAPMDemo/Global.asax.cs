using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AspectCore.APM.ApplicationProfiler;
using AspectCore.APM.Core;
using AspectCore.APM.HttpProfiler;
using AspectCore.APM.LineProtocolCollector;
using AspectCore.APM.Profiler;
using AspectCore.Extensions.Autofac;
using AspectCore.Injector;
using Autofac;
using Autofac.Integration.Mvc;

namespace AspectCoreAPMDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            IServiceContainer services = new ServiceContainer();

            Action<ApplicationOptions> appOptions = options =>
             {
                 options.ApplicationName = "AspectCoreAPMDemo";  //配置应用名称，一定要有
                 options.Environment = "Development";            //配置应用环境，一定要有
             };

            services.AddAspectCoreAPM(compontent =>
            {
                compontent.AddApplicationProfiler();
                compontent.AddHttpProfiler();
                compontent.AddLineProtocolCollector(options =>
                {
                    options.Database = "aspectcore";
                    options.Server = "http://192.168.3.4:8086";
                });
            }, appOptions);

            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(containerBuilder.Build()));

            var lifetime = DependencyResolver.Current.GetService<IComponentLifetime>();

            lifetime.Start();  //启动AspectCoreAPM
        }
    }
}
