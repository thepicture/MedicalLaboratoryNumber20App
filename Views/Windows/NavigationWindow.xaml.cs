using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Views.Pages;
using System.Windows;

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
                while (MainFrame.CanGoBack)
                {
                    MainFrame.GoBack();
                }
            }
        }
    }
}
