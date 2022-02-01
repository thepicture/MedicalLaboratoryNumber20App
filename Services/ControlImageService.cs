using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MedicalLaboratoryNumber20App.Services
{
    /// <summary>
    /// Реализует метод для преобразования элемента управления 
    /// в изображение.
    /// </summary>
    public class ControlImageService
    {
        /// <summary>
        /// Конвертирует элемент управления в .png.
        /// </summary>
        /// <param name="control">Элемент управления.</param>
        /// <returns>Байтовое представления изображения.</returns>
        public static byte[] ConvertToPng(Control control)
        {
            var rect = new Rect(control.RenderSize);
            var visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(control), null, rect);
            }
                RenderTargetBitmap bitmap =
                    new RenderTargetBitmap((int)control.ActualWidth,
                                           (int)control.ActualHeight,
                                           96,
                                           96,
                                           PixelFormats.Pbgra32);
            bitmap.Render(visual);
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (MemoryStream stream = new MemoryStream())
            {
                pngEncoder.Save(stream);
                return stream.ToArray();
            }
        }
    }
}
