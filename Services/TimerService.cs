﻿using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Views.Pages;
using System;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Реализует метод для работы с сеансом.
    /// </summary>
    public class TimerService : INotifyPropertyChanged
    {
        private TimeSpan _messageAppearTime;
        private TimeSpan _banTime;
        private readonly NavigationService _navigationService;
        private TimeSpan timeLeft;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan TimeLeft
        {
            get => timeLeft; private set
            {
                timeLeft = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeLeft)));
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр таймера.
        /// </summary>
        /// <param name="sessionTime">Длительность сессии.</param>
        /// <param name="messageAppearTime">Время появления сообщения.</param>
        /// <param name="banTime">Время блокировки.</param>
        public TimerService(TimeSpan sessionTime,
                            TimeSpan messageAppearTime,
                            TimeSpan banTime)
        {
            TimeLeft = sessionTime;
            _messageAppearTime = messageAppearTime;
            _banTime = banTime;
            _navigationService = (App.Current.MainWindow as NavigationWindow)
                .MainFrame.NavigationService;
        }

        /// <summary>
        /// Начнает сессию для заданной страницы.
        /// </summary>
        /// <param name="page"></param>
        internal void Start()
        {
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            timer.Tick += OnTick;
            timer.Start();
        }

        /// <summary>
        /// Вызывается каждую секунду таймером.
        /// </summary>
        private void OnTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;
            if (_navigationService.Content is LoginPage)
            {
                timer.Stop();
                return;
            }
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
            if (TimeLeft == _messageAppearTime)
            {
                MessageBoxService.ShowInfo("Сеанс завершится через столько минут: " +
                    $"{_messageAppearTime.TotalMinutes:N0}");
            }
            else if (TimeSpan.Zero == TimeLeft)
            {
                timer.Stop();
                while (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }
                BlockInterface();
                MessageBoxService.ShowInfo("Сеанс завершен. " +
                     "Вход заблокирован на столько минут: " +
                     $"{_banTime.TotalMinutes:N0}");
            }
        }

        /// <summary>
        /// Блокирует интерфейс приложения.
        /// </summary>
        private void BlockInterface()
        {
            App.Current.MainWindow.IsEnabled = false;
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = _banTime,
            };
            timer.Tick += OnUnblock;
            timer.Start();
        }

        /// <summary>
        /// Разблокирует интерфейс приложения.
        /// </summary>
        private void OnUnblock(object sender, EventArgs e)
        {
            App.Current.MainWindow.IsEnabled = true;
            (sender as DispatcherTimer).Stop();
        }
    }
}
