using MedicalLaboratoryNumber20App.Models.Services;
using System;
using System.ComponentModel;
using System.Windows.Controls;
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
        private Page _page;
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
        }

        /// <summary>
        /// Начнает сессию для заданной страницы.
        /// </summary>
        /// <param name="page"></param>
        internal void StartForPage(Page page)
        {
            _page = page;

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
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
            if (TimeLeft == _messageAppearTime)
            {
                MessageBoxService.ShowInfo("Сеанс завершится через " +
                    $"{_messageAppearTime.TotalMinutes:N2} минут");
            }
            else if (TimeSpan.Zero == TimeLeft)
            {
                timer.Stop();
                while (_page.NavigationService.CanGoBack)
                {
                    _page.NavigationService.GoBack();
                }
                BlockInterface();
                MessageBoxService.ShowInfo("Сеанс завершен. " +
                     $"Вход заблокирован на {_banTime.TotalMinutes:N2} минут");
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
