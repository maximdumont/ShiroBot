using System.IO;
using Discord.OAuth2;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;

namespace ShiroBot
{
    public class WebService
    {
        // Private variable for logging throughout webService class
        private static Logger _log;

        // Hard coding a few static variables for web service - to be re-visisted #lol
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
            //Add Authentication Service
            services.AddAuthentication(options => {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            // Add MVC functionaility -- Changed to AddMvc -- https://github.com/aspnet/Mvc/issues/2872
            services.AddMvc();

            // Insert a antiforgery token system with the cookie name "Discord-Shiro"
            services.Insert(0, ServiceDescriptor.Singleton(
                typeof(IConfigureOptions<AntiforgeryOptions>),
                new ConfigureOptions<AntiforgeryOptions>(options => options.CookieName = "Discord-Shiro")));
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

            // Use Static Files
            app.UseStaticFiles();

            // Add Cookie Authentication for signing in and signing out
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                LoginPath = new PathString("/signin"),
                LogoutPath = new PathString("/signout")
            });

            // Add Discord OAuth2 - Be sure to set the redirect url in your discord app to correct Url+CallbackPath.
            app.UseDiscordAuthentication(new DiscordOptions
            {
                DisplayName = "ShiroBot Discord Authentication",
                ClientId = "259132170604380161", // Discord Application ID
                ClientSecret = "xOjnXMUnDcbl3CBe8ZRxf1DqNOeR6xqN", // Discord Application Secret Token
                CallbackPath = new PathString("/discord/login"),
                Scope = { "identify", "email" }
            });


            // Enable MVC
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
