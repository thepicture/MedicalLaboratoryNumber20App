﻿using MedicalLaboratoryNumber20App.Services;
using MedicalLaboratoryNumber20App.Views.Pages.Sessions.LaboratoryWorkerPages;
using System;
using System.Windows.Controls;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions
{
    /// <summary>
    /// Interaction logic for LaboratoryWorkerPage.xaml
    /// </summary>
    public partial class LaboratoryWorkerPage : Page
    {
        public TimerService TimerService { get; }

        public LaboratoryWorkerPage()
        {
            InitializeComponent();
            TimerService = new TimerService(TimeSpan.FromSeconds(10),
                                             TimeSpan.FromSeconds(5),
                                             TimeSpan.FromSeconds(1));
            TimerService.StartForPage(this);
            DataContext = this;
        }

        private void NavigateToBiomaterials(object sender, System.Windows.RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(new BiomaterialsPage());
        }
    }
}
