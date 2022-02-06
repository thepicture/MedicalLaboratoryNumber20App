using MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryResearcherPages;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions
{
    /// <summary>
    /// Interaction logic for LaboratoryResearcherPage.xaml
    /// </summary>
    public partial class LaboratoryResearcherPage : Page
    {
        public LaboratoryResearcherPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void GoToAnalyzersPage(object sender, System.Windows.RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new AnalyzersPage());
        }
    }
}
