using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MedicalLaboratoryNumber20App.Views.Pages.AccountantPages
{
    /// <summary>
    /// Interaction logic for AccountantPage.xaml
    /// </summary>
    public partial class AccountantPage : Page
    {
        public AccountantPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Формирует счёт страховой компании в формате .pdf и .csv.
        /// </summary>
        private async void PerformGenerateOrder(object sender,
                                                RoutedEventArgs e)
        {
            if (FromPicker.SelectedDate == null
                || ToPicker.SelectedDate == null)
            {
                _ = MessageBoxService.ShowWarning("Укажите " +
                    "корректную дату " +
                    "начала и окончания периода");
                return;
            }
            if (FromPicker.SelectedDate >= ToPicker.SelectedDate)
            {
                _ = MessageBoxService
                    .ShowWarning("Дата начала периода "
                                 + "должна быть " +
                                 "раньше даты " +
                                 "окончания периода");
                return;
            }
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Укажите путь " +
                "к папке " +
                "с сохраненными отчётами"
            };

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            GenerateOrderButton.IsEnabled = false;
            string buttonOldContent = GenerateOrderButton.Content.ToString();
            GenerateOrderButton.Content = "Формируется отчёт ...";

            DateTime? from = FromPicker.SelectedDate;
            DateTime? to = ToPicker.SelectedDate;

            IEnumerable<InsuranceCompany> companies = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.InsuranceCompany
                    .Include(c => c.Patient)
                    .Include(c => c.Patient.Select(p => p.Blood))
                    .Include(c => c.Patient.Select(p => p.Blood.Select(b => b.BloodServiceOfUser)))
                    .Include(c => c.Patient.Select(p => p.Blood.Select(b => b.BloodServiceOfUser.Select(bs => bs.Service))))
                    .Where(c => Enumerable.All(c.Patient.Select(p => p.Blood.Select(b => b.BloodServiceOfUser.Select(bs => bs.FinishedDateTime))), d1 => d1.All(d2 => d2.All(d3 => d3 < to && d3 > from))))
                    .ToList();
                }
            });
            if (companies.Count() == 0)
            {
                _ = MessageBoxService.ShowWarning("За указанный период не найдено услуг");
                return;
            }
            if (companies.Select(c => c.Patient).Distinct().Count() > 500)
            {
                if (!MessageBoxService.ShowQuestion("За указанный период " +
                    "найдено более 500 пациентов. Формирование отчёта может занять " +
                    "долгое время. Продолжить?"))
                {
                    return;
                }
            }

            InsuranceCompanyReportService reportService =
                new InsuranceCompanyReportService(FromPicker.SelectedDate,
                                                  ToPicker.SelectedDate);
            bool isSuccessfulReporting = await Task.Run(() =>
            {
                return reportService.GenerateReport(companies,
                                                    folderBrowserDialog.SelectedPath);
            });
            if (isSuccessfulReporting)
            {
                MessageBoxService.ShowInfo("Отчёт успешно сгенерирован " +
                    $"по пути {folderBrowserDialog.SelectedPath}");
            }
            else
            {
                MessageBoxService.ShowError("Отчёт не сгенерирован. " +
                    $"убедитесь, что путь {folderBrowserDialog.SelectedPath} " +
                    "существует и попробуйте ещё раз");
            }

            GenerateOrderButton.Content = buttonOldContent;
            GenerateOrderButton.IsEnabled = true;
        }
    }
}
