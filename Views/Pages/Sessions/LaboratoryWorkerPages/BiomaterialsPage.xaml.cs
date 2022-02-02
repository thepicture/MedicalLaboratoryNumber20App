using MedicalLaboratoryNumber20App.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
            IEnumerable<Blood> bloodEnumerable = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                    new MedicalLaboratoryNumber20Entities())
                {
                    return context.Blood
                    .Include(b => b.Patient)
                    .ToList();
                }
            });
            Biomaterials.ItemsSource = bloodEnumerable;
        }

        private void PerformBiomaterialAccept(object sender, RoutedEventArgs e)
        {
            Blood blood = (sender as Button).DataContext as Blood;
            _ = NavigationService.Navigate(new OrderPage(blood));
        }
    }
}
