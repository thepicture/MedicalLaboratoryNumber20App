using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        private void FilterView()
        {
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
    }
}
