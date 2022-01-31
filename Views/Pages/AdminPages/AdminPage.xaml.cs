using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.AdminPages
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void NavigateToLoginHistories(object sender, System.Windows.RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new LoginHistoryPage());
        }
    }
}
