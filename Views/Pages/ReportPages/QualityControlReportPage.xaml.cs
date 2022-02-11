using MedicalLaboratoryNumber20App.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.ReportPages
{
    /// <summary>
    /// Interaction logic for QualityControlReportPage.xaml
    /// </summary>
    public partial class QualityControlReportPage : Page, INotifyPropertyChanged
    {
        private float _variationCoefficient;
        private float _meanDeviation;
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
        public QualityControlReportPage()
        {
            InitializeComponent();
            DataContext = this;

            ReportViewTypes.ItemsSource = ReportViewTypesList;
            CurrentViewType = ReportViewTypesList.First();

            ReportSaveTypes.ItemsSource = ReportSaveTypesList;
            CurrentSaveType = ReportSaveTypesList.First();

        }

        /// <summary>
        /// Подгружает список услуг асинхронно.
        /// </summary>
        private async Task LoadServicesAsync()
        {
            Services = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.Service.ToList();
                }
            });
            CurrentService = Services.First();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public float MeanDeviation
        {
            get => _meanDeviation; set
            {
                _meanDeviation = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(MeanDeviation)));
            }
        }
        public float VariationCoefficient
        {
            get => _variationCoefficient;
            set
            {
                _variationCoefficient = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(VariationCoefficient)));
            }
        }

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

        /// <summary>
        /// Отображает график/таблицу в соответствии с выбранным типом.
        /// </summary>
        private async void FilterView()
        {
            if (Services == null)
            {
                await LoadServicesAsync();
            }
            BloodServices = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.BloodServiceOfUser
                    .Include(bs => bs.Service)
                    .Where(bs => bs.ServiceCode == CurrentService.Code)
                    .ToList();
                }
            });
            ControlReportSeries.Points.Clear();
            ControlReportSeries.LegendText = CurrentService.ServiceName;
            if (BloodServices.Count == 0)
            {
                return;
            }

            foreach (BloodServiceOfUser bloodService in BloodServices)
            {
                if (decimal.TryParse(bloodService.Result, out decimal result))
                {
                    ControlReportSeries.Points.AddXY(bloodService.FinishedDateTime
                        .ToString("yyyy-MM-dd hh:mm:ss"),
                                                     result.ToString("N2"));
                }
            }
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

        public Service CurrentService
        {
            get => _currentService;
            set
            {
                _currentService = value;
                PropertyChanged?.Invoke(this,
                                     new PropertyChangedEventArgs(nameof(CurrentService)));
                FilterView();
            }
        }
        public List<Service> Services
        {
            get => _services;
            set
            {
                _services = value;
                PropertyChanged?.Invoke(this,
                                     new PropertyChangedEventArgs(nameof(Services)));
            }
        }

        public List<BloodServiceOfUser> BloodServices
        {
            get => _bloodServices;
            set
            {
                _bloodServices = value;
                PropertyChanged?.Invoke(this,
                                     new PropertyChangedEventArgs(nameof(BloodServices)));
            }
        }

        private string _currentSaveType;
        private string _currentViewType;
        private List<Service> _services;
        private Service _currentService;
        private List<BloodServiceOfUser> _bloodServices;
    }
}
