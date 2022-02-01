using MedicalLaboratoryNumber20App.Models;
using System.Collections.Generic;
using System.Linq;

namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Реализует статические методы для 
    /// генерации и сохранения штрих-кода.
    /// </summary>
    public static class BarcodeService
    {
        /// <summary>
        /// Создаёт новый штрих-код.
        /// </summary>
        /// <param name="barcodeText">Цифры штрих-кода.</param>
        /// <returns>Созданный штрих-код.</returns>
        public static Barcode NewBarcode(string barcodeText)
        {
            IEnumerable<int> barcodeNumbers = barcodeText
                .ToCharArray()
                .Select(c => int.Parse(c.ToString()));
            Barcode barcode = new Barcode(barcodeNumbers);
            return barcode;
        }
    }
}
