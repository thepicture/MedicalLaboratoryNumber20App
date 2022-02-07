using MedicalLaboratoryNumber20App.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
            LoadAnalyzers();
        }

        /// <summary>
        /// Подгружает анализаторы асинхронно в таблицу.
        /// </summary>
        private async void LoadAnalyzers()
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
    }
}
