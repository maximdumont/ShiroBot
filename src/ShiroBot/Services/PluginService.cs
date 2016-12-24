using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Discord.WebSocket;
using NLog;
using ShiroBot.Plugins;

namespace ShiroBot
{
    public class PluginService
    {
        // Private variable for logging in this class
        private static Logger s_log;

        // Store loaded plugins into a dictioanry 
        private Dictionary<String, IPlugin> _pluginRegistory;

        // General ShiroBot configuration
        public static IConfigurationRoot Configuration;

        // For discord client
        private readonly DiscordSocketClient _discordClient;

        public PluginService(IConfigurationRoot configuration, DiscordSocketClient discordClient)
        {
            // Copy configuration to class static variable and setup logging
            Configuration = configuration;
            _discordClient = discordClient;
            s_log = LogManager.GetCurrentClassLogger();
        }

        // needs a function to constantly watch plugins 

        // Load an individual plugin via its name 
        public void LoadPlugin(string pluginName)
        {
            // some code will need to plugin to events from here.

            pluginName = pluginName.UppercaseFirst();
            s_log.Info("Attempting to load plugin: " + pluginName);

            var pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", pluginName + ".dll");
            if (File.Exists(pluginPath))
            {
                try
                {
                    var myPlugin = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);

                    var myClass = myPlugin.GetType("ShiroBot.Plugins." + pluginName);
                    var myClassInstance = Activator.CreateInstance(myClass);

                    var myMethod = myClass.GetMethod("Init");
                    object[] arr = new object[] { new object[] { _discordClient } };
                    myMethod.Invoke(myClassInstance, arr);
                }
                catch (System.Exception ex)
                {
                    s_log.Error("There was an issue trying to load plugin: " + pluginName + ". Exception thrown was: " + ex.ToString());
                    return;
                }

                // need to bring the class into scope.......
                //_pluginRegistory.Add(pluginName, myClassInstance)
            }
            else
            {
                s_log.Error("Trying to load plugin: " + pluginName + " failed.");
            }
        }

        public void LoadAvailablePlugins()
        {
            var pluginDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "/Plugins/");
            string[] dirs = Directory.GetFiles(pluginDirectoryPath, "*.dll");
        }
    }
}