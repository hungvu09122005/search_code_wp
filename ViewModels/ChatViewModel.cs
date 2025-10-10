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
    /// <summary>
    /// ViewModel quản lý luồng chat giữa người dùng và AI.
    /// </summary>
    public class ChatViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// HttpClient dùng để gửi yêu cầu HTTP đến server AI.
        /// </summary>
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Hàng đợi Dispatcher để cập nhật UI từ luồng khác.
        /// </summary>
        private readonly DispatcherQueue _dispatcher = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Token hủy cho các tác vụ bất đồng bộ.
        /// </summary>
        private readonly CancellationTokenSource _cts = new();

        /// <summary>
        /// Văn bản nhập từ người dùng.
        /// </summary>
        private string _inputText = string.Empty;

        /// <summary>
        /// Đường dẫn Ngrok để kết nối đến server AI.
        /// </summary>
        private string _ngrokUrl = string.Empty;

        /// <summary>
        /// Cờ xác định đã kết nối đến server AI hay chưa.
        /// </summary>
        private bool _isConnected = false;

        /// <summary>
        /// Cờ xác định trạng thái bận khi đang gửi tin nhắn.
        /// </summary>
        private bool _isBusy = false; // ✅ Thêm cờ chặn nhập khi đang gửi

        /// <summary>
        /// Danh sách các tin nhắn trong cuộc trò chuyện.
        /// </summary>
        public ObservableCollection<string> Messages { get; } = new();

        /// <summary>
        /// Thuộc tính văn bản nhập từ người dùng.
        /// </summary>
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        /// <summary>
        /// Thuộc tính trạng thái bận của ViewModel.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                    (SendCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Lệnh gửi tin nhắn.
        /// </summary>
        public ICommand SendCommand { get; }

        /// <summary>
        /// Khởi tạo ViewModel và thiết lập lệnh gửi.
        /// </summary>
        public ChatViewModel()
        {
            SendCommand = new RelayCommand(async _ => await ProcessInputAsync(), _ => !IsBusy);
            AddBotMessage("Please enter a link to connect to AI.");
        }

        /// <summary>
        /// Xử lý nhập liệu từ người dùng và gửi đến AI.
        /// </summary>
        private async Task ProcessInputAsync()
        {
            if (IsBusy) return;
            IsBusy = true; // ✅ Chặn nhập mới

            string userMessage = InputText.Trim();
            if (string.IsNullOrEmpty(userMessage))
            {
                IsBusy = false;
                return;
            }

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

                IsBusy = false;
                return;
            }

            string response = await SendMessageToOllamaAsync(userMessage);
            AddBotMessage(response);

            IsBusy = false; // ✅ Cho phép nhập lại
        }

        /// <summary>
        /// Kết nối đến server AI qua đường dẫn Ngrok.
        /// </summary>
        /// <param name="url">Đường dẫn Ngrok.</param>
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

        /// <summary>
        /// Gửi tin nhắn đến server AI và nhận phản hồi.
        /// </summary>
        /// <param name="message">Nội dung tin nhắn.</param>
        /// <returns>Phản hồi từ AI.</returns>
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

        /// <summary>
        /// Thêm tin nhắn của người dùng vào danh sách và ghi log nếu đã kết nối.
        /// </summary>
        /// <param name="message">Nội dung tin nhắn.</param>
        private void AddUserMessage(string message)
        {
            _dispatcher.TryEnqueue(() =>
            {
                Messages.Add($"You: {message}");
                if (_isConnected) Logger.LogFile($"You: {message}");
            });
        }

        /// <summary>
        /// Thêm tin nhắn của bot vào danh sách và ghi log nếu đã kết nối.
        /// </summary>
        /// <param name="message">Nội dung tin nhắn.</param>
        private void AddBotMessage(string message)
        {
            _dispatcher.TryEnqueue(() =>
            {
                Messages.Add($"Bot: {message}");
                if (_isConnected) Logger.LogFile($"Bot: {message}");
            });
        }

        /// <summary>
        /// Sự kiện thông báo thay đổi thuộc tính.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gọi sự kiện PropertyChanged khi thuộc tính thay đổi.
        /// </summary>
        /// <param name="name">Tên thuộc tính thay đổi.</param>
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Lớp lệnh RelayCommand hỗ trợ thực thi bất đồng bộ cho ICommand.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Hàm thực thi lệnh.
        /// </summary>
        private readonly Func<object?, Task> _execute;

        /// <summary>
        /// Hàm kiểm tra có thể thực thi lệnh hay không.
        /// </summary>
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// Khởi tạo RelayCommand với hàm thực thi và kiểm tra.
        /// </summary>
        /// <param name="execute">Hàm thực thi.</param>
        /// <param name="canExecute">Hàm kiểm tra.</param>
        public RelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Kiểm tra có thể thực thi lệnh hay không.
        /// </summary>
        /// <param name="parameter">Tham số truyền vào.</param>
        /// <returns>True nếu có thể thực thi, ngược lại là false.</returns>
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        /// <summary>
        /// Thực thi lệnh bất đồng bộ.
        /// </summary>
        /// <param name="parameter">Tham số truyền vào.</param>
        public async void Execute(object? parameter) => await _execute(parameter);

        /// <summary>
        /// Sự kiện thông báo trạng thái thực thi lệnh thay đổi.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Gọi sự kiện CanExecuteChanged để cập nhật trạng thái lệnh.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
