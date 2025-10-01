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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    public sealed partial class FirstQuestionPage : Page
    {
        private QuestionData question;

        public FirstQuestionPage()
        {
            this.InitializeComponent();

            // Tạo câu hỏi trực tiếp tại đây
            question = new QuestionData("What is the capital of France?", "Chọn đáp án đúng:");

            // Thêm đáp án (1 đáp án đúng duy nhất)
            question.AddAnswer("Paris", true);
            question.AddAnswer("London", false);
            question.AddAnswer("Berlin", false);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Hiển thị dữ liệu câu hỏi
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
                ContentDialog dialog = new ContentDialog
                {
                    Title = isCorrect ? "Correct!" : "Wrong!",
                    Content = isCorrect ? "Bạn đã chọn đáp án đúng!" : "Đáp án sai, bạn sẽ quay về trang HomePage.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();

                if (isCorrect)
                {
                    // Nếu đúng → chuyển sang SecondQuestionPage
                    Frame.Navigate(typeof(SecondQuestionPage));
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