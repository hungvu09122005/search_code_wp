using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatBox.Plugins;

namespace ChatBox
{
    public sealed partial class MainWindow : Window
    {
        private const string NgrokUrl = "https://nonintermittent-therese-unregally.ngrok-free.dev"; 
        private readonly ObservableCollection<string> _messages = new();

        public MainWindow()
        {
            InitializeComponent();
            ChatListView.ItemsSource = _messages;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            // Hiển thị tin nhắn người dùng
            _messages.Add($"You: {userMessage}");
            InputTextBox.Text = string.Empty;
            Logger.LogFile($"You: {userMessage}");

            // Gửi tin nhắn tới Ollama
            string botResponse = await SendMessageToOllama(userMessage);
            _messages.Add($"Bot: {botResponse}");
            Logger.LogFile($"Bot: {botResponse}");
        }

        private async Task<string> SendMessageToOllama(string message)
        {
            try
            {
                using HttpClient client = new();

                var request = new
                {
                    model = "gpt-oss:20b",
                    prompt = message,
                    stream = false
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync($"{NgrokUrl}/api/generate", content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                string? lastLine = responseBody
                    .Trim()
                    .Split('\n')
                    .LastOrDefault(l => l.Contains("\"response\""));

                if (string.IsNullOrEmpty(lastLine))
                    return "No response from model.";

                using var doc = JsonDocument.Parse(lastLine);
                if (doc.RootElement.TryGetProperty("response", out var resp))
                    return resp.GetString() ?? "Empty response";

                return "No response field found.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
