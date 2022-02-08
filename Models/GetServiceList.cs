namespace MedicalLaboratoryNumber20App.Models
{
    public class GetServiceList
    {
        public GetServiceList()
        {
        }

        public string Patient { get; set; }
        public SerializedService[] Services { get; set; }
        public int? Progress { get; set; }
    }
}
