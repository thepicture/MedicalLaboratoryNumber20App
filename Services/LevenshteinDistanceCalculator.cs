using System;

namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Реализует вычисление по алгоритму Левенштейна.
    /// </summary>
    public class LevenshteinDistanceCalculator : ICalculator<int, string>
    {
        public int Calculate(string arg1, string arg2)
        {
            if (arg1.Length == 0)
            {
                return arg2.Length;
            }
            if (arg2.Length == 0)
            {
                return arg1.Length;
            }
            if (arg1[0] == arg2[0])
            {
                return Calculate(arg1.Substring(1), arg2.Substring(1));
            }
            return 1 + Math.Min(
                Math.Min(
                    Calculate(arg1.Substring(1), arg2),
                    Calculate(arg1, arg2.Substring(1))), Calculate(arg1.Substring(1), arg2.Substring(1))
                );
        }
    }
}
