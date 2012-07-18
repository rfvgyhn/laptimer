﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using LapTimer.Services;
using LapTimer.Data;
using System.Web.Configuration;
using LapTimer.Infrastructure;

namespace LapTimer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "BySlug",
                "events/{slug}",
                new { controller = "Event", action = "ByDate", }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new { controller = "Home", action = "Index" },
                new { action = @"^[^0-9].+" }
            );

            routes.MapRoute(
                "ByDate",
                "events/{slug}/{date}/{action}",
                new { controller = "Event", action = "ByDate", date = UrlParameter.Optional },
                new { date = @"\d{4}-\d{2}-\d{2}" }
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.DefaultBinder = new DefaultDictionaryBinder();

            var assembly = typeof(MvcApplication).Assembly;
            var builder = new ContainerBuilder();            
            builder.RegisterControllers(assembly);
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
            builder.RegisterType<MongoRepository>().As<IRepository>()
                   .WithParameters(new[] { 
                       new NamedParameter("connectionString", WebConfigurationManager.ConnectionStrings["Database"].ConnectionString),
                       new NamedParameter("databaseName", WebConfigurationManager.AppSettings["DatabaseName"])
                   });            

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}