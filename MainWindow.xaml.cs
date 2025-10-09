using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ChatBox.ViewModels;

namespace ChatBox
{
    public sealed partial class MainWindow : Window
    {
        public ChatViewModel ViewModel { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            // ✅ Gán DataContext cho RootGrid thay vì Window (vì Window không có DataContext)
            RootGrid.DataContext = ViewModel;
        }

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
