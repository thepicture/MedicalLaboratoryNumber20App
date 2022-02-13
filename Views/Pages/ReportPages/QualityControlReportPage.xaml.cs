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
    /// Interaction logic for QualityControlReportPage.xaml
    /// </summary>
    public partial class QualityControlReportPage : Page, INotifyPropertyChanged
    {
        private double _variationCoefficient;
        private double _meanDeviation;
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

        public double MeanValue { get; private set; }

        public double MeanQuadraticDeviation
        {
            get => _meanDeviation; set
            {
                _meanDeviation = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(MeanQuadraticDeviation)));
            }
        }
        public double VariationCoefficient
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
            if (BloodServices.Count == 0)
            {
                return;
            }

            if (BloodServices.Where(bs =>
            {
                return decimal.TryParse(bs.Result, out _);
            })
                .Count() == 0)
            {
                return;
            }
            MeanValue = BloodServices
                .Where(bs => double.TryParse(bs.Result, out _))
                .Average(bs => double.Parse(bs.Result));

            MeanQuadraticDeviation = Convert.ToDouble(GetMeanQuadraticDeviation(BloodServices));
            VariationCoefficient = MeanQuadraticDeviation / MeanValue * 100;
            ChartHost.Visibility = Visibility.Collapsed;
            PointsGrid.Visibility = Visibility.Collapsed;
            switch (CurrentViewType)
            {
                case "графиком":
                    ChartHost.Visibility = Visibility.Visible;
                    LoadAsChart();
                    break;
                case "таблицей":
                    PointsGrid.Visibility = Visibility.Visible;
                    LoadAsTable();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Выводит данные в виде таблицы.
        /// </summary>
        private void LoadAsTable()
        {
            IList<QualityControlPointPair> points = new List<QualityControlPointPair>();
            foreach (BloodServiceOfUser bloodService in BloodServices)
            {
                if (decimal.TryParse(bloodService.Result, out decimal result))
                {
                    QualityControlPointPair pointPair =
                        new QualityControlPointPair(bloodService.FinishedDateTime
                        .ToString("yyyy-MM-dd hh:mm:ss"), result);
                    points.Add(pointPair);
                }
            }
            PointsGrid.ItemsSource = points;
        }

        /// <summary>
        /// Выводит данные в виде графика.
        /// </summary>
        private void LoadAsChart()
        {
            ControlReportSeries.Points.Clear();
            ControlReportSeries.LegendText = CurrentService.ServiceName;

            Negative3s.FromPosition =
                           Negative3sValue.FromPosition = MeanValue - (MeanQuadraticDeviation * 3);
            Negative3s.ToPosition =
             Negative3sValue.ToPosition = MeanValue - (MeanQuadraticDeviation * 3) + 0.01;
            Negative3sValue.Text = (MeanValue - (MeanQuadraticDeviation * 3)).ToString("N2");
            Negative2s.FromPosition =
            Negative2sValue.FromPosition = MeanValue - (MeanQuadraticDeviation * 2);
            Negative2s.ToPosition =
             Negative2sValue.ToPosition = MeanValue - (MeanQuadraticDeviation * 2) + 0.01;
            Negative2sValue.Text = (MeanValue - (MeanQuadraticDeviation * 2)).ToString("N2");

            Negative1s.FromPosition =
            Negative1sValue.FromPosition = MeanValue - (MeanQuadraticDeviation * 1);
            Negative1s.ToPosition =
             Negative1sValue.ToPosition = MeanValue - (MeanQuadraticDeviation * 1) + 0.01;
            Negative1sValue.Text = (MeanValue - (MeanQuadraticDeviation * 1)).ToString("N2");

            Positive3s.FromPosition =
              Positive3sValue.FromPosition = MeanValue + (MeanQuadraticDeviation * 3);
            Positive3s.ToPosition =
             Positive3sValue.ToPosition = MeanValue + (MeanQuadraticDeviation * 3) + 0.01;
            Positive3sValue.Text = (MeanValue + (MeanQuadraticDeviation * 3)).ToString("N2");
            Positive2s.FromPosition =
            Positive2sValue.FromPosition = MeanValue + (MeanQuadraticDeviation * 2);
            Positive2s.ToPosition =
             Positive2sValue.ToPosition = MeanValue + (MeanQuadraticDeviation * 2) + 0.01;
            Positive2sValue.Text = (MeanValue + (MeanQuadraticDeviation * 2)).ToString("N2");

            Positive1s.FromPosition =
            Positive1sValue.FromPosition = MeanValue + (MeanQuadraticDeviation * 1);
            Positive1s.ToPosition =
             Positive1sValue.ToPosition = MeanValue + (MeanQuadraticDeviation * 1) + 0.01;
            Positive1sValue.Text = (MeanValue + (MeanQuadraticDeviation * 1)).ToString("N2");
            MeanDeviationCenter.FromPosition =
                MeanDeviationCenterValue.FromPosition =
                BloodServices
                .Where(bs => double.TryParse(bs.Result, out _))
                .Average(bs => double.Parse(double.Parse(bs.Result).ToString("N2")));
            MeanDeviationCenter.ToPosition =
             MeanDeviationCenterValue.ToPosition =
             BloodServices
             .Where(bs => double.TryParse(bs.Result, out _))
             .Average(bs => double.Parse(double.Parse(bs.Result).ToString("N2")) + 0.01);
            MeanDeviationCenterValue.Text = MeanValue.ToString("N2");
            foreach (BloodServiceOfUser bloodService in BloodServices)
            {
                if (decimal.TryParse(bloodService.Result, out decimal result))
                {
                    _ = ControlReportSeries.Points.AddXY(bloodService.FinishedDateTime
                        .ToString("yyyy-MM-dd hh:mm:ss"),
                                                     result.ToString("N2"));
                }
            }
        }

        /// <summary>
        /// Высчитывает среднеквадратичное отклонеие.
        /// </summary>
        /// <param name="bloodServices">Оказанные услуги.</param>
        private double GetMeanQuadraticDeviation(List<BloodServiceOfUser> bloodServices)
        {
            double currentSum = 0;
            foreach (BloodServiceOfUser service in bloodServices)
            {
                if (double.TryParse(service.Result, out double value))
                {
                    currentSum += Math.Pow(MeanValue - value, 2);
                }
            }
            double result = currentSum / bloodServices.Count();
            return Math.Sqrt(result);
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

        /// <summary>
        /// Сохраняет отчёт контроля качества.
        /// </summary>
        private void PerformReportSave(object sender, RoutedEventArgs e)
        {
            bool wasTableCollapsed = PointsGrid.Visibility == Visibility.Collapsed;
            switch (CurrentSaveType)
            {
                case "график":
                    SaveAsChart();
                    break;
                case "только таблица":
                    SaveAsTable();
                    break;
                case "график и таблица":
                    SaveAsTable();
                    SaveAsChart();
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
        /// Экспортирует график контроля качества в формате .pdf.
        /// </summary>
        private void SaveAsChart()
        {
            PointsGrid.Visibility = Visibility.Collapsed;
            ChartHost.Visibility = Visibility.Visible;
            LoadAsChart();
            new PrintVisualExportService(ChartHost, "Экспорт графика " +
                    "контроля качества в формате .pdf")
                .Export();
        }

        /// <summary>
        /// Экспортирует таблицу контроля качества в формате .pdf.
        /// </summary>
        private void SaveAsTable()
        {
            PointsGrid.Visibility = Visibility.Visible;
            ChartHost.Visibility = Visibility.Collapsed;
            LoadAsTable();
            new PrintVisualExportService(PointsGrid, "Экспорт таблицы" +
                 "контроля качества в формате .pdf")
             .Export();
        }
    }
}
