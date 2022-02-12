using System;

namespace MedicalLaboratoryNumber20App.Models
{
    /// <summary>
    /// Задаёт пару точек для представления контроля качества.
    /// </summary>
    public class QualityControlPointPair
    {
        public QualityControlPointPair(string x, decimal y)
        {
            X = x;
            Y = y;
        }

        public string X { get; set; }
        public decimal Y { get; set; }
    }
}
