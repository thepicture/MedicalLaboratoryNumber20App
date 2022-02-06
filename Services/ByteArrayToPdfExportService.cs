using System;
using System.IO;
using Word = Microsoft.Office.Interop.Word;

namespace MedicalLaboratoryNumber20App.Services
{
    public class ByteArrayToPdfExportService : IExportService
    {
        private readonly byte[] bytes;
        private readonly string filePath;
        private readonly string fileName;

        public ByteArrayToPdfExportService(byte[] bytes,
                                           string filePath,
                                           string fileName)
        {
            this.bytes = bytes;
            this.filePath = filePath;
            this.fileName = fileName;
        }

        public void Export()
        {
            Word.Application app = null;
            Word.Document document = null;
            string temporaryImagePath = string.Empty;
            try
            {
                temporaryImagePath =
                Path
                .Combine(Environment.CurrentDirectory,
                         "temp.png");
                File.WriteAllBytes(temporaryImagePath, bytes);
                string savePath = Path.Combine(filePath,
                                               fileName);
                app = new Word.Application();
                document = app.Documents.Add();
                Word.Paragraph paragraph = document.Paragraphs.Add();
                paragraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                _ = paragraph.Range.InlineShapes.AddPicture(temporaryImagePath);
                document.ExportAsFixedFormat(savePath,
                                             Word.WdExportFormat.wdExportFormatPDF);
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
        }
    }
}