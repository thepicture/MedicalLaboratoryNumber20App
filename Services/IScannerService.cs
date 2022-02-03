namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Определяет методы для сканирования данных.
    /// </summary>
    /// <typeparam name="T">Тип сканируемых данных.</typeparam>
    public interface IScannerService<T>
    {
        /// <summary>
        /// Сканирует данные.
        /// </summary>
        /// <returns>Отсканированные данные.</returns>
        T Scan();
    }
}
