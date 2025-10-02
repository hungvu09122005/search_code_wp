using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace App1.Moduls
{
    /// <summary>
    /// Load câu hỏi từ QuestionRepository theo chỉ số (index) và hiển thị ra UI.
    /// </summary>
    /// <param name="index">
    /// Vị trí câu hỏi trong danh sách QuestionRepository.
    /// </param>
    /// <param name="titleBlock">
    /// Button dùng để hiển thị tiêu đề câu hỏi (sử dụng Content để bind).
    /// </param>
    /// <param name="bodyBlock">
    /// TextBlock dùng để hiển thị nội dung chi tiết của câu hỏi.
    /// </param>
    /// <param name="answerList">
    /// ItemsControl nơi chứa danh sách các đáp án (Button).
    /// </param>
    /// <param name="answerClickHandler">
    /// Event handler được gắn cho mỗi đáp án khi người dùng click.
    /// </param>
    public static class QuestionLoader
    {
        public static void LoadQuestion(
            int index,
            Button titleBlock,
            TextBlock bodyBlock,
            ItemsControl answerList,
            RoutedEventHandler answerClickHandler)
        {
            // Lấy câu hỏi từ repository
            var question = Data.QuestionRepository.Questions[index];

            // Bind title và body
            titleBlock.Content = question.Title;
            bodyBlock.Text = question.Body;

            foreach (var ans in question.Answers)
            {
                var btn = new Button
                {
                    Content = ans.Text,
                    Tag = ans.IsCorrect,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                btn.Click += answerClickHandler;
                answerList.ItemsSource = question.Answers;
            }
        }
    }
}

