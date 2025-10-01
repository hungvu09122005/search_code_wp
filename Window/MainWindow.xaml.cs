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
    /// M?t c?a s? tr?ng có th? ???c s? d?ng ??c l?p ho?c ?i?u h??ng trong m?t Frame.
    /// </summary>
    public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
    {
        /// <summary>
        /// Kh?i t?o m?t th? hi?n m?i c?a l?p <see cref="MainWindow"/>.
        /// Thi?t l?p kích th??c c?a s? và ?i?u h??ng ??n trang ??ng nh?p.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            var currentWindow = this;

            // L?y handle c?a s? hi?n t?i
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            int width = 1980;
            int height = 1024;

            // ??t l?i kích th??c c?a s?
            appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
            // ?i?u h??ng ??n trang ??ng nh?p
            RootFrame.Navigate(typeof(LoginPage));
        }
    }
}
