using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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
        private string _ngrokUrl = string.Empty;
        private readonly ObservableCollection<string> _messages = new();
        private bool _isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            ChatListView.ItemsSource = _messages;
            AddBotMessage("Please enter a link to connect to AI.");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            AddUserMessage(userMessage);
            InputTextBox.Text = string.Empty;

            if (!_isConnected)
            {
                if (userMessage.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    await ConnectToServerAsync(userMessage);
                }
                else
                {
                    AddBotMessage("Please enter a valid Ngrok link (must start with http). Enter again!");
                }
                return;
            }

            string botResponse = await SendMessageToOllama(userMessage);
            AddBotMessage(botResponse);
        }

        private async Task ConnectToServerAsync(string url)
        {
            _ngrokUrl = url;
            AddBotMessage("I have received your link.");
            AddBotMessage("Please wait while connecting to AI...");

            string response = await SendMessageToOllama("Hello");

            if (response.StartsWith("Error"))
            {
                AddBotMessage("Failed to connect to AI server.");
                _isConnected = false;
            }
            else
            {
                AddBotMessage("How can I assist you today?");
                _isConnected = true;
            }
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

                var response = await client.PostAsync($"{_ngrokUrl}/api/generate", content);
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
                    return resp.GetString() ?? "Empty response.";

                return "No response field found.";
            }
            catch (Exception ex)
            {
                await ShowErrorAndExitAsync(ex.Message);
                return $"Error: {ex.Message}";
            }
        }

        private async Task ShowErrorAndExitAsync(string message)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = $"An error occurred: {message}",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await errorDialog.ShowAsync();

            var exitDialog = new ContentDialog
            {
                Title = "Exit",
                Content = "Exiting application...",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await exitDialog.ShowAsync();

            Environment.Exit(0);
        }

        private void AddUserMessage(string message)
        {
            // ✅ Cập nhật UI thread an toàn
            DispatcherQueue.TryEnqueue(() =>
            {
                _messages.Add($"You: {message}");
                if (_isConnected) Logger.LogFile($"You: {message}");
            });
        }

        private void AddBotMessage(string message)
        {
            // ✅ Cập nhật UI thread an toàn
            DispatcherQueue.TryEnqueue(() =>
            {
                _messages.Add($"Bot: {message}");
                if (_isConnected) Logger.LogFile($"Bot: {message}");
            });
        }
    }
}
