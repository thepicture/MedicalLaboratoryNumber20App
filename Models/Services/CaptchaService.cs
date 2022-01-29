using System;

namespace MedicalLaboratoryNumber20App.Models.Services
{
    /// <summary>
    /// Реализует методы для работы с captcha.
    /// </summary>
    public class CaptchaService
    {
        private string _currentCaptcha;

        /// <summary>
        /// Генерирует captcha.
        /// </summary>
        /// <returns>Экземпляр текущего класса.</returns>
        public CaptchaService Generate()
        {
            _currentCaptcha = Guid
                .NewGuid()
                .ToString()
                .Substring(0, 4);
            return this;
        }

        /// <summary>
        /// Получает captcha.
        /// </summary>
        /// <returns>Captcha.</returns>
        public string Get()
        {
            return _currentCaptcha;
        }

        /// <summary>
        /// Проверяет введённую captcha на действительность.
        /// </summary>
        /// <param name="captcha">Введённая captcha.</param>
        /// <returns>true, если captcha совпала 
        /// с captcha экземпляра текущего класса, 
        /// false в противном случае.</returns>
        public bool Check(string captcha)
        {
            return _currentCaptcha.ToLower() == captcha.ToLower();
        }
    }
}
