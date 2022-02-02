using MedicalLaboratoryNumber20App.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
            ComboSort.SelectedItem = sortTypes.First();
            DataContext = this;
            PerformSearchHistories();
        }

        /// <summary>
        /// Выводит историю входа, отфильтрованную или отсортированную 
        /// в зависимости от соответствующих параметров.
        /// </summary>
        private async void PerformSearchHistories()
        {
            IEnumerable<LoginHistory> currentLoginHistories =
                await Task.Run(() =>
                {
                    using (MedicalLaboratoryNumber20Entities context
                        = new MedicalLaboratoryNumber20Entities())
                    {
                        return context.LoginHistory.ToList();
                    }
                });
            switch (ComboSort.SelectedItem)
            {
                case "По возрастанию":
                    currentLoginHistories = currentLoginHistories
                       .OrderBy(s => s.LoginDateTime);
                    break;
                case "По убыванию":
                    currentLoginHistories = currentLoginHistories
                       .OrderByDescending(s => s.LoginDateTime);
                    break;
                case "Без сортировки":
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(LoginSearchBox.Text))
            {
                currentLoginHistories = currentLoginHistories
                    .Where(lh =>
                    {
                        return lh.EnteredLogin
                        .ToLower()
                        .Contains(LoginSearchBox.Text.ToLower());
                    });
            }
            LoginHistories.ItemsSource = currentLoginHistories;
        }

        /// <summary>
        /// Вызывается в момент изменения текста поиска.
        /// </summary>
        private void OnLoginSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            PerformSearchHistories();
        }

        /// <summary>
        /// Вызывается в момент изменения типа сортировки.
        /// </summary>
        private void OnSortChanged(object sender, SelectionChangedEventArgs e)
        {
            PerformSearchHistories();
        }
    }
}
