using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using System.Windows;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions
{
    /// <summary>
    /// Interaction logic for LaboratoryWorkerPage.xaml
    /// </summary>
    public partial class LaboratoryWorkerPage : Page
    {
        public LaboratoryWorkerPage(User user)
        {
            InitializeComponent();
            User = user;
            DataContext = this;
        }

        public User User { get; }
    }
}
