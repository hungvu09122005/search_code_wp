using ChatbotContractPlugin;
using System;
using System.IO;

namespace ChatBox.Plugins
{
    internal class MockLogger : ILogger
    {
        public void Log(string message)
        {
            try
            {
                var now = DateTime.Now;
                string filename = $"log{now:yyyyMMdd}.txt";
                string line = $"{now:HH:mm:ss} {message}";
                lock (typeof(MockLogger))
                {
                    File.AppendAllText(filename, line + Environment.NewLine);
                }
            }
            catch { /* tránh crash */ }
        }
    }
}
