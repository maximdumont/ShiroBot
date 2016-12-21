using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace ShiroBot
{
    public class WebService
    {
        // Private variable for logging throughout webService class
        private static Logger _log;

        // Hard coding a few static variables for web service - to be re-visisted
        private const string webServiceRoot = "/www";
        private const int webServicePort = 5050;
        private const string webServiceUrl = "http://localhost";

        // Configure web host
        public void BuildandRun()
        {
            // Save webhost configuration to deploy and run later
            var _webHostConfiguration = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory() + webServiceRoot)
                .UseWebRoot(Directory.GetCurrentDirectory() + webServiceRoot)
                .UseKestrel() // Use this and run the bot behind nginx
                .UseStartup<WebService>()
                .UseUrls(webServiceUrl + ":" + webServicePort.ToString())
                .Build();

            _webHostConfiguration.Run();
        }

        // Configure additional services to run on top of host and application
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(); // Add MVC functionaility 
        }

        // Configure the webService
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Setup logger interface to use Nlog
            loggerFactory.AddNLog();
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            // For now enable debugging options and functions whilst we're testing webService 
            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();

            // Enable MVC
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Test}/{action=Index}/{id?}");
            });

        }
    }
}