﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySave.Utils;
using EasySave.Utils.JobStates;
using LoggerLib;

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
            DeleteButton.Content = messages.GetMessage("DELETE_SAVE_JOB_MENU_LABEL");
            BackButton.Content = messages.GetMessage("BACK");

            UpdateList();
        }

        private void UpdateList()
        {
            var jobsList = StateJsonReader.GetInstance().GetJobs(true);
            SaveJobsListBox.ItemsSource = jobsList;
        }

        private void DeleteSelectedJob_Click(object sender, RoutedEventArgs e)
        {
            if (SaveJobsListBox.SelectedItem == null)
            {
                MessageBox.Show(
                    messages.GetMessage("INVALID_INPUT_MESSAGE"),
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

         

            SaveJob selectedJob = (SaveJob)SaveJobsListBox.SelectedItem;
            Logger.GetInstance().Log(
              new
              {
                  Type = "Delete",

                  Time = DateTime.Now,
                  Statue = "Start",
                  Message = "Start deleting SaveJobs" + selectedJob.Name
              });
            selectedJob.DeleteSave();
            UpdateList();

            Logger.GetInstance().Log(
                new
                {
                    Type = "Delete",

                    Time = DateTime.Now,
                    Statue = "Success",
                    Message = "Save Job" + selectedJob.Name+ "successfully deleted"
                });

            MessageBox.Show(
                string.Format(messages.GetMessage("SAVE_JOB_DELETED_SUCCESSFULLY"), selectedJob),
                "Confirmation",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
