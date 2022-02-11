using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryResearcherWindows
{
    /// <summary>
    /// Interaction logic for AnalyzerWindow.xaml
    /// </summary>
    public partial class AnalyzerWindow : Window, INotifyPropertyChanged
    {
        public string _address;
        private Patient currentPatient;

        public event PropertyChangedEventHandler PropertyChanged;

        public Patient CurrentPatient
        {
            get => currentPatient;
            set
            {
                currentPatient = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(currentPatient)));
            }
        }

        public AnalyzerWindow(Analyzer analyzer)
        {
            InitializeComponent();
            Analyzer = analyzer;
            _address = $"http://localhost:5000/api/analyzer/{Analyzer.AnalyzerName}";
            DataContext = this;
            _ = LoadUnperfomedServicesAsync();
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5),
            };
            timer.Tick += async (o, s) => await LoadPerformingServicesAsync();
            timer.Start();
        }

        /// <summary>
        /// Загружает выполяющиеся услуги асинхронно.
        /// </summary>
        private async Task LoadPerformingServicesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(_address);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                string json = await response.Content.ReadAsStringAsync();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                GetServiceList serviceList = (GetServiceList)serializer.Deserialize(json,
                                                                    typeof(GetServiceList));
                if (serviceList.Services != null)
                {
                    await Task.Run(() =>
                    {
                        using (MedicalLaboratoryNumber20Entities context =
                        new MedicalLaboratoryNumber20Entities())
                        {
                            foreach (SerializedService service in serviceList.Services)
                            {
                                Service currentDatabaseService = context.Service
                                                                .Find(service.ServiceCode);
                                service.ServiceName = currentDatabaseService.ServiceName;
                                if (double.TryParse(service.Result, out double result))
                                {

                                    bool isMeanDeviationTooHigh =
                                    (Convert.ToDouble(currentDatabaseService.MeanDeviation) / result > 5)
                                    || (Convert.ToDouble(currentDatabaseService.MeanDeviation) / result < (1.0 / 5));
                                    if (isMeanDeviationTooHigh)
                                    {
                                        _ = MessageBoxService.ShowWarningAsync("Возможный сбой " +
                                            "исследования " +
                                            "или некачественный биоматериал " +
                                            "для услуги " + service.ServiceName);
                                    }
                                }
                            }
                        }
                    });
                    PerformingServices.ItemsSource = serviceList.Services;
                }
                StatusBlock.Text = serviceList.Progress != null
                    ? $"Выполнено {serviceList.Progress}%"
                    : "Анализ выполнен";
            }
        }

        /// <summary>
        /// Подгружает невыполненные услуги асинхронно.
        /// </summary>
        private async Task LoadUnperfomedServicesAsync()
        {
            IEnumerable<BloodServiceOfUser> bloodServices =
                await Task.Run(() =>
                {
                    using (MedicalLaboratoryNumber20Entities context =
                    new MedicalLaboratoryNumber20Entities())
                    {
                        return context.BloodServiceOfUser
                        .Where(bs => bs.IsAccepted)
                        .Where(bs => bs.Service.Analyzer.Select(a => a.AnalyzerId).Contains(Analyzer.AnalyzerId))
                        .Where(bs => bs.BloodStatus.BloodStatusId == BloodStatuses.ShouldSend)
                        .Include(bs => bs.Blood)
                        .Include(bs => bs.Blood.Patient)
                        .Include(bs => bs.BloodStatus)
                        .Include(bs => bs.Service)
                        .OrderBy(bs => bs.Blood.Patient.PatientFullName)
                        .ToList();
                    }
                });
            UnperformedServices.ItemsSource = bloodServices;
        }

        public Analyzer Analyzer { get; }

        /// <summary>
        /// Осуществляет отправку услуги на анализатор.
        /// </summary>
        private void PerformSendToAnalyzerAsync(object sender, RoutedEventArgs e)
        {
            IEnumerable<BloodServiceOfUser> selectedServices = UnperformedServices
                .SelectedItems
                .Cast<BloodServiceOfUser>();
            if (selectedServices.Select(s => s.Blood.Patient.PatientId)
                .Distinct()
                .Count() != 1)
            {
                _ = MessageBoxService.ShowWarningAsync("Отправка услуг, " +
                    "принадлежащих более, чем одному пациенту, " +
                    "пока не поддерживается. Выберите список услуг, " +
                    "относящиеся к одному и тому же пациенту");
                return;
            }
            SerializedService[] services = selectedServices
                .ToList()
                .ConvertAll(bs => new SerializedService { ServiceCode = bs.Service.Code })
                .ToArray();
            Patient patientOfSelectedService = selectedServices
                    .First().Blood.Patient;
            currentPatient = patientOfSelectedService;
            Parallel.Invoke(async () =>
            {
                PostServiceList postService = new PostServiceList
                {
                    Patient = patientOfSelectedService.PatientId
                    .ToString(),
                    Services = services
                };
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string json = serializer.Serialize(postService);

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                        StringContent content = new StringContent(json,
                                                                  Encoding.UTF8,
                                                                  "application/json");

                        HttpResponseMessage response = await client.PostAsync(_address,
                                                                              content);
                        string result = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            _ = await MessageBoxService.ShowWarningAsync($"Возникла ошибка.\n" +
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
                                    foreach (BloodServiceOfUser service in selectedServices)
                                    {
                                        BloodServiceOfUser databaseBloodService =
                                        context.BloodServiceOfUser
                                        .Find(new object[] { service.BloodId, service.ServiceCode });
                                        databaseBloodService.BloodStatus =
                                        context.BloodStatus
                                        .First(b => b.BloodStatusId == BloodStatuses.Sent);
                                    }
                                    _ = context.SaveChanges();
                                }
                            });
                            PerformingServices.ItemsSource = selectedServices
                            .ToList()
                            .ConvertAll(s => new SerializedService
                            {
                                ServiceCode = s.ServiceCode,
                                ServiceName = s.Service.ServiceName,
                            });
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

        /// <summary>
        /// Вызывается в момент одобрения исследования.
        /// </summary>
        private async void OnConformingResultAsync(object sender, RoutedEventArgs e)
        {
            if (!MessageBoxService.ShowQuestion("Вы уверены, что данную услугу следует одобрить?"))
            {
                return;
            }
            Button button = sender as Button;
            SerializedService service = button.DataContext as SerializedService;
            await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    BloodServiceOfUser bloodOfService = context.BloodServiceOfUser
                    .First(s => s.ServiceCode == service.ServiceCode
                                && s.Blood.PatientId == CurrentPatient.PatientId
                                && s.IsAccepted &&
                                s.BloodStatusId == BloodStatuses.Sent);
                    bloodOfService.BloodStatusId = BloodStatuses.Complete;
                    bloodOfService.Result = service.Result;
                    _ = context.SaveChanges();
                }
            });
            PerformingServices.ItemsSource = PerformingServices.Items
                .Cast<SerializedService>()
                .Where(s => s.ServiceCode != service.ServiceCode);
        }

        /// <summary>
        /// Вызывается при необходимости повторного забора материала.
        /// </summary>
        private async void OnNonConformingResultAsync(object sender, RoutedEventArgs e)
        {
            if (!MessageBoxService.ShowQuestion("Вы уверены, " +
                "что необходим повторный забор материала?"))
            {
                return;
            }
            Button button = sender as Button;
            SerializedService service = button.DataContext as SerializedService;
            await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    BloodServiceOfUser bloodOfService = context.BloodServiceOfUser
                    .First(s => s.ServiceCode == service.ServiceCode
                                && s.Blood.PatientId == CurrentPatient.PatientId
                                && s.IsAccepted
                                && s.BloodStatusId == BloodStatuses.Sent);
                    bloodOfService.BloodStatusId = BloodStatuses.ShouldSend;
                    _ = context.SaveChanges();
                }
            });
            PerformingServices.ItemsSource = PerformingServices.Items
                .Cast<SerializedService>()
                .Where(s => s.ServiceCode != service.ServiceCode
                && s.Result != service.Result);
            await LoadUnperfomedServicesAsync();
        }
    }
}
