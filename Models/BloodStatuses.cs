namespace MedicalLaboratoryNumber20App.Models
{
    /// <summary>
    /// Представляет собой перечисление статусов биоматериала.
    /// </summary>
    public static class BloodStatuses
    {
        public const int Complete = 1;
        public const int ShouldSend = 2;
        public const int Sent = 3;
    }
}
