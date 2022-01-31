using MedicalLaboratoryNumber20App.Models.Entities;
using System.Data.Entity;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryWorkerPages
{
    /// <summary>
    /// Interaction logic for BiomaterialsPage.xaml
    /// </summary>
    public partial class BiomaterialsPage : Page
    {
        public BiomaterialsPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadBiomaterials();
        }

        private async void LoadBiomaterials()
        {
            using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
            {
                Biomaterials.ItemsSource = await context.Blood
                    .Include(b => b.Patient)
                    .ToListAsync();
            }
        }
    }
}
