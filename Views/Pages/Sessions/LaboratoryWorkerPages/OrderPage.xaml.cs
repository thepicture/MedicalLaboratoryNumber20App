using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
