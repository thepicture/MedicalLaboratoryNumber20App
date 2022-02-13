using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
                    LoadAsTable();
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
        private void LoadAsTable()
        {
            throw new NotImplementedException();
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
    }
}
