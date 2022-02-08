namespace MedicalLaboratoryNumber20App.Models
{
    public class PostServiceList
    {
        public PostServiceList()
        {
        }

        public string Patient { get; set; }
        public SerializedService[] Services { get; set; }
    }
}
