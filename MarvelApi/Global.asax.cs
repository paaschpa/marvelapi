using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Funq;
using MarvelApi.Services;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Mvc;
using ServiceStack.Redis;

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
            var redisCon = ConfigurationManager.AppSettings["redisUrl"].ToString();
            container.Register<IRedisClientsManager>(new PooledRedisClientManager(20, 60, redisCon));
            container.Register<ICacheClient>(c => (ICacheClient)c.Resolve<IRedisClientsManager>().GetCacheClient());

            //Set MVC to use the same Funq IOC as ServiceStack
            ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
        }
    }
}