using MedicalLaboratoryNumber20App.Views.Pages;
using System.Windows;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App
{
    /// <summary>
    /// Interaction logic for NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {
        public string CurrentTitle => (MainFrame.Content as Page)?.Title;
        public NavigationWindow()
        {
            InitializeComponent();

            DataContext = this;

            if (!MainFrame.Navigate(new LoginPage()))
            {
                MessageBox.Show("Не удалось запустить приложение");
            }
        }
    }
}
