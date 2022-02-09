using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Services;
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
                InvalidateUser();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public TimerService TimerService { get; set; } = new TimerService();

        internal void InvalidateUser()
        {
            PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(User)));
        }
    }
}
