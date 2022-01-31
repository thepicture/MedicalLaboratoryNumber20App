using MedicalLaboratoryNumber20App.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.AdminPages
{
    /// <summary>
    /// Interaction logic for LoginHistoryPage.xaml
    /// </summary>
    public partial class LoginHistoryPage : Page
    {
        public LoginHistoryPage()
        {
            InitializeComponent();
            IList<string> sortTypes = new List<string>
            {
                "Без сортировки",
                "По возрастанию",
                "По убыванию",
            };
            ComboSort.ItemsSource = sortTypes;
            DataContext = this;
        }
        private async void PerformSearchHistories()
        {
            using (MedicalLaboratoryNumber20Entities context = new MedicalLaboratoryNumber20Entities())
            {
                LoginHistories.ItemsSource = await context.LoginHistory.ToListAsync();
            }
        }

        private void OnLoginSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            PerformSearchHistories();
        }

        private void OnSortChanged(object sender, SelectionChangedEventArgs e)
        {
            PerformSearchHistories();
        }
    }
}
