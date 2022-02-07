using MedicalLaboratoryNumber20App.Models.Entities;
using System.Windows;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryResearcherWindows
{
    /// <summary>
    /// Interaction logic for AnalyzerWindow.xaml
    /// </summary>
    public partial class AnalyzerWindow : Window
    {
        public AnalyzerWindow(Analyzer analyzer)
        {
            InitializeComponent();
            Analyzer = analyzer;
            DataContext = this;
            UnperformedServices.ItemsSource = analyzer.Service;
        }

        public Analyzer Analyzer { get; private set; }

        /// <summary>
        /// Осуществляет отправку услуги на анализатор.
        /// </summary>
        private void PerformSendToAnalyzer(object sender, RoutedEventArgs e)
        {

        }
    }
}
