namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Определяет метод для вычисления значения.
    /// </summary>
    /// <typeparam name="TResult">Тип вычисленного значения.</typeparam>
    /// <typeparam name="TArg">Тип входного аргумента.</typeparam>
    public interface ICalculator<TResult, TArg>
    {
        /// <summary>
        /// Вычисляет значение.
        /// </summary>
        /// <param name="arg1">Первый аргумент для вычисления.</param>
        /// <param name="arg2">Второй аргумент для вычисления.</param>
        /// <returns>Вычисленное значение.</returns>
        TResult Calculate(TArg arg1, TArg arg2);
    }
}
