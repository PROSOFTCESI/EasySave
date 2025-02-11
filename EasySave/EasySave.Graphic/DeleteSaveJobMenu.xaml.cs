using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySave.Utils;

namespace EasySave.Graphic
{
    /// <summary>
    /// Logique d'interaction pour DeleteSaveJobMenu.xaml
    /// </summary>
    public partial class DeleteSaveJobMenu : Page
    {
        private readonly Messages messages = Messages.GetInstance();
        private readonly List<string> saveJobs;

        public DeleteSaveJobMenu()
        {
            InitializeComponent();

            UpdateTitle.Text = messages.GetMessage("DELETE_SAVE_JOB_MENU_LABEL");
            InstructionLabel.Text = messages.GetMessage("ASK_SAVE_JOB_TO_DELETE_MESSAGE");
            DeleteButton.Content = messages.GetMessage("DELETE_SAVE_JOB_MENU_LABEL");
            BackButton.Content = messages.GetMessage("BACK");

            saveJobs = new List<string>
            {
                "0 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
                "1 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
                "2 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'",
            };

            SaveJobsListBox.ItemsSource = saveJobs;
        }

        private void DeleteSelectedJob_Click(object sender, RoutedEventArgs e)
        {
            if (SaveJobsListBox.SelectedItem != null)
            {
                string selectedJob = SaveJobsListBox.SelectedItem.ToString();
                int jobNumber = int.Parse(selectedJob.Split('-')[0].Trim());

                saveJobs.RemoveAt(jobNumber);
                SaveJobsListBox.ItemsSource = null;
                SaveJobsListBox.ItemsSource = saveJobs;

                MessageBox.Show(
                    string.Format(messages.GetMessage("SAVE_JOB_DELETED_SUCCESSFULLY"), selectedJob),
                    "Confirmation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    messages.GetMessage("INVALID_INPUT_MESSAGE"),
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
