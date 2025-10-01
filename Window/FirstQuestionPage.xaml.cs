using App1.Models;
using App1.Window;
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
using static App1.Data.QuestionRepository;

namespace App1
{
    public sealed partial class FirstQuestionPage : Page
    {
        private QuestionData? question;

        public FirstQuestionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            question = App1.Data.QuestionRepository.Questions[0];

            TitleTextBlock.Text = question.Title;
            BodyTextBlock.Text = question.Body;

            foreach (var ans in question.Answers)
            {
                var btn = new Button
                {
                    Content = ans.Text,
                    Tag = ans.IsCorrect,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                btn.Click += AnswerButtonClick;
                AnswerList.ItemsSource = question.Answers;
            }
        }
        private async void AnswerButtonClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is bool isCorrect)
            {
                if (isCorrect)
                {
                    var correctDialog = new ContentDialog
                    {
                        Title = "Correct!",
                        Content = "You answered correctly!",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };


                    await correctDialog.ShowAsync();
                    Frame.Navigate(typeof(SecondQuestionPage));
                }
                else
                {
                    var wrongDialog = new ContentDialog
                    {
                        Title = "Dead End!",
                        Content = "You've reached a dead end.\nYou can choose to go back to the previous door or start over from the beginning.",
                        PrimaryButtonText = "Go Back",
                        SecondaryButtonText = "Home",
                        DefaultButton = ContentDialogButton.Primary,
                        XamlRoot = this.XamlRoot
                    };


                    var result = await wrongDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        // Quay lại trang hiện tại
                        Frame.GoBack(); 
                    }
                    else if (result == ContentDialogResult.Secondary)
                    { 
                        Frame.Navigate(typeof(HomePage), App1.Data.UserState.Name);
                    }
                }
            }
        }
    }
}
