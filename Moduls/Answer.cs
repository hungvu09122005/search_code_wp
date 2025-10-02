using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace App1.Moduls
{
    public class Answer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        public Answer(string text, bool isCorrect)
        {
            Text = text;
            IsCorrect = isCorrect;
        }
    }

    /// <summary>
    /// Xử lý logic khi người dùng chọn một đáp án.
    /// </summary>
    /// <param name="btn">
    /// Nút Button được nhấn (chứa Tag là bool để xác định đúng/sai).
    /// </param>
    /// <param name="root">
    /// XamlRoot hiện tại, cần để hiển thị ContentDialog.
    /// </param>
    /// <param name="frame">
    /// Frame dùng để điều hướng giữa các trang.
    /// </param>
    /// <param name="nextPage">
    /// Trang tiếp theo cần điều hướng đến nếu trả lời đúng.
    /// </param>
    /// <param name="homeParam">
    /// Tham số truyền cho HomePage nếu người dùng chọn quay lại (có thể null).
    /// </param>
    /// <returns>
    /// Task bất đồng bộ, đảm bảo xử lý dialog và điều hướng không chặn UI thread.
    /// </returns>
    public static class AnswerModule
    {
        public static async Task HandleAnswerAsync(Button? btn, XamlRoot root, Frame frame, Type nextPage, object? homeParam = null)
        {
            if (btn?.Tag is bool isCorrect)
            {
                if (isCorrect)
                {
                    var dialog = Dialogs.CreateCorrectDialog(root);
                    await dialog.ShowAsync();
                    frame.Navigate(nextPage);
                }
                else
                {
                    var dialog = Dialogs.CreateWrongDialog(root);
                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Secondary)
                    {
                        frame.Navigate(typeof(HomePage), homeParam);
                    }
                }
            }
        }
    }
}
