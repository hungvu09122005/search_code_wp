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

namespace App1.Window
{
    /// <summary>
    /// Trang hiển thị câu hỏi cuối cùng trong ứng dụng.
    /// </summary>
    public sealed partial class FinalQuestionPage : Page
    {
        /// <summary>
        /// Dữ liệu câu hỏi hiện tại.
        /// </summary>
        private QuestionData? question;

        /// <summary>
        /// Khởi tạo một instance mới của <see cref="FinalQuestionPage"/>
        /// </summary>
        public FinalQuestionPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Được gọi khi điều hướng đến trang này.
        /// Thiết lập nội dung câu hỏi và các đáp án.
        /// </summary>
        /// <param name="e">Tham số sự kiện điều hướng.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App1.Models.QuestionLoader.LoadQuestion(
                2,                     // index câu hỏi
                TitleTextBlock,        // nơi hiển thị title
                BodyTextBlock,         // nơi hiển thị body
                AnswerList,            // nơi chứa các button
                AnswerButtonClick      // event handler khi bấm đáp án
            );
        }

        /// <summary>
        /// Xử lý sự kiện khi người dùng chọn một đáp án.
        /// Hiển thị thông báo đúng/sai và điều hướng trang phù hợp.
        /// </summary>
        /// <param name="sender">Nút được nhấn.</param>
        /// <param name="e">Tham số sự kiện.</param>
        private async void AnswerButtonClick(object sender, RoutedEventArgs e)
        {
            await App1.Models.AnswerModule.HandleAnswerAsync(
                sender as Button,
                this.XamlRoot,
                Frame,
                typeof(ResultPage),   // Page tiếp theo (truyền động)
                App1.Data.UserState.Name      // tham số cho HomePage
            );
        }
    }
}
