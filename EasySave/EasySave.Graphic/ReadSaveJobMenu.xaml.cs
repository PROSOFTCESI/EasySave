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
using EasySave.Utils;
using EasySave.Utils.JobStates;

namespace EasySave.Graphic
{
    /// <summary>
    /// Logique d'interaction pour ReadSaveJobMenu.xaml
    /// </summary>
    public partial class ReadSaveJobMenu : Page
    {


        private readonly Messages messages = Messages.GetInstance();
        private readonly List<string> saveJobs;

        public ReadSaveJobMenu()
        {
            InitializeComponent();

            UpdateTitle.Text = messages.GetMessage("READ_SAVE_JOBS_MENU_LABEL");
            BackButton.Content = messages.GetMessage("BACK");

            List<SaveJob> saveJobs = StateJsonReader.GetInstance().GetJobs();

            SaveJobsListBox.ItemsSource = saveJobs;
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
