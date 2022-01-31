using MedicalLaboratoryNumber20App.Models.Entities;
using System.ComponentModel;
using System.Windows;

namespace MedicalLaboratoryNumber20App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        private User user;

        public User User
        {
            get => user; set
            {
                user = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(User)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
