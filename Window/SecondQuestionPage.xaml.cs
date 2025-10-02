using App1.Modulo;
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
    /// Trang hiển thị câu hỏi thứ hai và xử lý lựa chọn đáp án của người dùng.
    /// </summary>
    public sealed partial class SecondQuestionPage : Page
    {
        /// <summary>
        /// Dữ liệu câu hỏi hiện tại.
        /// </summary>
        private QuestionData? question;

        /// <summary>
        /// Khởi tạo trang SecondQuestionPage.
        /// </summary>
        public SecondQuestionPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Được gọi khi điều hướng đến trang này. Thiết lập dữ liệu câu hỏi và hiển thị lên giao diện.
        /// </summary>
        /// <param name="e">Tham số sự kiện điều hướng.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App1.Modulo.QuestionLoader.LoadQuestion(
                1,                     // index câu hỏi
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
            await App1.Modulo.AnswerModule.HandleAnswerAsync(
                sender as Button,
                this.XamlRoot,
                Frame,
                typeof(FinalQuestionPage),   // Page tiếp theo (truyền động)
                App1.Data.UserState.Name      // tham số cho HomePage
            );
        }
    }
}