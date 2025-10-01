using App1.Models;
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
    public sealed partial class SecondQuestionPage : Page
    {
        private QuestionData question;

        public SecondQuestionPage()
        {
            this.InitializeComponent();

            // Tạo câu hỏi
            question = new QuestionData("What is the largest planet in our Solar System?", "Chọn đáp án đúng:");

            question.AddAnswer("Earth", false);
            question.AddAnswer("Jupiter", true);
            question.AddAnswer("Mars", false);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
                btn.Click += AnswerButton_Click;
                AnswerList.Children.Add(btn);
            }
        }

        private async void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn?.Tag is bool isCorrect)
            {
                var dialog = new ContentDialog
                {
                    Title = isCorrect ? "Correct!" : "Wrong!",
                    Content = isCorrect ? "Bạn đã chọn đáp án đúng!" : "Đáp án sai, bạn sẽ quay về trang HomePage.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();

                if (isCorrect)
                {
                    // Nếu đúng → chuyển sang FinalQuestionPage
                    Frame.Navigate(typeof(FinalQuestionPage));
                }
                else
                {
                    // Nếu sai → quay về HomePage
                    Frame.Navigate(typeof(HomePage));
                }
            }
        }
    }
}