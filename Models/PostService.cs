namespace MedicalLaboratoryNumber20App.Models
{
    public class PostService
    {
        public PostService()
        {
        }

        public string Patient { get; set; }
        public SimpleService[] Services { get; set; }
    }
}
