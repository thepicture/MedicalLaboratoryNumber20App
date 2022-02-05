namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Определяет метод для генерации отчёта.
    /// </summary>
    /// <typeparam name="T">Входной тип для генерации отчёта.</typeparam>
    public interface IReportService<T>
    {
        /// <summary>
        /// Генерирует отчёт в соответствии со входным объектом.
        /// </summary>
        /// <param name="obj">Входной объект.</param>
        /// <param name="path">Путь для сохранения отчёта.</param>
        /// <returns>Возвращает true, если отчёт успешно сформирован, 
        /// иначе  возвращает false.</returns>
        bool GenerateReport(T obj, string path);
    }
}
