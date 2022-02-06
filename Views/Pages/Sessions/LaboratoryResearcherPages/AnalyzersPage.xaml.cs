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
                    .Include(a => a.BloodServiceOfUser)
                    .Include(a => a.BloodServiceOfUser.Select(b => b.Service))
                    .ToList();
                }
            });
            Analyzers.ItemsSource = currentAnalyzers
                .Select(a =>
                {
                    return new Analyzer
                    {
                        AnalyzerName = a.AnalyzerName,
                        BloodServiceOfUser = new List<BloodServiceOfUser>()
                        {
                            new BloodServiceOfUser {
                                Service = new Service
                                {
                                    ServiceName = string.Join(", ", a.BloodServiceOfUser
                                        .Where(b => b.UserId == (App.Current as App).User.UserId)
                                        .Select(b => b.Service.ServiceName)
                                        .Distinct())
                                }
                            }
                        }
                    };
                });
        }
    }
}
