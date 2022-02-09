using MedicalLaboratoryNumber20App.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace MedicalLaboratoryNumber20App.Services
{
    public class InsuranceCompanyReportService :
        IReportService<IEnumerable<InsuranceCompany>>
    {
        private readonly DateTime? _from;
        private readonly DateTime? _to;

        public InsuranceCompanyReportService(DateTime? from,
                                             DateTime? to)
        {
            _from = from;
            _to = to;
        }

        public bool GenerateReport(IEnumerable<InsuranceCompany> obj,
                                   string path)
        {
            Excel.Application application = null;
            Excel.Workbook workbook = null;
            try
            {
                application = new Excel.Application();

                workbook = application.Workbooks.Add();
                Excel.Worksheet sheet = workbook.Sheets.Add();

                int rowCounter = 1;
                decimal totalSum = 0;
                Excel.Range range;
                for (int i = 0; i < obj.Count(); i++)
                {
                    Excel.Range insuranceCompanyRangeStart = sheet.Cells[1][rowCounter];
                    range = sheet.Range[sheet.Cells[1][rowCounter], sheet.Cells[2][rowCounter]];
                    range.Merge();
                    range.Value = "Отчёт на компанию " + obj.ElementAt(i).InsuranceName;
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowCounter++;
                    sheet.Cells[1][rowCounter] = "Название страховой компании";
                    sheet.Cells[2][rowCounter] = obj.ElementAt(i).InsuranceName;
                    rowCounter++;
                    sheet.Cells[1][rowCounter] = "Период для оплаты";
                    sheet.Cells[2][rowCounter] = $"С {_from:yyyy/MM/dd} "
                                                 + $"по {_to:yyyy/MM/dd}";
                    rowCounter++;
                    range = sheet.Range[sheet.Cells[1][rowCounter], sheet.Cells[2][rowCounter]];
                    range.Merge();
                    range.Value = "ФИО пациентов с оказанными им услугами ";
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowCounter++;
                    foreach (Patient patient in obj.ElementAt(i).Patient)
                    {
                        range = sheet.Range[sheet.Cells[1][rowCounter], sheet.Cells[2][rowCounter]];
                        range.Merge();
                        range.Value = $"Пациент {patient.PatientFullName}";
                        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        rowCounter++;

                        decimal currentPatientSum = 0;

                        foreach (Blood blood in patient.Blood)
                        {
                            foreach (BloodServiceOfUser bloodService in blood.BloodServiceOfUser)
                            {
                                sheet.Cells[1][rowCounter] = bloodService.Service.ServiceName;
                                sheet.Cells[2][rowCounter] = $"{bloodService.Service.PriceInRubles:F2} руб.";
                                rowCounter++;
                                currentPatientSum += bloodService.Service.PriceInRubles;
                                totalSum += bloodService.Service.PriceInRubles;
                            }
                        }
                        range = sheet.Range[sheet.Cells[1][rowCounter], sheet.Cells[2][rowCounter]];
                        range.Merge();
                        range.Value = $"Общая стоимость услуг пациента: {currentPatientSum:F2} руб.";
                        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    }
                    Excel.Range allReportRange = sheet.Range[insuranceCompanyRangeStart, sheet.Cells[2][rowCounter]];
                    allReportRange.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                        allReportRange.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                        allReportRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                        allReportRange.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                        allReportRange.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                        allReportRange.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle =
                            Excel.XlLineStyle.xlContinuous;
                    rowCounter += 2;
                }
                range = sheet.Range[sheet.Cells[1][rowCounter], sheet.Cells[2][rowCounter]];
                range.Merge();
                range.Value = $"Итоговая стоимость по всем пациентам: {totalSum:F2} руб.";
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                sheet.Columns.AutoFit();

                sheet.SaveAs(Path.Combine(path, "savedsheet.csv"), Excel.XlFileFormat.xlCSV);
                sheet.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, Path.Combine(path, "savedsheet.pdf"));
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
                throw;
            }
            finally
            {
                workbook?.Close(false);
                application?.Quit();
            }
        }
    }
}
