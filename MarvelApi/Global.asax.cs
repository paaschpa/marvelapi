using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Funq;
using MarvelApi.Services;
using ServiceStack;

namespace MarvelApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            new AppHost().Init();
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("Marvel API", typeof(ComicsService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
        }
    }
}