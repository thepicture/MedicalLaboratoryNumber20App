using MedicalLaboratoryNumber20App.Models.Entities;
using System;

namespace MedicalLaboratoryNumber20App.Models.Services
{
    public static class LoginHistoryService
    {
        public static async void Write(string login, bool isSuccessful)
        {
            using (MedicalLaboratoryNumber20Entities context =
                new MedicalLaboratoryNumber20Entities())
            {
                LoginHistory history = new LoginHistory
                {
                    LoginDateTime = DateTime.Now,
                    EnteredLogin = login,
                    IsSuccessful = isSuccessful,
                };
                _ = context.LoginHistory.Add(history);
                try
                {
                    if (await context.SaveChangesAsync() > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("History was written");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    MessageBoxService.ShowError("Не удалось " +
                        "записать историю входа. Перезайдите в приложение");
                }
            }
        }
    }
}
