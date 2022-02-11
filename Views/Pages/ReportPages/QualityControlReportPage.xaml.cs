using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private void FilterView()
        {
            ControlReportSeries.Points.AddXY("2020-01-01 00:00:00", 3);
            ControlReportSeries.Points.AddXY("2020-01-02 00:00:00", 4);
            ControlReportSeries.Points.AddXY("2020-01-03 00:00:00", 1);
            ControlReportSeries.Points.AddXY("2020-01-04 00:00:00", 2.5);
            ControlReportSeries.Points.AddXY("2020-01-05 00:00:00", 6);
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

        private string _currentSaveType;
        private string _currentViewType;
    }
}
