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
    internal class Logger
    {
        public static void LogFile (string message)
        {
            List<(string, ILogger)> loggers = new List<(string, ILogger)>();

            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = (new DirectoryInfo(folder)).GetFiles("*Plugin*.dll");

            foreach (var fi in fis)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(fi.FullName);
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (type.IsClass && (type.Name != typeof(MockLogger).Name)
                            && typeof(ChatbotContractPlugin.ILogger).IsAssignableFrom(type))
                        {
                            var item = (ILogger)Activator.CreateInstance(type)!;
                            loggers.Add((fi.Name, item));
                        }
                    }
                }
                catch (BadImageFormatException)
                {
                    // Log or skip invalid DLLs
                    continue;
                }
            }
            // Nếu có plugin thật, dùng nó
            ILogger? logger = loggers.FirstOrDefault().Item2;

            // Nếu không có plugin nào, fallback sang MockLogger
            logger ??= new MockLogger();

            logger.Log(message);
        }
    }
}
