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
        }

        public Analyzer Analyzer { get; private set; }
    }
}
