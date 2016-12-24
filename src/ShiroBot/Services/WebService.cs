using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Discord.OAuth2;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using Newtonsoft.Json;
using ShiroBot.Services.WebService.Models;

namespace ShiroBot
{
    public class WebService
    {
        // Private variable for logging throughout webService class
        private static Logger s_log;

        // Hard coding a few static variables for web service - to be re-visisted #lol
        private const string WebServiceRoot = "/www";
        private const int WebServicePort = 5050;
        private const string WebServiceUrl = "http://localhost";

        // Configure web host
        public void BuildandRun()
        {
            // Save webhost configuration to deploy and run later
            var webHostConfiguration = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory() + WebServiceRoot)
                .UseWebRoot(Directory.GetCurrentDirectory() + WebServiceRoot)
                .UseKestrel() // Use this and run the bot behind nginx
                .UseStartup<WebService>()
                .UseUrls(WebServiceUrl + ":" + WebServicePort.ToString())
                .Build();
            webHostConfiguration.Start();
        }

        // Configure additional services to run on top of host and application
        public void ConfigureServices(IServiceCollection services)
        {
            //Add Authentication Service
            services.AddAuthentication(options =>
            {
                // Add Authentication Service
                services.AddAuthentication(siOptions =>
                {
                    siOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });

                // Add MVC functionaility -- Changed to AddMvc -- https://github.com/aspnet/Mvc/issues/2872
                services.AddMvc();

                // Insert a antiforgery token system with the cookie name "Discord-Shiro"
                services.Insert(0, ServiceDescriptor.Singleton(
                    typeof(IConfigureOptions<AntiforgeryOptions>),
                    new ConfigureOptions<AntiforgeryOptions>(afOptions => afOptions.CookieName = "Discord-Shiro")));
            });
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

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in and sign out cookie
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
                ClientSecret = "3tu-ooS5RV3uisXFy4zVnmfguQJVBujq", // Discord Application Secret Token
                CallbackPath = new PathString("/discord/login"),
                Scope = { "identify", "guilds" }, // Scopes identify and guilds
                SaveTokens = true, // Save the token "await Context.Authentication.GetTokenAsync()" (If we happen to want to grab token session) in .cshtml for example
                Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        // Create user request to /users/@me/guilds - Lists selfuser's guild list. Just inspect <GuildsModel>.
                        HttpRequestMessage userRequest = new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/users/@me/guilds");
                        // Add Header "Authorization: Bearer TOKEN"
                        userRequest.Headers.Add("Authorization", "Bearer " + context.AccessToken);
                        // Accept Type "application/json"
                        userRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        // Sends a HTTP request asynchronously.
                        HttpResponseMessage userResponse = await context.Backchannel.SendAsync(userRequest, context.HttpContext.RequestAborted);
                        // Make sure we get a success on our request, otherwise fail.
                        userResponse.EnsureSuccessStatusCode();
                        // Read the response after our request is successful to string.
                        var text = await userResponse.Content.ReadAsStringAsync();
                        // Deserialize the JSON into a readable obj.
                        var user = JsonConvert.DeserializeObject<Guilds[]>(text);

                        // Creates a new ClaimsIdentity to add to our claims list.
                        var identity = new ClaimsIdentity(
                            context.Identity.AuthenticationType,
                            ClaimsIdentity.DefaultNameClaimType,
                            ClaimsIdentity.DefaultRoleClaimType);

                        // Since our guilds is an array of guild objects we need to loop it and add the incrementing number on the claim type.
                        for (int i = 0; i < user.Length; i++)
                        {
                            var identifier = user[i].Id;
                            if (!string.IsNullOrEmpty(identifier))
                            {
                                context.Identity.AddClaim(new Claim(
                                    String.Format("urn:discord:guild:id:{0}", i), identifier,
                                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }
                            var avatar = user[i].Icon;
                            if (!string.IsNullOrEmpty(avatar))
                            {
                                context.Identity.AddClaim(new Claim(
                                    String.Format("urn:discord:guild:avatar:{0}", i), avatar,
                                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }
                            var permissions = user[i].Permissions;
                            if (!string.IsNullOrEmpty(permissions.ToString()))
                            {
                                context.Identity.AddClaim(new Claim(
                                    String.Format("urn:discord:guild:permissions:{0}", i), permissions.ToString(),
                                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }
                            var guildname = user[i].Name;
                            if (!string.IsNullOrEmpty(guildname))
                            {
                                context.Identity.AddClaim(new Claim(
                                    String.Format("urn:discord:guild:name:{0}", i), guildname,
                                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }
                        }

                        // Add the ClaimsIdentity we created to our main identity.
                        context.Identity.Actor = identity;
                    }
                }
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
