using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MedicalLaboratoryNumber20App.Views.Pages.Sessions
{
    /// <summary>
    /// Interaction logic for LaboratoryResearcherPage.xaml
    /// </summary>
    public partial class LaboratoryResearcherPage : Page
    {
        public LaboratoryResearcherPage(Models.Entities.User user)
        {
            InitializeComponent();
        }
    }
}
