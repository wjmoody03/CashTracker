using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using ct.Data.Contexts;
using ct.Data.Repositories;
using ct.Domain;
using ct.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ct.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ConfigureDependencyInjection();
            AutoMapperConfig.Configure();
        }

        protected void ConfigureDependencyInjection()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            //api? 
            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            //custom
            //builder.RegisterInstance(new ctContext(CashTrackerConfigurationManager.AzureSQLConnectionString)).InstancePerRequest().As<IctContext>();
            builder.RegisterType<ctContext>().As<IctContext>().InstancePerRequest();
            builder.RegisterType<TransactionRepository>().As<ITransactionRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<TransactionTypeRepository>().As<ITransactionTypeRepository>();
            builder.RegisterType<AccountBalanceRepository>().As<IAccountBalanceRepository>();
            builder.RegisterType<BudgetRepository>().As<IBudgetRepository>();
            builder.RegisterType<AccountDownloadResultRepository>().As<IAccountDownloadResultRepository>();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //api:
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);



        }
    }
}
