﻿using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryWorkerPages
{
    /// <summary>
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page, INotifyPropertyChanged
    {
        private const int MinimumBarcodeNumber = 100000;
        private const int MaximumBarcodeNumber = 999999 + 1;
        private readonly Random random = new Random();
        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Service> _services;

        public OrderPage(Blood blood)
        {
            InitializeComponent();
            DataContext = this;
            Blood = blood;
            LoadBarcodeHint();
            LoadPatients();
            CurrentServices = new ObservableCollection<Service>();
            Services.ItemsSource = CurrentServices;
        }

        /// <summary>
        /// Загружает подсказку штрих-кода, 
        /// если соответствующее поле ввода пустое.
        /// </summary>
        private async void LoadBarcodeHint()
        {
            if (!string.IsNullOrWhiteSpace(BarcodeBox.Text))
            {
                BarcodeHint.Text = string.Empty;
                return;
            }
            string lastOrderId = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                   new MedicalLaboratoryNumber20Entities())
                {
                    if (context.Order.Count() == 0)
                    {
                        return "1";
                    }
                    else
                    {
                        int lastOrder = context.Order.Max(o => o.OrderId);
                        return lastOrder.ToString();
                    }
                }
            });
            BarcodeHint.Text = lastOrderId;
        }

        public Blood Blood { get; }
        public Barcode CurrentBarcode { get; private set; }
        public bool IsBusy
        {
            get => _isBusy; set
            {
                _isBusy = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(IsBusy)));
            }
        }

        public ObservableCollection<Service> CurrentServices
        {
            get => _services;
            set
            {
                _services = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(CurrentServices)));
            }
        }

        /// <summary>
        /// Позволяет ввести посредством нажатия клавиши Enter
        /// или сохранить штрих-код.
        /// </summary>
        private async void OnBarcodeKeyDown(object sender, KeyEventArgs e)
        {
            LoadBarcodeHint();
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(BarcodeBox.Text))
                {
                    BarcodeBox.Text = BarcodeHint.Text;
                }
                else
                {
                    if (BarcodeBox.Text
                        .ToCharArray()
                        .ToList()
                        .All(c =>
                        {
                            return int
                            .TryParse(c.ToString(),
                                      out _);
                        }))
                    {
                        GenerateBarcode();
                        SaveBarcode();
                    }
                    else
                    {
                        BarcodeBox.IsEnabled = false;

                        _ = await MessageBoxService
                            .ShowWarning("Не удалось создать штрих-код. "
                                         + "Необходимо ввести только "
                                         + "десятичные цифры");
                        BarcodeBox.IsEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Генерирует штрих-код.
        /// </summary>
        private void GenerateBarcode()
        {
            string barcodeText = BarcodeBox.Text
                                 + DateTime.Now.ToString("yyyyMMdd")
                                 + random.Next(MinimumBarcodeNumber,
                                               MaximumBarcodeNumber);
            CurrentBarcode = BarcodeService.NewBarcode(barcodeText);
            BarcodeView.ItemsSource = CurrentBarcode.Strips;
        }

        /// <summary>
        /// Сохраняет штрих-код в память текущего устройства.
        /// </summary>
        private async void SaveBarcode()
        {
            byte[] barcodeBytes = null;
            Dispatcher.Invoke(() =>
            {
                barcodeBytes = ControlImageService.ConvertToPng(BarcodeView);
            }, DispatcherPriority.Loaded);
            System.Windows.Forms.FolderBrowserDialog dialog =
                new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                _ = await MessageBoxService
                    .ShowWarning("Выбор пути сохранения был отменён");
                return;
            }
            IsBusy = true;
            ByteArrayToPdfExportService exporter =
                new ByteArrayToPdfExportService(barcodeBytes,
                                                dialog.SelectedPath);
            await Task.Run(() => exporter.Export());
            MessageBoxService.ShowInfo("Штрих-код сохранён " +
                $"по пути {dialog.SelectedPath}");
            IsBusy = false;
        }

        /// <summary>
        /// Сканирует штрих-код со сканера.
        /// </summary>
        private void PerformBarcodeScan(object sender, RoutedEventArgs e)
        {
            BarcodeScannerService barcodeScanner = new BarcodeScannerService();
            string scannedBarCode =
                BarcodeBox.Text =
                barcodeScanner
                .Scan()
                .Replace("\r", "");
            BarcodeHint.Text = string.Empty;
            CurrentBarcode = BarcodeService.NewBarcode(scannedBarCode);
            BarcodeView.ItemsSource = CurrentBarcode.Strips;
        }

        /// <summary>
        /// Открывает модальное окно добавления пациента.
        /// </summary>
        private void PerformOpenAddPatientModalWindow(object sender,
                                                      RoutedEventArgs e)
        {
            AddPatientWindow addPatientWindow =
                new AddPatientWindow()
                {
                    Owner = App.Current.MainWindow,
                };
            if ((bool)addPatientWindow.ShowDialog())
            {
                MessageBoxService.ShowInfo("Пациент успешно добавлен!");
                _ = LoadPatients()
                    .ContinueWith(t =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ComboPatients.SelectedItem = ComboPatients.Items
                           .Cast<Patient>()
                           .First(p =>
                           {
                               return p.PatientId == addPatientWindow.Patient.PatientId;
                           });
                        });
                    });
            }
            else
            {
                MessageBoxService.ShowInfo("Добавление нового пациента " +
                    "было отменено");
            }
        }

        /// <summary>
        /// Загружает пациентов в выпадающий список.
        /// </summary>
        private async Task LoadPatients()
        {
            IEnumerable<Patient> patients = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.Patient.ToList();
                }
            });
            ComboPatients.ItemsSource = patients;
            ComboPatients.SelectedItem = patients.First();
        }

        /// <summary>
        /// Добавляет новую услугу в заказ.
        /// </summary>
        private void PerformAddNewService(object sender, RoutedEventArgs e)
        {
            CurrentServices.Add(new Service());
        }

        /// <summary>
        /// Удаляет услугу из заказа, оставляя услугу в базе данных.
        /// </summary>
        private void PerformDeleteService(object sender, RoutedEventArgs e)
        {
            Service service = (sender as Button).DataContext as Service;
            _ = CurrentServices.Remove(service);
        }

        /// <summary>
        /// Вызывается в момент изменения названия услуги.
        /// </summary>
        private async void OnServiceTitleChanged(object sender, KeyEventArgs e)
        {
            NonExistingServicesMessage.Visibility = Visibility.Collapsed;
            foreach (Service service in Services.Items.Cast<Service>())
            {
                bool isServiceExists = await Task.Run(() =>
                {
                    using (MedicalLaboratoryNumber20Entities context =
                    new MedicalLaboratoryNumber20Entities())
                    {
                        return context.Service
                        .Any(s => s.ServiceName
                        .ToLower() == service.ServiceName
                        .ToLower());
                    }
                });
                if (!isServiceExists)
                {
                    NonExistingServicesMessage.Visibility = Visibility.Visible;
                    return;
                }
            }
        }
    }
}
