using ChatBox.Plugins;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatBox.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private static readonly HttpClient _httpClient = new();
        private readonly DispatcherQueue _dispatcher = DispatcherQueue.GetForCurrentThread();
        private readonly CancellationTokenSource _cts = new();

        private string _inputText = string.Empty;
        private string _ngrokUrl = string.Empty;
        private bool _isConnected = false;

        public ObservableCollection<string> Messages { get; } = new();

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public ICommand SendCommand { get; }

        public ChatViewModel()
        {
            SendCommand = new RelayCommand(async _ => await ProcessInputAsync());
            AddBotMessage("Please enter a link to connect to AI.");
        }

        private async Task ProcessInputAsync()
        {
            string userMessage = InputText.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            AddUserMessage(userMessage);
            InputText = string.Empty;

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

            string response = await SendMessageToOllamaAsync(userMessage);
            AddBotMessage(response);
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
                var req = new
                {
                    model = "gpt-oss:20b",
                    prompt = message,
                    stream = false
                };

                using var content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");
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
            _dispatcher.TryEnqueue(() =>
            {
                Messages.Add($"You: {message}");
                if (_isConnected) Logger.LogFile($"You: {message}");
            });
        }

        private void AddBotMessage(string message)
        {
            _dispatcher.TryEnqueue(() =>
            {
                Messages.Add($"Bot: {message}");
                if (_isConnected) Logger.LogFile($"Bot: {message}");
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public async void Execute(object? parameter) => await _execute(parameter);

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
