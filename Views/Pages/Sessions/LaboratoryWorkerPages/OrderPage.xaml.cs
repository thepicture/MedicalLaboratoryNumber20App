using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
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
        private readonly ICalculator<int, string> _levenshteinCalculator
            = new LevenshteinDistanceCalculator();

        public event PropertyChangedEventHandler PropertyChanged;

        public OrderPage(Blood blood)
        {
            InitializeComponent();
            DataContext = this;
            Blood = blood;
            BarcodeBox.Text = Blood.Barcode;
            OrderServices.ItemsSource = new List<Service>();
            _ = LoadBarcodeHintAsync();
            _ = LoadPatientsAsync();
            _ = LoadDatabaseServicesAsync();
        }

        /// <summary>
        /// Подгружает все услуги из базы данных асинхронно.
        /// </summary>
        private async Task LoadDatabaseServicesAsync()
        {
            IEnumerable<Service> databaseServices = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.Service.ToList();
                }
            });
            DatabaseServices.ItemsSource = databaseServices;
        }

        /// <summary>
        /// Загружает подсказку штрих-кода, 
        /// если соответствующее поле ввода пустое.
        /// </summary>
        private async Task LoadBarcodeHintAsync()
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

        /// Позволяет ввести посредством нажатия клавиши Enter
        /// или сохранить штрих-код.
        /// </summary>
        private async void OnBarcodeKeyDownAsync(object sender,
                                                 KeyEventArgs e)
        {
            await LoadBarcodeHintAsync();
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
                        SaveBarcodeAsync();
                    }
                    else
                    {
                        BarcodeBox.IsEnabled = false;

                        _ = await MessageBoxService
                            .ShowWarningAsync("Не удалось создать штрих-код. "
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
            Blood.Barcode = barcodeText;
            BarcodeView.ItemsSource = CurrentBarcode.Strips;
        }

        /// <summary>
        /// Сохраняет штрих-код в память текущего устройства.
        /// </summary>
        private void SaveBarcodeAsync()
        {
            new PrintVisualExportService(BarcodeView, "Укажите путь сохранения " +
                    "штрих-кода в формате .pdf")
                .Export();
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
                _ = LoadPatientsAsync()
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
        /// Загружает пациентов в выпадающий список асинхронно.
        /// </summary>
        private async Task LoadPatientsAsync()
        {
            string searchText = PatientSearchBox.Text;
            IEnumerable<Patient> patients = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    IEnumerable<Patient> currentPatients = context.Patient
                    .ToList();
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        currentPatients = currentPatients.Where(p =>
                        {
                            return _levenshteinCalculator.Calculate(p.PatientFullName.ToLower(),
                                                                 PatientSearchBox.Text.ToLower()) < 4;
                        });
                    }
                    return currentPatients;
                }
            });
            ComboPatients.ItemsSource = patients;
            ComboPatients.SelectedItem = patients
                .FirstOrDefault(p =>
                {
                    return p.PatientFullName == Blood.Patient?.PatientFullName;
                });
            if (ComboPatients.SelectedItem == null)
            {
                ComboPatients.SelectedItem = patients.FirstOrDefault();
            }
        }

        /// <summary>
        /// Удаляет услугу из заказа, но оставляет её в базе данных.
        /// </summary>
        private async void PerformDeleteServiceAsync(object sender,
                                                     RoutedEventArgs e)
        {
            Service service = (sender as Button).DataContext as Service;
            await LoadDatabaseServicesAsync();
            DatabaseServices.ItemsSource = DatabaseServices.ItemsSource
                .Cast<Service>()
                .Except
                (
                OrderServices.ItemsSource.Cast<Service>(), new ServiceEqualityComparer()
                )
                .Append(service);
            OrderServices.ItemsSource = OrderServices.ItemsSource
                .Cast<Service>()
                .Except
                (
                DatabaseServices.ItemsSource.Cast<Service>(), new ServiceEqualityComparer()
                );
        }

        /// <summary>
        /// Вызывается в момент поиска пациента по ФИО.
        /// </summary>
        private async void OnPatientSearchAsync(object sender,
                                                KeyEventArgs e)
        {
            await LoadPatientsAsync();
        }

        /// <summary>
        /// Добавляет услугу из базы данных к текущему заказу.
        /// </summary>
        private void PerformAddService(object sender, RoutedEventArgs e)
        {
            Service service = (sender as Button).DataContext as Service;
            OrderServices.ItemsSource = OrderServices.ItemsSource
                .Cast<Service>()
                .Append(service);
            DatabaseServices.ItemsSource = DatabaseServices.ItemsSource
                .Cast<Service>()
                .Except
                (
                OrderServices.ItemsSource.Cast<Service>(), new ServiceEqualityComparer()
                );
        }

        /// <summary>
        /// Вызывается при поиске услуги по названию.
        /// </summary>
        private async void OnServiceSearchAsync(object sender,
                                                KeyEventArgs e)
        {
            IEnumerable<Service> foundServices = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.Service.ToList();
                }
            });

            string searchText = ServiceSearchBox.Text;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                foundServices = await Task.Run(() =>
                {
                    return foundServices.Where(s =>
                     {
                         return _levenshteinCalculator
                         .Calculate(s.ServiceName.ToLower(),
                                    searchText.ToLower()) < 4;
                     });
                });
            }
            DatabaseServices.ItemsSource = foundServices
                       .Except(
                       OrderServices.Items.Cast<Service>(),
                       new ServiceEqualityComparer());
        }

        /// <summary>
        /// Сохраняет заказ.
        /// </summary>
        private async void PerformSaveOrderAsync(object sender,
                                                 RoutedEventArgs e)
        {
            if (ComboPatients.SelectedItem is null)
            {
                _ = await MessageBoxService
                .ShowWarningAsync("Необходимо указать клиента в выпадающем списке");
                return;
            }
            if (OrderServices.Items.Count == 0)
            {
                _ = await MessageBoxService
                    .ShowWarningAsync("Укажите хотя бы одну услугу для заказа");
                return;
            }
            if (string.IsNullOrWhiteSpace(BarcodeBox.Text)
                || !BarcodeBox.Text
                .ToCharArray()
                .All(c =>
                {
                    return char.IsDigit(c);
                }))
            {
                _ = await MessageBoxService.ShowWarningAsync("Введите штрих-код " +
                    "в виде десятичных цифр, " +
                    "так как формирование заказа без штрих-кода " +
                    "не допускается");
                return;
            }
            SaveOrderButton.IsEnabled = false;
            Blood.PatientId = (ComboPatients.SelectedItem as Patient).PatientId;
            Order order = new Order
            {
                CreationDate = DateTime.Now,
                Service = OrderServices.Items.Cast<Service>().ToList(),
                BloodId = Blood.BloodId,
            };
            bool isSaved = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    try
                    {
                        context
                        .Entry(context.Blood.Find(Blood.BloodId))
                        .CurrentValues
                        .SetValues(Blood);
                        _ = context.Order.Add(order);
                        _ = context.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        return false;
                    }
                }
            });

            if (isSaved)
            {
                await GenerateOrderReportAsync(order.OrderId);
                await GenerateTextLinkAsync(order);
                MessageBoxService.ShowInfo("Заказ успешно сформирован!");
                NavigationService.GoBack();
            }
            else
            {
                MessageBoxService.ShowError("Заказ не сформирован. " +
                    "Перезайдите на страницу " +
                    "и попробуйте ещё раз");
            }
            SaveOrderButton.IsEnabled = true;
        }

        /// <summary>
        /// Генерирует текстовое представление заказа.
        /// </summary>
        /// <param name="order">Заказ, 
        /// определяющий содержимое текстового файла.</param>
        private async Task GenerateTextLinkAsync(Order order)
        {
            System.Windows.Forms.FolderBrowserDialog linkBrowserDialog =
                new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "Укажите путь сохранения заказа " +
                "в виде текстового файла"
                };
            if (linkBrowserDialog.ShowDialog()
                == System.Windows.Forms.DialogResult.OK)
            {
                bool isLinkSaved = await Task.Run(() =>
                {
                    try
                    {
                        File.WriteAllText(Path.Combine(linkBrowserDialog.SelectedPath,
                                                       $"ИнформацияОЗаказе_" +
                                                       $"{order.CreationDate:yyyy-MM-ddThh-mm-ss}" +
                                                       $".txt"),
                                          $"https://wsrussia.ru/?data=base64(" +
                                          $"дата_заказа=" +
                                          $"{order.CreationDate:yyyy-MM-ddThh:mm:ss}" +
                                          $"&номер_заказа=" +
                                          $"{order.OrderId})");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        return false;
                    }
                });
                if (isLinkSaved)
                {
                    MessageBoxService.ShowInfo("Ссылка на заказ сохранена " +
                        $"по пути {linkBrowserDialog.SelectedPath}");
                }
                else
                {
                    MessageBoxService.ShowError("Не удалось сохранить ссылку на заказ. " +
                        "Пробуем ещё раз ...");
                    await GenerateTextLinkAsync(order);
                }
            }
        }

        /// <summary>
        /// Генерирует электронный вид заказа в формате .pdf.
        /// </summary>
        /// <param name="orderId">Идентификатор созданного заказа.</param>
        private async Task GenerateOrderReportAsync(int orderId)
        {
            Order order = await Task.Run(() =>
            {
                using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
                {
                    return context.Order
                    .Include(o => o.Service)
                    .Include(o => o.Blood)
                    .Include(o => o.Blood.Patient)
                    .FirstOrDefault(o => o.OrderId == orderId);
                }
            });
            System.Windows.Forms.FolderBrowserDialog reportBrowserDialog =
                new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "Выберите путь сохранения заказа в формате .pdf",
                };
            if (reportBrowserDialog.ShowDialog()
                == System.Windows.Forms.DialogResult.OK)
            {
                bool isReportSaved = await Task.Run(() =>
                {
                    return new OrderReportService().GenerateReport(order,
                                      reportBrowserDialog.SelectedPath);
                });

                if (isReportSaved)
                {
                    MessageBoxService.ShowInfo("Отчёт " +
                        "успешно сформирован в .pdf файл " +
                        $"по пути {reportBrowserDialog.SelectedPath}!");
                }
                else
                {
                    MessageBoxService.ShowError("Отчёт не был сформирован в .pdf. " +
                        "Пробуем ещё раз ...");
                    await GenerateOrderReportAsync(order.OrderId);
                }
            }
        }

        /// <summary>
        /// Срабатывает при навигации на предыдущую страницу.
        /// </summary>
        private void PerformGoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Открывает модальное окно редактирования существующего пациента.
        /// </summary>
        private void PerformEditPatient(object sender, RoutedEventArgs e)
        {
            Patient patient = ComboPatients.SelectedItem as Patient;
            AddPatientWindow addPatientWindow =
                           new AddPatientWindow(patient)
                           {
                               Owner = App.Current.MainWindow,
                           };
            if ((bool)addPatientWindow.ShowDialog())
            {
                MessageBoxService.ShowInfo("Пациент успешно изменен!");
                _ = LoadPatientsAsync()
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
                MessageBoxService.ShowInfo("Изменение пациента " +
                    "было отменено");
            }
        }
    }
}
