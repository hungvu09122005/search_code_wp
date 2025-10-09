using ChatbotContractPlugin;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBox.Plugins
{
    internal class MockLogger : ILogger
    {
        private static readonly object _lock = new();

        public void Log(string message)
        {
            try
            {
                lock (_lock)
                {
                    var now = DateTime.Now;
                    string folder = Path.Combine(AppContext.BaseDirectory, "logs");
                    Directory.CreateDirectory(folder);
                    string file = Path.Combine(folder, $"log_{now:yyyyMMdd}.txt");

                    File.AppendAllText(file, $"[{now:HH:mm:ss}] {message}\n");
                }
            }
            catch
            {
                // Bỏ qua lỗi ghi file
            }
        }
    }
}
