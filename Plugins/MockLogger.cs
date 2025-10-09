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
        public MockLogger()
        {
        }

        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Log(string message)
        {
            var now = DateTime.Now;
            string filename = $"log{now.Year}{now.Month}{now.Day}.txt";
            using (var writer = File.AppendText(filename))
            {
                writer.WriteLine($"{now.Hour}:{now.Minute}:{now.Second} {message}");
            }
        }
    }
}
