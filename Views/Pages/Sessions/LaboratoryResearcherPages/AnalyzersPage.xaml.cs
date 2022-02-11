using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryResearcherWindows;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryResearcherPages
{
    /// <summary>
    /// Interaction logic for AnalyzersPage.xaml
    /// </summary>
    public partial class AnalyzersPage : Page
    {
        public AnalyzersPage()
        {
            InitializeComponent();
            _ = LoadAnalyzersAsync();
        }

        /// <summary>
        /// Подгружает анализаторы асинхронно в таблицу.
        /// </summary>
        private async Task LoadAnalyzersAsync()
        {
            IEnumerable<Analyzer> currentAnalyzers = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.Analyzer
                    .Include(a => a.Service)
                    .ToList();
                }
            });
            Analyzers.ItemsSource = currentAnalyzers;
        }

        /// <summary>
        /// Осуществляет открытие окна анализатора.
        /// </summary>
        private void PerformAnalyzerWindowOpen(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string buttonOldContent = button.Content.ToString();
            Analyzer analyzer = button.DataContext as Analyzer;
            AnalyzerWindow analyzerWindow = new AnalyzerWindow(analyzer)
            {
                Owner = App.Current.MainWindow
            };
            analyzerWindow.Closed += (window, args) =>
            {
                button.Content = buttonOldContent;
                button.IsEnabled = true;
            };
            button.Content = "Окно уже открыто";
            button.IsEnabled = false;
            analyzerWindow.Show();
        }
    }
}
