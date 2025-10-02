using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace App1.Modulo
{
    public static class Dialogs
    {
        public static ContentDialog CreateCorrectDialog(XamlRoot root)
        {
            return new ContentDialog
            {
                Title = "Correct!",
                Content = "You answered correctly!",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = root
            };
        }

        public static ContentDialog CreateWrongDialog(XamlRoot root)
        {
            return new ContentDialog
            {
                Title = "Dead End!",
                Content = "You've reached a dead end.\nYou can choose to go back to the previous door or start over from the beginning.",
                PrimaryButtonText = "Go Back",
                SecondaryButtonText = "Home",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = root
            };
        }
    }
}

