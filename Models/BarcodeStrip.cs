using System.Windows;

namespace MedicalLaboratoryNumber20App.Models
{
    /// <summary>
    /// Реализует преобразование номера в 
    /// ширину и цифру штриха.
    /// </summary>
    public class BarcodeStrip
    {
        public BarcodeStrip(int number)
        {
            Width = (double)new LengthConverter()
                .ConvertFrom($"{ number * 0.015}cm");
            Number = number;
        }

        public double Width { get; }
        public int Number { get; }
    }
}
