using System.Windows;

namespace MedicalLaboratoryNumber20App.Models
{
    /// <summary>
    /// Представляет собой символ captcha.
    /// </summary>
    public class CaptchaLetter
    {
        public char Character { get; }
        public Thickness LetterMargin { get; }

        public CaptchaLetter(char character, Thickness letterMargin)
        {
            Character = character;
            LetterMargin = letterMargin;
        }
    }
}
