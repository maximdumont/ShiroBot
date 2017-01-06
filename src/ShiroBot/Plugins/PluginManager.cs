using System.Collections.Generic;
using NLog;

namespace ShiroBot.Plugins
{
    public delegate void PluginEvent(IPlugin plugin); // this will handle all the events, needs to be implement

    // Manages a dictionary of plugins for ShiroBot
    public sealed class PluginManager
    {
        // Private variable for logging in this class
        private static Logger s_log;

        // Called when a plugin has been added to the Plugin Manager
        public event PluginEvent OnPluginAdded;

        // Called when a plugin has been removed from the Plugin Manager
        public event PluginEvent OnPluginRemoved;

        // All loaded plugins
        private readonly IDictionary<string, IPlugin> loadedPlugins;

        // All hook subscriptions
        private readonly IDictionary<string, IList<IPlugin>> hookSubscriptions;

        // Initializes a new instance of the PluginManager class
        public PluginManager(Logger log)
        {
            // Initialize
            loadedPlugins = new Dictionary<string, IPlugin>();
            hookSubscriptions = new Dictionary<string, IList<IPlugin>>();
            s_log = log; // copy logger to local static instance
        }

        // Adds a plugin to this manager
        public bool AddPlugin(IPlugin plugin)
        {
            if (loadedPlugins.ContainsKey(plugin.Name)) return false;
            loadedPlugins.Add(plugin.Name, plugin);
            plugin.HandleAddedToManager(this);
            OnPluginAdded?.Invoke(plugin);
            return true;
        }

        // Removes a plugin from this manager
        public bool RemovePlugin(IPlugin plugin)
        {
            if (!loadedPlugins.ContainsKey(plugin.Name)) return false;
            loadedPlugins.Remove(plugin.Name);
            foreach (var list in hookSubscriptions.Values)
                if (list.Contains(plugin)) list.Remove(plugin);
            plugin.HandleRemovedFromManager(this);
            OnPluginRemoved?.Invoke(plugin);
            return true;
        }

        // Gets and returns a plugin by string name
        public IPlugin GetPlugin(string name)
        {
            IPlugin plugin;
            return loadedPlugins.TryGetValue(name, out plugin) ? plugin : null;
        }

        // Gets all plugins managed by this manager
        public IEnumerable<IPlugin> GetPlugins() => loadedPlugins.Values;

        // Subscribes the specified plugin to the specified hook
        internal void SubscribeToHook(string hook, IPlugin plugin)
        {
            if (!loadedPlugins.ContainsKey(plugin.Name) || !plugin.IsCorePlugin && hook.StartsWith("I")) return;
            IList<IPlugin> sublist;
            if (!hookSubscriptions.TryGetValue(hook, out sublist))
            {
                sublist = new List<IPlugin>();
                hookSubscriptions.Add(hook, sublist);
            }
            if (!sublist.Contains(plugin)) sublist.Add(plugin);
            s_log.Debug("Plugin {plugin.Name} is subscribing to hook '{hook}'!");
        }

        // Unsubscribes the specified plugin to the specified hook`
        internal void UnsubscribeToHook(string hook, IPlugin plugin)
        {
            if (!loadedPlugins.ContainsKey(plugin.Name) || !plugin.IsCorePlugin && hook.StartsWith("I")) return;
            IList<IPlugin> sublist;
            if (hookSubscriptions.TryGetValue(hook, out sublist) && sublist.Contains(plugin))
                sublist.Remove(plugin);
            s_log.Debug($"Plugin {plugin.Name} is unsubscribing to hook '{hook}'!");
        }

        // Calls a hook on all plugins of this manager
        public object CallHook(string hook, params object[] args)
        {
            // Locate the sublist
            IList<IPlugin> plugins;
            if (!hookSubscriptions.TryGetValue(hook, out plugins)) return null;
            if (plugins.Count == 0) return null;

            // Loop each item
            var values = new object[plugins.Count];
            var returnCount = 0;
            object finalValue = null;
            IPlugin finalPlugin = null;
            for (var i = 0; i < plugins.Count; i++)
            {
                // Call the hook
                var value = plugins[i].CallHook(hook, args);
                if (value != null)
                {
                    values[i] = value;
                    finalValue = value;
                    finalPlugin = plugins[i];
                    returnCount++;
                }
            }

            // Is there a return value?
            if (returnCount == 0) return null;

            // This logic needs to be broken down a bit more.
            if (returnCount > 1 && finalValue != null)
            {
                // Placeholder for hook conflicts 
                // With the current implementation of the bot, plugins having conflicts is normal for now....
            }

            return finalValue;
        }
    }
}