namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Определяет метод для экспорта данных.
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Экспортирует данные.
        /// </summary>
        /// <param name="filePath">Путь к файлу, если данные сохранены, 
        /// иначе значение по умолчанию.</param>
        /// <returns>Возвращает true, если данные экспортированы, иначе false.</returns>
        bool Export(out string filePath);
    }
}
