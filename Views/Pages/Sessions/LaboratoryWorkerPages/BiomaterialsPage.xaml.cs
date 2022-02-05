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
        }

        /// <summary>
        /// Подгружает асинхронно биоматериалы в список.
        /// </summary>
        private async void LoadBiomaterials()
        {
            IEnumerable<Blood> bloodEnumerable = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                    new MedicalLaboratoryNumber20Entities())
                {
                    return context.Blood
                    .Include(b => b.Patient)
                    .Where(b => b.Order.Count == 0)
                    .ToList();
                }
            });
            Biomaterials.ItemsSource = bloodEnumerable;
        }

        /// <summary>
        /// Производит навигацию на страницу добавления заказа.
        /// </summary>
        private void PerformBiomaterialAccept(object sender, RoutedEventArgs e)
        {
            Blood blood = (sender as Button).DataContext as Blood;
            _ = NavigationService.Navigate(new OrderPage(blood));
        }

        /// <summary>
        /// Вызывается в момент обновления видимости страницы.
        /// </summary>
        private void OnLoad(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                LoadBiomaterials();
            }
        }
    }
}
