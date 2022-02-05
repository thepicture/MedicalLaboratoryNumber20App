using MedicalLaboratoryNumber20App.Models.Entities;
using System.IO;
using System.Linq;
using Word = Microsoft.Office.Interop.Word;

namespace MedicalLaboratoryNumber20App.Services
{
    public class OrderReportService : IReportService<Order>
    {
        public bool GenerateReport(Order obj, string path)
        {
            Word.Application app = null;
            Word.Document document = null;
            try
            {
                app = new Word.Application();
                app.Visible = true;
                document = app.Documents.Add();
                Word.Paragraph paragraph = document.Paragraphs.Add();
                Word.Range range = paragraph.Range;
                Word.Table table = range.Tables.Add(range, 9, 2);
                table.Range.Borders.InsideLineStyle =
                    table.Range.Borders.OutsideLineStyle =
                    Word.WdLineStyle.wdLineStyleSingle;
                table.Range.Cells.VerticalAlignment = Word
                    .WdCellVerticalAlignment
                    .wdCellAlignVerticalCenter;
                table.Cell(1, 1).Range.Text = "Параметр заказа";
                table.Cell(1, 2).Range.Text = "Значение";
                table.Rows[1].Range.Bold = 1;
                table.Cell(2, 1).Range.Text = "Дата заказа";
                table.Cell(2, 2).Range.Text = obj.CreationDate.ToString();
                table.Cell(3, 1).Range.Text = "Номер заказа";
                table.Cell(3, 2).Range.Text = obj.OrderId.ToString();
                table.Cell(4, 1).Range.Text = "Номер пробирки";
                table.Cell(4, 2).Range.Text = obj.Blood.Barcode;
                table.Cell(5, 1).Range.Text = "Номер страхового полиса";
                table.Cell(5, 2).Range.Text = obj.Blood.Patient.SecurityNumber;
                table.Cell(6, 1).Range.Text = "ФИО";
                table.Cell(6, 2).Range.Text = obj.Blood.Patient.PatientFullName;
                table.Cell(7, 1).Range.Text = "Дата рождения";
                table.Cell(7, 2).Range.Text = obj.Blood.Patient.BirthDate.ToString("dd/MM/yyyy");
                table.Cell(8, 1).Range.Text = "Перечень услуг";
                table.Cell(8, 2).Range.Text = string.Join(", ", obj.Service.Select(s => s.ServiceName));
                table.Cell(9, 1).Range.Text = "Стоимость";
                table.Cell(9, 2).Range.Text = obj.Service.Sum(s => s.PriceInRubles).ToString() + " руб.";
                document.ExportAsFixedFormat(Path.Combine(path,
                                                          "ЭлектронныйЗаказ-" +
                                                          $"{obj.CreationDate:yyyy-MM-dd_hh-mm-ss}" +
                                                          ".pdf"),
                                Word.WdExportFormat.wdExportFormatPDF);
                return true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }
            finally
            {
                document?.Close(false);
                app?.Quit(false);
            }
        }
    }
}
