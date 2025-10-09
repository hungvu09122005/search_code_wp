using ChatbotContractPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ChatBox.Plugins
{
    internal static class Logger
    {
        private static readonly List<ILogger> _cachedLoggers = new();
        private static bool _initialized = false;
        private static readonly object _lock = new();

        public static void LogFile(string message)
        {
            try
            {
                if (!_initialized)
                    LoadPlugins();

                var logger = _cachedLoggers.FirstOrDefault() ?? new MockLogger();
                logger.Log(message);
            }
            catch (Exception ex)
            {
                // Fallback an toàn
                new MockLogger().Log($"[LoggerError] {ex.Message}");
            }
        }

        private static void LoadPlugins()
        {
            lock (_lock)
            {
                if (_initialized) return;

                string folder = AppDomain.CurrentDomain.BaseDirectory;
                var dlls = new DirectoryInfo(folder).GetFiles("*Plugin*.dll");

                foreach (var fi in dlls)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(fi.FullName);
                        var types = assembly.GetTypes();

                        foreach (var type in types)
                        {
                            if (type.IsClass &&
                                typeof(ILogger).IsAssignableFrom(type) &&
                                type.Name != nameof(MockLogger))
                            {
                                if (Activator.CreateInstance(type) is ILogger instance)
                                    _cachedLoggers.Add(instance);
                            }
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        new MockLogger().Log($"[PluginLoadError] {ex.Message}");
                    }
                }

                _initialized = true;
            }
        }
    }
}
