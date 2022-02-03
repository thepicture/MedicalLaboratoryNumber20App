using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryWorkerPages
{
    /// <summary>
    /// Interaction logic for AddPatientWindow.xaml
    /// </summary>
    public partial class AddPatientWindow : Window
    {
        public Patient Patient { get; set; }
           = new Patient();

        public AddPatientWindow(string fullName)
        {
            InitializeComponent();
            Patient.PatientFullName = fullName;
            Patient.BirthDate = System.DateTime.Now;
            DataContext = this;
            LoadInsuranceCompanies();
            LoadSocialTypes();
        }

        /// <summary>
        /// Загружает страховые полисы.
        /// </summary>
        private async void LoadSocialTypes()
        {
            IEnumerable<PatientSocialType> socialTypesItems =
                await Task.Run(() =>
                {
                    using (MedicalLaboratoryNumber20Entities context =
                        new MedicalLaboratoryNumber20Entities())
                    {
                        return context.PatientSocialType.ToList();
                    }
                });
            SocialTypes.ItemsSource = socialTypesItems;
            SocialTypes.SelectedItem = socialTypesItems.First();
        }

        /// <summary>
        /// Загружает страховые компании.
        /// </summary>
        private async void LoadInsuranceCompanies()
        {
            IEnumerable<InsuranceCompany> insuranceCompaniesItems =
              await Task.Run(() =>
              {
                  using (MedicalLaboratoryNumber20Entities context =
                      new MedicalLaboratoryNumber20Entities())
                  {
                      return context.InsuranceCompany.ToList();
                  }
              });
            InsuranceCompanies.ItemsSource = insuranceCompaniesItems;
            InsuranceCompanies.SelectedItem = insuranceCompaniesItems.First();
        }

        /// <summary>
        /// Вызывается в момент добавления пациента.
        /// </summary>
        private void OnPatientSave(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Вызывается в момент отмены добавления пациента.
        /// </summary>
        private void PerformGoBack(object sender, RoutedEventArgs e)
        {
            if (MessageBoxService.ShowQuestion("Действительно отменить " +
                "добавление пациента?"))
            {
                DialogResult = false;
            }
        }
    }
}
