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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public LoginPage()
    {
        this.InitializeComponent();
    }

    private async void SaluteClick(object sender, RoutedEventArgs e)
    {
        String nameUser = NameUser.Text;
        if (nameUser == String.Empty)
        {
            var dialog = new ContentDialog()
            {
                Title = "Error",
                Content = "Please enter your name!",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
            return;
        }

        this.Frame.Navigate(typeof(HomePage), nameUser);
    }
}

