using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ChatBox.ViewModels;

namespace ChatBox
{
    /// <summary>
    /// Lớp <c>MainWindow</c> đại diện cửa sổ chính của ứng dụng ChatBox.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// ViewModel quản lý luồng chat giữa người dùng và AI.
        /// </summary>
        public ChatViewModel ViewModel { get; } = new();

        /// <summary>
        /// Khởi tạo một instance mới của <see cref="MainWindow"/>.
        /// Thiết lập DataContext cho <c>RootGrid</c> để liên kết dữ liệu với ViewModel.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Gán DataContext cho RootGrid thay vì Window (vì Window không có DataContext)
            RootGrid.DataContext = ViewModel;
        }

        /// <summary>
        /// Xử lý sự kiện nhấn phím trong <c>InputTextBox</c>.
        /// Nếu phím Enter được nhấn, thực thi lệnh gửi tin nhắn từ ViewModel.
        /// </summary>
        /// <param name="sender">Đối tượng phát sinh sự kiện.</param>
        /// <param name="e">Thông tin về sự kiện phím được nhấn.</param>
        private void InputTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                if (ViewModel.SendCommand.CanExecute(null))
                    ViewModel.SendCommand.Execute(null);
            }
        }
    }
}
