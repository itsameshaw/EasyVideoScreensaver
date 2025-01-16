using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace EasyVideoScreensaver
{
    internal class PluginManager
    {
        struct Plugin
        {
            public ISaverPlugin plugin;
            public int priority;
        }

        private List<Plugin> plugins = new List<Plugin>();

        public PluginManager(string[] paths) {
            foreach (string path in paths)
            {
                var directory = new DirectoryInfo(path);
                var files = directory.GetFiles();
                foreach (var file in files)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(path);
                        if (assembly != null)
                        {
                            var pluginTypes = assembly.GetTypes().Where(type => typeof(ISaverPlugin).IsAssignableFrom(type) && !type.IsAbstract);
                            foreach (var pluginType in pluginTypes)
                            {
                                try
                                {
                                    var plugin = (ISaverPlugin)Activator.CreateInstance(pluginType);
                                    var priorityProperty = pluginType.GetProperty("Priority", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                                    var priority = (int)(priorityProperty?.GetValue(plugin) ?? 0);
                                    plugins.Add(new Plugin() { plugin = plugin, priority = priority });
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }

            plugins.Sort((a, b) => b.priority - a.priority);
        }

        public Func<string, Task<Uri>>[] UrlHandlers(string url)
        {
            List<Plugin> handlers = new List<Plugin>();
            foreach (var plugin in plugins)
            {
                if (plugin.plugin.CanHandleURL(url))
                {
                    handlers.Add(plugin);
                }
            }

            return handlers.SelectMany(h => { return new Func<string, Task<Uri>>[] { (inputUrl) => h.plugin.HandleURL(inputUrl) }; }).ToArray();
        }
    }
}
