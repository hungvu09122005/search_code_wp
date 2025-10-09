using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBox.Services
{
    public class ChatService
    {
        private static readonly HttpClient _httpClient = new();
        private readonly CancellationTokenSource _cts = new();

        private string _ngrokUrl = string.Empty;
        public bool IsConnected { get; private set; }

        public async Task<string> ConnectAsync(string url)
        {
            _ngrokUrl = url;

            string response = await SendMessageAsync("Hello");

            if (response.StartsWith("Error"))
            {
                IsConnected = false;
                return "Failed to connect to AI server. Please check the URL and try again.";
            }

            IsConnected = true;
            return "Connection established! How can I assist you today?";
        }

        public async Task<string> SendMessageAsync(string message)
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

                // 🧠 Parsing JSON đúng chuẩn, không Split()
                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("response", out var resp))
                        return resp.GetString() ?? "Empty response.";
                }
                catch (JsonException)
                {
                    return "No valid JSON response from model.";
                }

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
                return $"Error: {ex.Message}";
            }
        }
    }
}
