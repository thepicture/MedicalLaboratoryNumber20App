using MedicalLaboratoryNumber20App.Models;
using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Views.Pages.AccountantPages;
using MedicalLaboratoryNumber20App.Views.Pages.AdminPages;
using MedicalLaboratoryNumber20App.Views.Pages.Sessions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MedicalLaboratoryNumber20App.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private bool _isFirstTimeWrongPassword = true;
        private readonly CaptchaService _captchaService = new CaptchaService();
        private readonly Random random = new Random();

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
            if (CaptchaPanel.Visibility == Visibility.Visible
                && !_captchaService.Check(Captcha.Text))
            {
                MessageBoxService.ShowWarning("Вы ввели неверную captcha. " +
                    "Попробуйте ещё раз");
                RequireCaptcha();
                return;
            }
            string userLogin = Login.Text;
            string userPassword = PasswordHidden.Password;
            if (string.IsNullOrEmpty(userLogin)
                || string.IsNullOrEmpty(userPassword))
            {
                MessageBoxService.ShowWarning("Заполните поля и логина, и пароля");
                return;
            }
            BtnLogin.IsEnabled = false;
            BtnLogin.Content = "Авторизация...";
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
                    if (_isFirstTimeWrongPassword)
                    {
                        MessageBoxService.ShowWarning("Неуспешная авторизация. " +
                            "Неверный логин или пароль");
                        RequireCaptcha();
                        _isFirstTimeWrongPassword = false;
                    }
                    else
                    {
                        TimeSpan timeSpan = TimeSpan.FromSeconds(10);
                        MessageBoxService.ShowWarning($"Система заблокирована " +
                            $"на {timeSpan.TotalSeconds:N2} секунд");
                        RequireCaptcha();
                        BlockIntefaceFor(timeSpan);
                    }
                }
                else
                {
                    (App.Current as App).User = user;
                    MessageBoxService
                        .ShowInfo($"Вы авторизованы, {user.UserName}");
                    CaptchaPanel.Visibility = Visibility.Collapsed;
                    switch (user.UserType.UserTypeName)
                    {
                        case "Лаборант":
                            _ = NavigationService
                                .Navigate(new LaboratoryWorkerPage());
                            break;
                        case "Лаборант-исследователь":
                            _ = NavigationService
                                .Navigate(new LaboratoryResearcherPage());
                            break;
                        case "Бухгалтер":
                            _ = NavigationService
                                .Navigate(new AccountantPage());
                            break;
                        case "Администратор":
                            _ = NavigationService
                                .Navigate(new AdminPage());
                            break;
                        default:
                            System.Diagnostics.Debug
                                .WriteLine("No user page was found");
                            MessageBoxService.ShowError("Не удалось " +
                                "найти страницу для перехода. " +
                                "Обратитесь к системному администратору");
                            break;
                    }
                }
                LoginHistoryService.Write(Login.Text, user != null);
            }
        }

        /// <summary>
        /// Просит ввести captcha.
        /// </summary>
        private void RequireCaptcha()
        {
            CaptchaPanel.Visibility = Visibility.Visible;
            string captcha = _captchaService
                .Generate()
                .Get();
            IEnumerable<CaptchaLetter> captchaLetters = captcha
                .ToCharArray()
                .Select(c =>
                {
                    int marginDensity = random.Next(-20, 20);
                    Thickness marginThickness =
                    new Thickness(0, marginDensity, 0, 0);
                    return new CaptchaLetter(c, marginThickness);
                });
            CaptchaList.ItemsSource = captchaLetters;
        }

        /// <summary>
        /// Блокирует возможность входа на заданное время.
        /// </summary>
        /// <param name="timeSpan">Заданное время.</param>
        private void BlockIntefaceFor(TimeSpan timeSpan)
        {
            IsEnabled = false;
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = timeSpan
            };
            timer.Tick += OnUnlockInterface;
            timer.Start();
        }

        /// <summary>
        /// Выполняется при разблокировке интерфейса.
        /// </summary>
        private void OnUnlockInterface(object sender, EventArgs e)
        {
            IsEnabled = true;
            MessageBoxService.ShowInfo("Система разблокирована");
            (sender as DispatcherTimer).Stop();
            RequireCaptcha();
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

        private void OnCaptchaRequire(object sender, RoutedEventArgs e)
        {
            RequireCaptcha();
        }
    }
}
