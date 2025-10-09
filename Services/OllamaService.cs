using ChatBox.Models;
using ChatBox.Plugins;
using ChatbotContractPlugin;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatBox.Services
{
    public class OllamaService : IDisposable
    {
        private readonly HttpClient _client = new();
        private string _baseUrl = string.Empty;

        public bool IsConnected { get; private set; } = false;

        public OllamaService()
        {
            Logger.LogFile("🧩 OllamaService created.");
        }

        public async Task<bool> ConnectAsync(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            Logger.LogFile($"Attempting to connect to {_baseUrl}");

            try
            {
                var test = new OllamaRequest { prompt = "ping" };
                var response = await _client.PostAsJsonAsync($"{_baseUrl}/api/generate", test);
                IsConnected = response.IsSuccessStatusCode;
                Logger.LogFile(IsConnected ? "Connected successfully." : "Connection failed.");
                return IsConnected;
            }
            catch (Exception ex)
            {
                Logger.LogFile($"❌ Connection error: {ex.Message}");
                IsConnected = false;
                return false;
            }
        }

        public async Task<string> SendMessageAsync(string message)
        {
            if (!IsConnected)
                return "⚠️ Not connected to server.";

            try
            {
                var req = new OllamaRequest { prompt = message };
                var response = await _client.PostAsJsonAsync($"{_baseUrl}/api/generate", req);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var parsed = JsonSerializer.Deserialize<OllamaResponse>(json);

                string text = parsed?.Response ?? parsed?.Message ?? "No response from AI.";
                Logger.LogFile($"🤖 Bot: {text}");
                return text;
            }
            catch (Exception ex)
            {
                Logger.LogFile($"❌ Error sending message: {ex.Message}");
                return $"❌ Error: {ex.Message}";
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
