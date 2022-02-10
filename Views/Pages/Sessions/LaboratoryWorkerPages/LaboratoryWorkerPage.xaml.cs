using MedicalLaboratoryNumber20App.Views.Pages.ReportPages;
using MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryWorkerPages;
using System.Windows;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions
{
    /// <summary>
    /// Interaction logic for LaboratoryWorkerPage.xaml
    /// </summary>
    public partial class LaboratoryWorkerPage : Page
    {
        public LaboratoryWorkerPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void NavigateToBiomaterials(object sender, RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new BiomaterialsPage());
        }

        private void GoToReportCreationPage(object sender, RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new WhatReportUserNeedPage());
        }
    }
}
