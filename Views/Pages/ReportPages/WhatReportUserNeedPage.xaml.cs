using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MedicalLaboratoryNumber20App.Views.Pages.ReportPages
{
    /// <summary>
    /// Interaction logic for WhatReportUserNeedPage.xaml
    /// </summary>
    public partial class WhatReportUserNeedPage : Page
    {
        public WhatReportUserNeedPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Осуществляет навигацию 
        /// на страницу формирования отчёта 
        /// по контролю качества.
        /// </summary>
        private void GoToQualityControlReportPage(object sender, RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new QualityControlReportPage());
        }

        /// <summary>
        /// Осуществляет навигацию 
        /// на страницу формирования отчёта 
        /// по оказанным услугам.
        /// </summary>
        private void GoToAppliedServicesReportPage(object sender, RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new AppliedServicesReportPage());
        }
    }
}
