using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Configuration;
using Hangfire.SqlServer;
using Hangfire;
using System.Web.Http;
using System.Web;
using Hangfire.Dashboard;
using System.Net;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

[assembly: OwinStartup(typeof(xPort5.Bot.Startup))]

namespace xPort5.Bot
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            #region When making calls to Rest services, deactivating the NagleAlgorithm can reduce latency when the data transferred over the network is small.
            // refer: https://alexandrebrisebois.wordpress.com/2013/03/24/why-are-webrequests-throttled-i-want-more-throughput/
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 1000;
            #endregion

            #region Initialize log4net with local configuration file when other methods not working
            // Refer: https://stackify.com/making-log4net-net-core-work/?utm_referrer=https%3A%2F%2Fwww.google.ca%2F
            //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            //XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // Refer: https://stackoverflow.com/questions/21166126/log4net-separate-config-file-not-working
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "logging.config"));
            #endregion

            #region Initialize Hangfire

            #region database connection string and custom schema
            // simple
            //GlobalConfiguration.Configuration.UseSqlServerStorage("SysDb");

            // more control options
            var conn = ConfigurationManager.ConnectionStrings["x5db"].ConnectionString;
            var options = new SqlServerStorageOptions
            {
                SchemaName = "Hangfire.x5.Bot"
            };
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage(conn, options);
            #endregion

            #region Hangfire 祇容許 localhost，要用 MyAuthorizationFilter 搞
            // refer: http://docs.hangfire.io/en/latest/configuration/using-dashboard.html?highlight=authorization#configuring-authorization
            var dashOptions = new DashboardOptions {
                AppPath = VirtualPathUtility.ToAbsolute("~"),
                Authorization = new[]
                {
                    new MyAuthorizationFilter()
                }
            };
            app.UseHangfireDashboard("/hangfire", dashOptions);
            #endregion

            app.UseHangfireServer();
            #endregion

            var config = new HttpConfiguration();

            SwaggerConfig.Register(config);

            WebApiConfig.Register(config);

            app.UseWebApi(config);
        }

        public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                // In case you need an OWIN context, use the next line, `OwinContext` class
                // is the part of the `Microsoft.Owin` package.
                var owinContext = new OwinContext(context.GetOwinEnvironment());
                
                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                //return owinContext.Authentication.User.Identity.IsAuthenticated;
                //
                // allow anonymous
                return true;
            }
        }
    }
}
