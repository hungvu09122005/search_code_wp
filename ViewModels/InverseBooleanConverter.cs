    using Microsoft.UI.Xaml.Data;
    using System;

    namespace ChatBox
    {
        /// <summary>
        /// Bộ chuyển đổi giá trị kiểu bool sang giá trị ngược lại.
        /// </summary>
        public class InverseBooleanConverter : IValueConverter
        {
            /// <summary>
            /// Chuyển đổi giá trị bool sang giá trị ngược lại.
            /// </summary>
            /// <param name="value">Giá trị đầu vào kiểu bool.</param>
            /// <param name="targetType">Kiểu dữ liệu mục tiêu.</param>
            /// <param name="parameter">Tham số bổ sung.</param>
            /// <param name="language">Ngôn ngữ chuyển đổi.</param>
            /// <returns>Giá trị bool ngược lại hoặc true nếu không phải kiểu bool.</returns>
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                if (value is bool b)
                    return !b;
                return true;
            }

            /// <summary>
            /// Chuyển đổi ngược giá trị bool sang giá trị ngược lại.
            /// </summary>
            /// <param name="value">Giá trị đầu vào kiểu bool.</param>
            /// <param name="targetType">Kiểu dữ liệu mục tiêu.</param>
            /// <param name="parameter">Tham số bổ sung.</param>
            /// <param name="language">Ngôn ngữ chuyển đổi.</param>
            /// <returns>Giá trị bool ngược lại hoặc false nếu không phải kiểu bool.</returns>
            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                if (value is bool b)
                    return !b;
                return false;
            }
        }
    }
