using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryWorkerPages
{
    /// <summary>
    /// Interaction logic for AddPatientWindow.xaml
    /// </summary>
    public partial class AddPatientWindow : Window
    {
        public Patient Patient { get; set; } = new Patient();

        public AddPatientWindow()
        {
            InitializeComponent();
            DataContext = this;
            Patient.BirthDate = System.DateTime.Now;
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
        private async void OnPatientSave(object sender, RoutedEventArgs e)
        {
            Patient.InsuranceCompanyId = (InsuranceCompanies.SelectedItem as InsuranceCompany).InsuranceCompanyId;
            Patient.SocialTypeId = (SocialTypes.SelectedItem as PatientSocialType).SocialTypeId;
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(Patient.PatientFullName))
            {
                _ = errors.AppendLine("Укажите ФИО");
            }
            if (Patient.BirthDate == null || Patient.BirthDate >= DateTime.Now)
            {
                _ = errors.AppendLine("Укажите корректную дату рождения");
            }
            if (!int.TryParse(PatientPassportSeries.Text, out _)
                || PatientPassportSeries.Text.Length != 4)
            {
                _ = errors.AppendLine("Укажите корректную серию паспорта (4 цифры)");
            }
            if (!int.TryParse(PatientPassportNumber.Text, out _)
                || PatientPassportNumber.Text.Length != 6)
            {
                _ = errors.AppendLine("Укажите корректный номер паспорта (6 цифр)");
            }
            if (string.IsNullOrWhiteSpace(Patient.PatientPhone))
            {
                _ = errors.AppendLine("Укажите телефон");
            }
            if (string.IsNullOrWhiteSpace(Patient.PatientEmail))
            {
                _ = errors.AppendLine("Укажите e-mail");
            }
            if (string.IsNullOrWhiteSpace(Patient.SecurityNumber)
                || !int.TryParse(Patient.SecurityNumber, out _))
            {
                _ = errors.AppendLine("Укажите номер страхового полиса");
            }

            if (errors.Length > 0)
            {
                MessageBoxService.ShowError(errors.ToString());
                return;
            }

            bool isSaved = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    _ = context.Patient.Add(Patient);
                    try
                    {
                        _ = context.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        return false;
                    }
                }
            });

            if (isSaved)
            {
                DialogResult = true;
            }
            else
            {
                MessageBoxService.ShowError("Пациент " +
                    "не добавлен. Попробуйте ещё раз или " +
                    "перезапустите модальное окно");
            }
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
