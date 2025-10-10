using ChatbotContractPlugin;
using System;
using System.IO;

namespace ChatBox.Plugins
{
    /// <summary>
    /// Triển khai giao diện <see cref="ILogger"/> để ghi log vào file văn bản.
    /// </summary>
    internal class MockLogger : ILogger
    {
        /// <summary>
        /// Ghi một thông điệp log vào file theo ngày hiện tại.
        /// </summary>
        /// <param name="message">Thông điệp cần ghi log.</param>
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
