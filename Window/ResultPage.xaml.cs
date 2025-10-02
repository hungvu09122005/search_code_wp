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

namespace App1.Window
{
    public sealed partial class ResultPage : Page
    {
        public ResultPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ResultTextBlock.Text = "You have escaped the Escape Room!\nFeel free to send us an invitation via email:\n23120268@student.hcmus.edu.vn";
        }

        private void BackToHomeClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HomePage), App1.Data.UserState.Name);
        }

        private void ExitGameClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}