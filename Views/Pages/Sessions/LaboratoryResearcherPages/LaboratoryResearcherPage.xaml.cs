using MedicalLaboratoryNumber20App.Services;
using System;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions
{
    /// <summary>
    /// Interaction logic for LaboratoryResearcherPage.xaml
    /// </summary>
    public partial class LaboratoryResearcherPage : Page
    {
        public TimerService TimerService { get; }
        public LaboratoryResearcherPage()
        {
            InitializeComponent();
            TimerService = new TimerService(TimeSpan.FromMinutes(10),
                                             TimeSpan.FromMinutes(5),
                                             TimeSpan.FromMinutes(1));
            TimerService.StartForPage(this);
            DataContext = this;
        }
    }
}
