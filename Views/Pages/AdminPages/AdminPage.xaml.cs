using MedicalLaboratoryNumber20App.Views.Pages.ReportPages;
using System.Windows;
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

        private void NavigateToLoginHistories(object sender, RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new LoginHistoryPage());
        }

        private void GoToReportCreationPage(object sender, RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new WhatReportUserNeedPage());
        }
    }
}
