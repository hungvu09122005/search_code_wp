using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

namespace App1
{
    /// <summary>
    /// Một cửa sổ trống có thể được sử dụng độc lập hoặc điều hướng trong một Frame.
    /// </summary>
    public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
    {
        /// <summary>
        /// Khởi tạo một thể hiện mới của lớp <see cref="MainWindow"/>.
        /// Thiết lập kích thước cửa sổ và điều hướng đến trang đăng nhập.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            var currentWindow = this;

            // Lấy handle của cửa sổ hiện tại
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            int width = 1980;
            int height = 1024;

            // Đặt lại kích thước cửa sổ
            appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
            // Điều hướng đến trang đăng nhập
            RootFrame.Navigate(typeof(LoginPage));
        }
    }
}
