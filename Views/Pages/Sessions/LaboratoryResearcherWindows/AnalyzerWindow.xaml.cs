using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;

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
            LoadUnperfomedServicesAsync();
        }

        /// <summary>
        /// Подгружает невыполненные услуги асинхронно.
        /// </summary>
        private async 
        /// <summary>
        /// Подгружает невыполненные услуги асинхронно.
        /// </summary>
        Task
LoadUnperfomedServicesAsync()
        {
            IEnumerable<BloodServiceOfUser> bloodServices =
                await Task.Run(() =>
                {
                    using (MedicalLaboratoryNumber20Entities context =
                    new MedicalLaboratoryNumber20Entities())
                    {
                        return context.BloodServiceOfUser
                        .Where(bs => bs.IsAccepted)
                        .Include(bs => bs.Blood)
                        .Include(bs => bs.Blood.Patient)
                        .Include(bs => bs.BloodStatus)
                        .Include(bs => bs.Service)
                        .ToList();
                    }
                });
            UnperformedServices.ItemsSource = bloodServices
                .Where(bs => bs.AnalyzerId == Analyzer.AnalyzerId);
        }

        public Analyzer Analyzer { get; private set; }

        /// <summary>
        /// Осуществляет отправку услуги на анализатор.
        /// </summary>
        private void PerformSendToAnalyzerAsync(object sender, RoutedEventArgs e)
        {
            Parallel.Invoke(async () =>
            {
                await Task.Delay(2000);
                Button button = sender as Button;
                BloodServiceOfUser service = button.DataContext as BloodServiceOfUser;
                PostService postService = new PostService
                {
                    Patient = service.Blood.Patient.PatientId.ToString(),
                    Services = new List<SimpleService>
                    {
                        new SimpleService
                        {
                            ServiceCode = service.Service.Code,
                        }
                    }.ToArray(),
                };
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string json = serializer.Serialize(postService);
                string address = $"http://localhost:5000/api/analyzer/{Analyzer.AnalyzerName}";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                        StringContent content = new StringContent(json,
                                                                  Encoding.UTF8,
                                                                  "application/json");

                        HttpResponseMessage response = await client.PostAsync(address,
                                                                              content);
                        string result = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            _ = await MessageBoxService.ShowWarning($"Возникла ошибка.\n" +
                                $"Сообщение: {result}");
                            return;

                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                using (MedicalLaboratoryNumber20Entities context =
                                new MedicalLaboratoryNumber20Entities())
                                {
                                    BloodServiceOfUser databaseBloodService =
                                    context.BloodServiceOfUser
                                    .Find(new object[] { service.BloodId, service.ServiceCode });
                                    databaseBloodService.BloodStatus =
                                    context.BloodStatus
                                    .First(b => b.BloodStatusName == "Отправлена на исследование");
                                    _ = context.SaveChanges();
                                }
                            });
                            button.IsEnabled = false;
                            button.Content = "Отправлена";
                            await LoadUnperfomedServicesAsync();
                            MessageBoxService.ShowInfo($"Услуга успешно отправлена");
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBoxService.ShowError("Не удалось отправить услугу. " +
                        "Прошло максимальное время ожидания в 30 секунд. " +
                        "Проверьте и исправьте настройки сети, затем повторите попытку");
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
                catch (DbException ex)
                {
                    MessageBoxService.ShowError("Не удалось отправить услугу. " +
                        "Возникла проблема с обновлением статуса в базе данных. " +
                        "Проверьте подключение к базе данных и повторите попытку");
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            });
        }
    }
}
