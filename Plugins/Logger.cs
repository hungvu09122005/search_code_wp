using ChatbotContractPlugin;
using ChatBox.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ChatBox.Plugins
{
    public static class Logger
    {
        private static ILogger? _logger;

        static Logger()
        {
            _logger = LoadLoggerFromPlugins() ?? new MockLogger();
            _logger.Log("✅ Logger system initialized.");
        }

        public static void LogFile(string message)
        {
            try
            {
                _logger?.Log(message);
            }
            catch
            {
                // Không để log lỗi gây crash app
            }
        }

        private static ILogger? LoadLoggerFromPlugins()
        {
            try
            {
                string folder = AppContext.BaseDirectory;
                var pluginFiles = new DirectoryInfo(folder).GetFiles("*Plugin*.dll");

                foreach (var file in pluginFiles)
                {
                    try
                    {
                        var asm = Assembly.LoadFrom(file.FullName);
                        var loggerType = asm.GetTypes()
                            .FirstOrDefault(t =>
                                t.IsClass &&
                                typeof(ILogger).IsAssignableFrom(t) &&
                                !t.IsAbstract);

                        if (loggerType != null)
                        {
                            var instance = (ILogger)Activator.CreateInstance(loggerType)!;
                            instance.Log($"🧩 Loaded plugin: {file.Name}");
                            return instance;
                        }
                    }
                    catch (BadImageFormatException) { continue; }
                    catch { continue; }
                }
            }
            catch { }

            return null;
        }
    }
}

