﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySave.CustomExceptions;
using EasySave.Utils;
using EasySave.Utils.JobStates;
using LoggerLib;

namespace EasySave.Graphic;

/// <summary>
/// Logique d'interaction pour UpdateSaveJobMenu.xaml
/// </summary>
public partial class UpdateSaveJobMenu : Page
{
    // Instance de Messages initialisée avec la langue par défaut (ici Messages.EN)
    private readonly Messages messages = Messages.GetInstance();

    // Liste des travaux de sauvegarde disponibles
    private readonly List<string> saveJobs;

    public UpdateSaveJobMenu()
    {
        InitializeComponent();

        // Mise à jour des libellés en fonction de la langue
        UpdateTitle.Text = messages.GetMessage("UPDATE_SAVE_JOB_MENU_LABEL");
 
        UpdateButton.Content = messages.GetMessage("UPDATE_SAVE_JOB_MENU_LABEL");
        BackButton.Content = messages.GetMessage("BACK");

        // Initialisation de la liste des travaux (exemple)
        //saveJobs = new List<string>
        //{
        //    "0 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
        //    "1 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
        //    "2 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'",
        //    "3 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
        //    "4 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
        //    "5 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'",
        //    "6 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
        //    "7 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
        //    "8 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'",
        //    "9 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
        //    "10 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
        //    "11 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'",
        //    "12 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
        //    "13 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
        //    "14 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'",
        //    "15 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
        //    "16 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
        //    "17 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'",
        //    "18 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
        //    "19 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
        //    "20 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'",
        //    "21 - 'SaveULTIME' | Source : 'C:\\Users\\Poirr\\Downloads' | Destination : 'C:\\Users\\Poirr\\Documents\\SAVETEST'",
        //    "22 - 'SaveWork' | Source : 'D:\\Projects' | Destination : 'D:\\Backup'",
        //    "23 - 'SavePhotos' | Source : 'E:\\Photos' | Destination : 'E:\\Backup\\Photos'"
        //};

        StateJsonReader stateJsonReader = StateJsonReader.GetInstance();

        List<SaveJob> jobs = SaveJob.Instances;

        // Remplir la ListBox avec les travaux disponibles
        foreach (SaveJob job in jobs)
        {
            SaveJobsListBox.Items.Add(job);
        }
    }

    private void UpdateSelectedJobs_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Récupération des éléments sélectionnés dans la ListBox
            var selectedJobs = SaveJobsListBox.SelectedItems;
            if (selectedJobs.Count == 0)
            {
                // Message indiquant qu'aucun travail n'a été sélectionné
                MessageBox.Show(
                    messages.GetMessage("NO_SAVE_JOB_MESSAGE"),
                    messages.GetMessage("INVALID_INPUT_MESSAGE"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            foreach (var job in selectedJobs)
            {
                SaveJob saveJob = (SaveJob)job;
                Logger.GetInstance().Log(
                    new
                    {
                        Type = "Update",
                        Time = DateTime.Now,
                        Statue = "Start",
                        Message = "Start updating " + saveJob.Name,
                        SaveJob = saveJob
                    }
                );
                bool updated = saveJob.Save();
                if (!updated)
                {
                    // Message indiquant qu'un travail n'a pas pu être mis à jour
                    Logger.GetInstance().Log(
                        new
                        {
                            Type = "Update",
                            Time = DateTime.Now,
                            Statue = "Error",
                            Message = "Error updating the job " + saveJob.Name,
                            SaveJob = saveJob
                        }
                    );
                    MessageBoxDisplayer.DisplayError("SAVE_JOB_UPDATE_FAILED_MESSAGE");
                    return;
                }
                Logger.GetInstance().Log(
                    new
                    {
                        Type = "Update",
                        Time = DateTime.Now,
                        Statue = "Success",
                        Message = "Sucessfuly update " + job,
                        SaveJobs = job
                    }
                );
            }

            // Concaténer les travaux sélectionnés
            string jobsUpdated = string.Join("\n", selectedJobs.Cast<object>());
            // Affichage d'un message de confirmation avec la liste des travaux (remplacement du placeholder {0})
            MessageBoxDisplayer.DisplayConfirmation("SAVE_JOB_UPDATED_SUCCESSFULLY", jobsUpdated);
        }
        catch (Exception ex)
        {
            Logger.GetInstance().Log(
                new
                {
                    Type = "Update",
                    Time = DateTime.Now,
                    Statue = "Error",
                    Message = "Error updating a job",
                    Error = ex.ToString()
                }
            );

            if (ex is BusinessSoftwareRunningException)
            {
                MessageBoxDisplayer.DisplayError("BUSINESS_SOFTWARE_DETECTED_ERROR");
                return;
            }

            if (ex is Exception)
            {
                MessageBoxDisplayer.DisplayError("SAVE_JOB_UPDATE_FAILED_MESSAGE");
                return;
            }
        }       
    }

    private void GoBack_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }
}
