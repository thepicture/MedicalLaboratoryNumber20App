using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Views.Pages;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MedicalLaboratoryNumber20App
{
    /// <summary>
    /// Interaction logic for NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {
        public NavigationWindow()
        {
            InitializeComponent();

            DataContext = this;
            if (!MainFrame.Navigate(new LoginPage()))
            {
                MessageBoxService.ShowError("Не удалось запустить приложение. Перезайдите");
                App.Current.Shutdown();
            }
        }

        private void OnExitingToLoginPage(object sender, RoutedEventArgs e)
        {
            if (MessageBoxService.ShowQuestion("Действительно " +
                "выйти на главный экран - окно входа?"))
            {
                (App.Current as App).User = null;
                while (MainFrame.CanGoBack)
                {
                    MainFrame.GoBack();
                }
            }
        }

        /// <summary>
        /// Возвращает пользователя назад.
        /// </summary>
        private void PerformGoBack(object sender, RoutedEventArgs e)
        {
            MainFrame.GoBack();
        }

        /// <summary>
        /// Изменяет аватар пользователя.
        /// </summary>
        private async void PerformChangePictureAsync(object sender,
                                                     RoutedEventArgs e)
        {
            OpenFileDialog pictureFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.PNG;*.JPG;*.BMP;*.JPEG;*.GIF)" +
                "|" +
                "(*.PNG;*.JPG;*.BMP;*.JPEG;*.GIF)",
            };
            if (pictureFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                User user = (App.Current as App).User;
                bool isPictureChanged = await Task.Run(() =>
                {
                    using (MedicalLaboratoryNumber20Entities context =
                         new MedicalLaboratoryNumber20Entities())
                    {
                        context.User.Find(user.UserId).UserImage = File.ReadAllBytes(pictureFileDialog.FileName);
                        try
                        {
                            _ = context.SaveChanges();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                            return false;
                            throw;
                        }
                    }
                });

                if (!isPictureChanged)
                {
                    MessageBoxService.ShowError("Не удалось заменить изображение. " +
                        "Проверьте подключение к сети и попробуйте ещё раз");
                }
                else
                {
                    user.UserImage = File.ReadAllBytes(pictureFileDialog.FileName);
                    (App.Current as App).InvalidateUser();
                    MessageBoxService.ShowInfo("Изображение изменено");
                }
            }
        }
    }
}
