using MedicalLaboratoryNumber20App.Models.Entities;

namespace MedicalLaboratoryNumber20App.Models
{
    /// <summary>
    /// Представляет собой пару значений
    /// услуга - количество оказанной услуги.
    /// </summary>
    public class ServiceAndApplyPoint
    {
        public ServiceAndApplyPoint(Service currentService, int applyCount)
        {
            CurrentService = currentService;
            ApplyCount = applyCount;
        }

        public void Increment() => ApplyCount++;

        public Service CurrentService { get; set; }
        public int ApplyCount { get; set; }
    }
}
