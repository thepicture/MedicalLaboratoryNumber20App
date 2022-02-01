using MedicalLaboratoryNumber20App.Models.Services;
using MedicalLaboratoryNumber20App.Views.Pages;
using System;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Реализует методы для работы с сеансом.
    /// </summary>
    public class TimerService : INotifyPropertyChanged
    {
        private TimeSpan _messageAppearTime;
        private TimeSpan _banTime;
        private TimeSpan _timeLeft;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan TimeLeft
        {
            get => _timeLeft; private set
            {
                _timeLeft = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(TimeLeft)));
            }
        }

        /// <summary>
        /// Уставливает временные параметры для сессии.
        /// </summary>
        /// <param name="sessionTime">Длительность сессии.</param>
        /// <param name="messageAppearTime">Время появления сообщения.</param>
        /// <param name="banTime">Время блокировки.</param>
        /// <returns>Экземпляр текущего класса <see cref="TimerService"/>.</returns>
        public TimerService SetTime(TimeSpan sessionTime,
                            TimeSpan messageAppearTime,
                            TimeSpan banTime)
        {
            TimeLeft = sessionTime;
            _messageAppearTime = messageAppearTime;
            _banTime = banTime;
            return this;
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
        /// Вызывается каждую секунду таймером для отсчёта времени сессии.
        /// </summary>
        private void OnTick(object sender, EventArgs e)
        {
            NavigationService navigationService =
                ((NavigationWindow)App.Current.MainWindow)
                .MainFrame.NavigationService;
            DispatcherTimer timer = sender as DispatcherTimer;
            if (navigationService.Content is LoginPage)
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
                while (navigationService.CanGoBack)
                {
                    navigationService.GoBack();
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
