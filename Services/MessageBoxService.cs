using System.Windows;

namespace MedicalLaboratoryNumber20App.Models.Services
{
    /// <summary>
    /// Реализует методы для представления различных 
    /// типов обратной связи.
    /// </summary>
    public static class MessageBoxService
    {
        /// <summary>
        /// Показывает информацию.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void ShowInfo(string message)
        {
            _ = MessageBox.Show(message,
                            "Информация",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        /// <summary>
        /// Показывает ошибку.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void ShowError(string message)
        {
            _ = MessageBox.Show(message,
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }

        /// <summary>
        /// Показывает предупреждение.
        /// </summary>
        /// <param name="message">Предупреждение.</param>
        public static void ShowWarning(string message)
        {
            _ = MessageBox.Show(message,
                            "Предупреждение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
        }

        /// <summary>
        /// Задаёт вопрос.
        /// </summary>
        /// <param name="question">Вопрос.</param>
        public static bool ShowQuestion(string question)
        {
            return MessageBox.Show(question,
                            "Вопрос",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
