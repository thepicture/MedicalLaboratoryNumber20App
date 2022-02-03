using System;

namespace MedicalLaboratoryNumber20App.Services
{
    public class BarcodeScannerService : IScannerService<string>
    {
        private const int MinRandomValue = 1;
        private const int MaxRandomValue = 100;
        private const int MinSixDigitValue = 100000;
        private const int MaxSixDigitValue = 999999 + 1;
        private readonly Random random;

        public BarcodeScannerService()
        {
            random = new Random();
        }

        public string Scan()
        {
            return $"{random.Next(MinRandomValue, MaxRandomValue)}"
                   + $"{DateTime.Now:yyyyMMdd}"
                   + $"{random.Next(MinSixDigitValue, MaxSixDigitValue)}"
                   + "\r";
        }
    }
}
