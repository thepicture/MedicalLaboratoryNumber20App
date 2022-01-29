using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// Осуществляет авторизацию.
        /// </summary>
        private async void PerformLogin(object sender, RoutedEventArgs e)
        {
            BtnLogin.IsEnabled = false;
            BtnLogin.Content = "Авторизация...";
            string userLogin = Login.Text;
            string userPassword = PasswordHidden.Password;
            using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
            {
                User user = await context.User
                    .FirstOrDefaultAsync(u => u.UserLogin == userLogin
                                         && u.UserPassword == userPassword);
                BtnLogin.IsEnabled = true;
                BtnLogin.Content = "Войти";
                if (user == null)
                {
                    MessageBoxService.ShowWarning("Неверный логин или пароль");
                }
                else
                {
                    MessageBoxService.ShowInfo($"Вы авторизованы, {user.UserName}");
                }
                LoginHistoryService.Write(Login.Text, user != null);
            }
        }

        /// <summary>
        /// Осуществляет выход из приложения.
        /// </summary>
        private void PerformExit(object sender, RoutedEventArgs e)
        {
            if (MessageBoxService.ShowQuestion("Действительно выйти?"))
            {
                App.Current.Shutdown();
            }
        }

        /// <summary>
        /// Показывает вводимый пароль.
        /// </summary>
        private void PerformVisiblePassword(object sender, RoutedEventArgs e)
        {
            PasswordVisible.Text = PasswordHidden.Password;
            PasswordHidden.Visibility = Visibility.Collapsed;
            PasswordVisible.Visibility = Visibility.Visible;
            BtnLogin.IsEnabled = false;
        }

        /// <summary>
        /// Прячет вводимый пароль под маской ввода.
        /// </summary>
        private void PerformHiddenPassword(object sender, RoutedEventArgs e)
        {
            PasswordHidden.Password = PasswordVisible.Text;
            PasswordVisible.Visibility = Visibility.Collapsed;
            PasswordHidden.Visibility = Visibility.Visible;
            BtnLogin.IsEnabled = true;
        }
    }
}
