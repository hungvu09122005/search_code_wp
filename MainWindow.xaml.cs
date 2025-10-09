using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ChatBox.Models;
using ChatBox.Services;
using ChatBox.Plugins;
using ChatbotContractPlugin; // ⚡ Interface từ plugin

namespace ChatBox
{
    public sealed partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> _messages = new();
        private readonly OllamaService _ollama;

        public MainWindow()
        {
            this.InitializeComponent();
            ChatListView.ItemsSource = _messages;

            // 🔌 Khởi tạo logger plugin (tự động)
            Logger.LogFile("💡 ChatBox started.");
            _ollama = new OllamaService();

            Logger.LogFile("✅ OllamaService initialized.");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessMessageAsync();
        }

        private async void InputTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true; // Không xuống dòng
                await ProcessMessageAsync();
            }
        }

        private async Task ProcessMessageAsync()
        {
            string text = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            // Nếu nhập link ngrok, kết nối
            if (text.StartsWith("http"))
            {
                bool ok = await _ollama.ConnectAsync(text);
                _messages.Add(ok ? $"🌐 Connected to: {text}" : $"❌ Failed to connect: {text}");
                InputTextBox.Text = "";
                return;
            }

            if (!_ollama.IsConnected)
            {
                _messages.Add("⚠️ Please enter your Ngrok URL first!");
                return;
            }

            _messages.Add($"🧑 You: {text}");
            InputTextBox.Text = "";

            string reply = await _ollama.SendMessageAsync(text);
            _messages.Add($"🤖 Bot: {reply}");
        }
    }
}
