using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.ReportPages
{
    /// <summary>
    /// Interaction logic for AppliedServicesReportPage.xaml
    /// </summary>
    public partial class AppliedServicesReportPage : Page, INotifyPropertyChanged
    {
        public readonly List<string> ReportViewTypesList = new List<string>
            {
                "графиком",
                "таблицей",
            };
        public readonly List<string> ReportSaveTypesList = new List<string>
            {
                "график",
                "только таблица",
                "график и таблица",
            };

        public string CurrentViewType
        {
            get => _currentViewType;
            set
            {
                _currentViewType = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(CurrentViewType)));
                FilterView();
            }
        }

        private bool _isBusy;

        /// <summary>
        /// Фильтрует представление в зависимости от выбранных параметров.
        /// </summary>
        private async void FilterView()
        {
            if ((FromDate == null && ToDate == null)
                || FromDate >= ToDate)
            {
                return;
            }
            BloodServices = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.BloodServiceOfUser
                    .Where(bs => bs.FinishedDateTime < ToDate
                                 && bs.FinishedDateTime > FromDate)
                    .Include(bs => bs.Service)
                    .Include(bs => bs.Blood)
                    .Include(bs => bs.Blood.Patient)
                    .ToList();
                }
            });
            ChartHost.Visibility = System.Windows.Visibility.Collapsed;
            PointsGrid.Visibility = System.Windows.Visibility.Collapsed;
            switch (CurrentViewType)
            {
                case "графиком":
                    ChartHost.Visibility = System.Windows.Visibility.Visible;
                    LoadAsChart();
                    break;
                case "таблицей":
                    PointsGrid.Visibility = System.Windows.Visibility.Visible;
                    await LoadAsTable();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Загружает данные в виде графика.
        /// </summary>
        private void LoadAsChart()
        {
            IEnumerable<ServiceAndApplyPoint> serviceAndApplyPoints =
                BloodServices
                .Select(bs => bs.Service)
                .Distinct()
                .Select(s => new ServiceAndApplyPoint(s, 0))
                .ToList();
            foreach (BloodServiceOfUser bloodServiceOfUser in BloodServices)
            {
                serviceAndApplyPoints
                    .First(p =>
                    {
                        return p.CurrentService.Code == bloodServiceOfUser.ServiceCode;
                    })
                    .Increment();
            }
            AppliedServicesSeries.Points.Clear();
            AppliedServicesSeries.LegendText = $"Оказанные услуги " +
                $"с {FromDate:yyyy-mm-dd} по {ToDate:yyyy-mm-dd}";
            foreach (ServiceAndApplyPoint point in serviceAndApplyPoints)
            {
                _ = AppliedServicesSeries.Points
                    .AddXY(point.CurrentService.ServiceName,
                           point.ApplyCount);
            }
        }

        /// <summary>
        /// Загружает данные в виде таблицы.
        /// </summary>
        private async Task LoadAsTable()
        {
            IsBusy = true;
            IList<KeyValueRow> keyValueRows = new List<KeyValueRow>();
            await Task.Run(() =>
            {
                keyValueRows.Add(new KeyValueRow("Количество оказанных услуг за период времени",
                                             BloodServices.Count()
                                                          .ToString()));
                keyValueRows.Add(new KeyValueRow("Перечень услуг за период времени",
                                                 string.Join(", ", BloodServices.Select(bs => bs.Service.ServiceName)
                                                 .Distinct())));
                keyValueRows.Add(new KeyValueRow("Количество пациентов",
                                                 BloodServices.Select(bs => bs.Blood.Patient.PatientId)
                                                 .Count()
                                                 .ToString()));
                keyValueRows.Add(new KeyValueRow("Количество пациентов " +
                    "в день по каждой услуге",
                    "Смотрите ниже"));
                IEnumerable<Service> services = BloodServices
                    .Select(bs => bs.Service)
                    .Distinct()
                    .ToList();
                foreach (Service service in services)
                {
                    keyValueRows.Add(new KeyValueRow("Наименование услуги",
                   service.ServiceName));
                    for (DateTime i = FromDate; i < ToDate; i = i.AddDays(1))
                    {
                        keyValueRows.Add(new KeyValueRow(i.ToString("yyyy-MM-dd"),
                                                    BloodServices.Where(bs => bs.FinishedDateTime > i
                                                    && bs.FinishedDateTime < i.AddDays(1))
                                                    .Where(bs => bs.ServiceCode == service.Code)
                                                    .Count()
                                                    .ToString()));
                    }
                }
                keyValueRows.Add(new KeyValueRow("Средний результат " +
                    "кажждого исследования в день " +
                    "по выбранному периоду",
                    "Смотрите ниже"));
                foreach (Service service in services)
                {
                    keyValueRows.Add(new KeyValueRow("Наименование услуги",
                 service.ServiceName));
                    for (DateTime i = FromDate; i < ToDate; i = i.AddDays(1))
                    {
                        IEnumerable<BloodServiceOfUser> currentBloodServices = BloodServices
                            .Where(bs =>
                            {
                                return bs.FinishedDateTime > i && bs.FinishedDateTime < i.AddDays(1);
                            })
                            .Where(bs => bs.ServiceCode == service.Code)
                            .Where(bs => decimal.TryParse(bs.Result, out _));
                        if (currentBloodServices.Count() > 0)
                        {
                            keyValueRows.Add(new KeyValueRow(i.ToString("yyyy-MM-dd"),
                                                        currentBloodServices
                                                        .Average(bs => decimal.Parse(bs.Result))
                                                        .ToString("N2")));
                        }
                        else
                        {
                            keyValueRows.Add(new KeyValueRow(i.ToString("yyyy-MM-dd"), "0"));
                        }
                    }
                }
            });
            PointsGrid.ItemsSource = keyValueRows;
            IsBusy = false;
        }

        public string CurrentSaveType
        {
            get => _currentSaveType;
            set
            {
                _currentSaveType = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(CurrentSaveType)));
            }
        }

        public DateTime FromDate
        {
            get => _fromDate;
            set
            {
                _fromDate = value;
                PropertyChanged?.Invoke(this,
                                     new PropertyChangedEventArgs(nameof(FromDate)));
                FilterView();
            }
        }
        public DateTime ToDate
        {
            get => _toDate;
            set
            {
                _toDate = value;
                PropertyChanged?.Invoke(this,
                                     new PropertyChangedEventArgs(nameof(ToDate)));
                FilterView();
            }
        }

        public List<BloodServiceOfUser> BloodServices { get; set; }
        public bool IsBusy
        {
            get => _isBusy; set
            {
                _isBusy = value;
                PropertyChanged?.Invoke(this,
                                  new PropertyChangedEventArgs(nameof(IsBusy)));
            }
        }

        private string _currentSaveType;
        private string _currentViewType;
        private DateTime _fromDate;
        private DateTime _toDate;

        public event PropertyChangedEventHandler PropertyChanged;

        public AppliedServicesReportPage()
        {
            InitializeComponent();
            DataContext = this;

            ReportViewTypes.ItemsSource = ReportViewTypesList;
            CurrentViewType = ReportViewTypesList.First();

            ReportSaveTypes.ItemsSource = ReportSaveTypesList;
            CurrentSaveType = ReportSaveTypesList.First();

            ToDate = DateTime.Now;
            FromDate = DateTime.Now - TimeSpan.FromDays(365);
        }

        private void PerformSaveReport(object sender, RoutedEventArgs e)
        {
            bool wasTableCollapsed = PointsGrid.Visibility == Visibility.Collapsed;
            switch (CurrentSaveType)
            {
                case "график":
                    ExportAsChart();
                    break;
                case "только таблица":
                    break;
                case "график и таблица":
                    break;
                default:
                    break;
            }
            if (wasTableCollapsed)
            {
                PointsGrid.Visibility = Visibility.Collapsed;
                ChartHost.Visibility = Visibility.Visible;
            }
            else
            {
                PointsGrid.Visibility = Visibility.Visible;
                ChartHost.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Экспортирует оказанные услуги как график в формат .pdf.
        /// </summary>
        private void ExportAsChart()
        {
            PointsGrid.Visibility = Visibility.Collapsed;
            ChartHost.Visibility = Visibility.Visible;
            LoadAsChart();
            new PrintVisualExportService(ChartHost, "Экспорт графика " +
                    "контроля качества в формате .pdf")
                .Export();
        }
    }
}
