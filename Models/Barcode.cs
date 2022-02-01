using System.Collections.Generic;
using System.Linq;

namespace MedicalLaboratoryNumber20App.Models
{
    public class Barcode
    {
        public IEnumerable<BarcodeStrip> Strips { get; }

        public Barcode(IEnumerable<int> numbers)
        {
            Strips = numbers.Select(n => new BarcodeStrip(n));
        }
    }
}
