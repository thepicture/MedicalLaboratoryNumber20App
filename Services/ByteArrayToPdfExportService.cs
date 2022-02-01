using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace MedicalLaboratoryNumber20App.Services
{
    public class ByteArrayToPdfExportService : IExportService
    {
        private readonly byte[] bytes;

        public ByteArrayToPdfExportService(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public bool TryExport(out string filePath)
        {
            filePath = null;
            Word.Application app = null;
            Word.Document document = null;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            string temporaryImagePath = string.Empty;
            try
            {
                temporaryImagePath =
                Path
                .Combine(Environment.CurrentDirectory,
                         "temp.png");
                File.WriteAllBytes(temporaryImagePath, bytes);
                string saveFileName = "BarCode-"
                                      + $"{DateTime.Now:yyyy-mm-dd_hh-mm-ss}.pdf";
                string savePath = Path.Combine(dialog.SelectedPath,
                                               saveFileName);
                app = new Word.Application();
                document = app.Documents.Add();
                Word.Paragraph paragraph = document.Paragraphs.Add();
                paragraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                _ = paragraph.Range.InlineShapes.AddPicture(temporaryImagePath);
                document.ExportAsFixedFormat(savePath,
                                             Word.WdExportFormat.wdExportFormatPDF);
                filePath = savePath;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.StackTrace);
            }
            finally
            {
                if (File.Exists(temporaryImagePath))
                {
                    File.Delete(temporaryImagePath);
                }
                document?.Close(SaveChanges: false);
                app?.Quit(SaveChanges: false);
            }
            return false;
        }
    }
}