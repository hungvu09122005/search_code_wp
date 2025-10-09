using ChatBox.Plugins;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBox
{
    public sealed partial class MainWindow : Window
    {
        private static readonly HttpClient _httpClient = new(); // dùng lại toàn cục
        private readonly ObservableCollection<string> _messages = new();
        private string _ngrokUrl = string.Empty;
        private bool _isConnected = false;
        private readonly CancellationTokenSource _cts = new();

        public MainWindow()
        {
            InitializeComponent();
            ChatListView.ItemsSource = _messages;
            AddBotMessage("Please enter a link to connect to AI.");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInputAsync();
        }

        private async void InputTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                await ProcessUserInputAsync();
            }
        }

        private async Task ProcessUserInputAsync()
        {
            string userMessage = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            AddUserMessage(userMessage);
            InputTextBox.Text = string.Empty;

            if (!_isConnected)
            {
                if (Uri.TryCreate(userMessage, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    await ConnectToServerAsync(userMessage);
                }
                else
                {
                    AddBotMessage("Please enter a valid Ngrok link (must start with http). Enter again!");
                }
                return;
            }

            string botResponse = await SendMessageToOllamaAsync(userMessage);
            AddBotMessage(botResponse);
        }

        private async Task ConnectToServerAsync(string url)
        {
            _ngrokUrl = url;
            AddBotMessage("I have received your link.");
            AddBotMessage("Please wait while connecting to AI...");

            string response = await SendMessageToOllamaAsync("Hello");

            if (response.StartsWith("Error"))
            {
                AddBotMessage("Failed to connect to AI server. Please check the URL and try again.");
                _isConnected = false;
            }
            else
            {
                AddBotMessage("Connection established! How can I assist you today?");
                _isConnected = true;
            }
        }

        private async Task<string> SendMessageToOllamaAsync(string message)
        {
            if (string.IsNullOrEmpty(_ngrokUrl))
                return "Error: No server URL set.";

            try
            {
                var request = new
                {
                    model = "gpt-oss:20b",
                    prompt = message,
                    stream = false
                };

                using var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                using var response = await _httpClient.PostAsync($"{_ngrokUrl}/api/generate", content, _cts.Token);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                string? lastLine = null;

                try
                {
                    lastLine = responseBody
                        .Trim()
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .LastOrDefault(l => l.Contains("\"response\""));
                }
                catch
                {
                    return "No valid response from model.";
                }

                if (string.IsNullOrEmpty(lastLine))
                    return "No response from model.";

                using var doc = JsonDocument.Parse(lastLine);
                if (doc.RootElement.TryGetProperty("response", out var resp))
                    return resp.GetString() ?? "Empty response.";

                return "No response field found.";
            }
            catch (TaskCanceledException)
            {
                return "Request canceled.";
            }
            catch (HttpRequestException ex)
            {
                return $"Error: Cannot reach server ({ex.Message})";
            }
            catch (Exception ex)
            {
                AddBotMessage($"Unexpected error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        private void AddUserMessage(string message)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _messages.Add($"You: {message}");
                if (_isConnected) Logger.LogFile($"You: {message}");
                ChatListView.ScrollIntoView(_messages[^1]);
            });
        }

        private void AddBotMessage(string message)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _messages.Add($"Bot: {message}");
                if (_isConnected) Logger.LogFile($"Bot: {message}");
                ChatListView.ScrollIntoView(_messages[^1]);
            });
        }
    }
}
