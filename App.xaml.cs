using MedicalLaboratoryNumber20App.Models.Entities;
using MedicalLaboratoryNumber20App.Services;
using System;
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
        public TimerService TimerService { get; set; } = new TimerService();
    }
}
