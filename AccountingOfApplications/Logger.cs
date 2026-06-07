using System;
using System.IO;
using System.Windows.Forms;

namespace AccountingOfApplications
{
    public static class Logger
    {
        private static string logFilePath;

        static Logger()
        {
            // Папка для логов в документах пользователя
            string logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AccountingLogs");

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            // Имя файла: log_2026-06-02.txt
            string fileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
            logFilePath = Path.Combine(logFolder, fileName);
        }

        public static void Write(string message, string level = "INFO")
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Если не удалось записать в лог, показываем ошибку
                MessageBox.Show($"Ошибка записи в лог: {ex.Message}");
            }
        }

        public static void WriteError(string message)
        {
            Write(message, "ERROR");
        }

        public static void WriteWarning(string message)
        {
            Write(message, "WARNING");
        }

        public static void WriteInfo(string message)
        {
            Write(message, "INFO");
        }
    }
}