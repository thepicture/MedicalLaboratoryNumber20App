using System.Printing;
using System.Windows.Controls;
using System.Windows.Media;

namespace MedicalLaboratoryNumber20App.Services
{
    public class PrintVisualExportService : IExportService
    {
        private readonly Visual _visual;
        private readonly string _description;

        public PrintVisualExportService(Visual visual,
                                        string description)
        {
            _visual = visual;
            _description = description;
        }

        public void Export()
        {
            using (PrintServer printServer = new PrintServer())
            {
                PrintDialog printDialog = new PrintDialog
                {
                    PrintQueue = new PrintQueue(printServer, "Microsoft Print to PDF"),
                };
                printDialog.PrintVisual(_visual, _description);
            }
        }
    }
}
