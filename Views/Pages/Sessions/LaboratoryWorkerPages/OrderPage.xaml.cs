using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Services;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

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

        public OrderPage(Blood blood)
        {
            InitializeComponent();
            Blood = blood;
            LoadBarcodeHint();
            DataContext = this;
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

            using (MedicalLaboratoryNumber20Entities context =
               new MedicalLaboratoryNumber20Entities())
            {
                if (await context.Order.CountAsync() == 0)
                {
                    BarcodeHint.Text = "1";
                }
                else
                {
                    int lastOrder = await context.Order.MaxAsync(o => o.OrderId);
                    BarcodeHint.Text = lastOrder.ToString();
                }
            }
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
                        GenerateAndSaveBarcode();
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
        private void GenerateAndSaveBarcode()
        {
            string barcodeText = BarcodeBox.Text
                                 + DateTime.Now.ToString("yyyyMMdd")
                                 + random.Next(MinimumBarcodeNumber,
                                               MaximumBarcodeNumber);
            CurrentBarcode = BarcodeService.NewBarcode(barcodeText);
            BarcodeView.ItemsSource = CurrentBarcode.Strips;
            byte[] barcodeBytes = null;
            Dispatcher.Invoke(() =>
            {
                barcodeBytes = ControlImageService.ConvertToPng(BarcodeView);
            }, System.Windows.Threading.DispatcherPriority.Loaded);
            SaveBarcode(barcodeBytes);
        }

        private void SaveBarcode(byte[] barcodeBytes)
        {
            IsBusy = true;
            if (new ByteArrayToPdfExportService(barcodeBytes)
                .TryExport(out string filePath))
            {
                MessageBoxService.ShowInfo("Штрих-код сохранён " +
                    $"по пути {filePath}");
            }
            else
            {
                MessageBoxService.ShowError("Не удалось сохранить штрих-код. " +
                    "Вероятно, выбор пути сохранения был отменён");
            }
            IsBusy = false;
        }
    }
}
