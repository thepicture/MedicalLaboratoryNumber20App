namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Определяет метод для сканирования данных.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемых отсканированных данных.</typeparam>
    public interface IScannerService<T>
    {
        /// <summary>
        /// Сканирует данные.
        /// </summary>
        /// <returns>Отсканированные данные.</returns>
        T Scan();
    }
}
