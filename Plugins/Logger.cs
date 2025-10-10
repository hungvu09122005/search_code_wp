using ChatbotContractPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ChatBox.Plugins
{
    /// <summary>
    /// Lớp tĩnh dùng để ghi log thông qua các plugin logger.
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// Danh sách các logger đã được cache sau khi tải plugin.
        /// </summary>
        private static readonly List<ILogger> _cachedLoggers = new();

        /// <summary>
        /// Biến đánh dấu đã khởi tạo logger hay chưa.
        /// </summary>
        private static bool _initialized = false;

        /// <summary>
        /// Đối tượng dùng để khóa khi tải plugin logger.
        /// </summary>
        private static readonly object _lock = new();

        /// <summary>
        /// Ghi một thông điệp log vào file thông qua logger plugin.
        /// Nếu không có plugin logger, sử dụng MockLogger.
        /// </summary>
        /// <param name="message">Thông điệp cần ghi log.</param>
        public static void LogFile(string message)
        {
            try
            {
                if (!_initialized)
                    LoadPlugins();

                var logger = _cachedLoggers.FirstOrDefault() ?? new MockLogger();
                logger.Log(message);
            }
            catch (Exception ex)
            {
                new MockLogger().Log($"[LoggerError] {ex.Message}");
            }
        }

        /// <summary>
        /// Tải các plugin logger từ thư mục hiện tại và cache lại.
        /// Chỉ thực hiện một lần duy nhất.
        /// </summary>
        private static void LoadPlugins()
        {
            lock (_lock)
            {
                if (_initialized) return;

                string folder = AppDomain.CurrentDomain.BaseDirectory;
                var dlls = new DirectoryInfo(folder).GetFiles("*Plugin*.dll");

                foreach (var fi in dlls)
                {
                    try
                    {
                        var asm = Assembly.LoadFrom(fi.FullName);
                        foreach (var type in asm.GetTypes())
                        {
                            if (type.IsClass && typeof(ILogger).IsAssignableFrom(type) && type.Name != nameof(MockLogger))
                            {
                                if (Activator.CreateInstance(type) is ILogger instance)
                                    _cachedLoggers.Add(instance);
                            }
                        }
                    }
                    catch { continue; }
                }

                _initialized = true;
            }
        }
    }
}
