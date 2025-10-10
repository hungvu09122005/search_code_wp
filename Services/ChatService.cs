using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBox.Services
{
    /// <summary>
    /// Dịch vụ trò chuyện với AI server thông qua HTTP.
    /// </summary>
    public class ChatService
    {
        /// <summary>
        /// Đối tượng HttpClient dùng để gửi yêu cầu HTTP.
        /// </summary>
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Token hủy bỏ cho các yêu cầu bất đồng bộ.
        /// </summary>
        private readonly CancellationTokenSource _cts = new();

        /// <summary>
        /// URL của server AI (ngrok).
        /// </summary>
        private string _ngrokUrl = string.Empty;

        /// <summary>
        /// Trạng thái kết nối đến server AI.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Kết nối đến server AI với URL được cung cấp.
        /// </summary>
        /// <param name="url">URL của server AI.</param>
        /// <returns>Thông báo kết quả kết nối.</returns>
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

        /// <summary>
        /// Gửi tin nhắn đến server AI và nhận phản hồi.
        /// </summary>
        /// <param name="message">Nội dung tin nhắn gửi đi.</param>
        /// <returns>Phản hồi từ server AI hoặc thông báo lỗi.</returns>
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
