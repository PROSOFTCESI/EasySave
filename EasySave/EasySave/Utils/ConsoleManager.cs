﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using EasySave.Utils.JobStates;
using System.Text.RegularExpressions;

namespace EasySave.Utils;
internal class ConsoleManager
{
    private bool isRunning = true;
    private readonly Messages messages = Messages.GetInstance(); // To switch between languages

    /// <summary>
    /// Launch the main menu while the program is running
    /// </summary>
    public void Launch()
    {
        while(isRunning)
        {
            try
            {
                ShowMainMenu();
                string input = GetUserInput();
                ClearConsole();

                switch (input)
                {
                    case "1":
                        CreateSaveJobMenu();
                        break;
                    case "2":
                        UpdateSaveJobMenu();
                        break;
                    case "3":
                        ReadSaveJobsMenu();
                        break;
                    case "4":
                        DeleteSaveJobMenu();
                        break;
                    case "5":
                        LanguageSelectionMenu();
                        break;
                    case "6":
                        isRunning = false;
                        break;
                    default:
                        WriteRed(messages.GetMessage("INVALID_INPUT_MESSAGE"));
                        break;
                }
            }
            catch (Exception)
            {
                WriteRed(messages.GetMessage("UNKNOWN_ERROR_MESSAGE"));
            }
            PressKeyToContinue(isRunning ? null : messages.GetMessage("THANKS_END_MESSAGE"));
        }
    }

    /// <summary>
    /// Display the main menu
    /// </summary>
    private void ShowMainMenu()
    {
        Console.Clear();
        WriteCyan(messages.GetMessage("TOP_MENU_LABEL"));
        WriteCyan("1 - ", messages.GetMessage("CREATE_SAVE_JOB_MENU_LABEL"));
        WriteCyan("2 - ", messages.GetMessage("UPDATE_SAVE_JOB_MENU_LABEL"));
        WriteCyan("3 - ", messages.GetMessage("READ_SAVE_JOBS_MENU_LABEL"));
        WriteCyan("4 - ", messages.GetMessage("DELETE_SAVE_JOB_MENU_LABEL"));
        WriteCyan("5 - ", messages.GetMessage("CHANGE_LANGUAGE_MENU_LABEL"));
        WriteCyan("6 - ", messages.GetMessage("EXIT_MENU_LABEL"));
        WriteCyan(messages.GetMessage("BOTTOM_MENU_LABEL"));
    }

    /// <summary>
    /// Display the create job menu and get the user input (job name, source path, destination path...)
    /// </summary>
    private void CreateSaveJobMenu()
    {
        WriteCyan(messages.GetMessage("CREATE_SAVE_JOB_MENU_LABEL"));
        string name = GetUserInput("ASK_SAVE_JOB_NAME_MESSAGE");
        string source = GetUserInput("ASK_SAVE_JOB_SOURCE_MESSAGE");
        string destination = GetUserInput("ASK_SAVE_JOB_DESTINATION_MESSAGE");
        string type = GetUserInput("ASK_SAVE_JOB_TYPE_MESSAGE");

        // Create the save job
        bool isCreated;
        SaveJob saveJob;

        switch(type)
        {
            case "0":
                saveJob = new FullSave(name, source, destination);
                break;
            case "1":
                saveJob = new DifferentialSave(name, source, destination);
                break;
            default:
                WriteRed(messages.GetMessage("INVALID_INPUT_MESSAGE"));
                return;
        }

        isCreated = saveJob.CreateSave();

        if (!isCreated)
        {
            WriteRed(messages.GetMessage("SAVE_JOB_CREATION_FAILED_MESSAGE"));
            return;
        }
        WriteGreen(string.Format(messages.GetMessage("SAVE_JOB_CREATED_SUCCESSFULLY"), name));
    }

    /// <summary>
    /// Update one or multiple save jobs
    /// </summary>
    private void UpdateSaveJobMenu()
    {
        WriteCyan(messages.GetMessage("UPDATE_SAVE_JOB_MENU_LABEL"));
        List<SaveJob> availableJobs = SaveJob.Instances;
        if (!ShowAvailableSaveJobs(availableJobs))
        {
            return;
        }
        string jobsToUpdateInput = GetUserInput("ASK_JOBS_TO_UPDATE");
        List<int>? jobsToUpdateTab = GetJobsToUpdate(jobsToUpdateInput);

        if (jobsToUpdateTab == null)
        {
            WriteRed(messages.GetMessage("INVALID_INPUT_MESSAGE"));
            return;
        }

        WriteYellow(messages.GetMessage("UPDATING_JOBS_MESSAGE"));

        bool jobsUpdated = false;
        // Update the job(s)
        try
        {
            foreach (int jobIndex in jobsToUpdateTab)
            {
                SaveJob job = availableJobs[jobIndex];
                job.Save();
            }
            jobsUpdated = true;
        }
        catch (Exception)
        {
            jobsUpdated = false;
        }

        if (!jobsUpdated)
        {
            WriteRed(messages.GetMessage("SAVE_JOB_UPDATE_FAILED_MESSAGE"));
            return;
        }

        WriteGreen(string.Format(messages.GetMessage("SAVE_JOB_UPDATED_SUCCESSFULLY"), string.Join(",", jobsToUpdateTab)));
    }

    /// <summary>
    /// Display all the actives save jobs
    /// </summary>
    private void ReadSaveJobsMenu()
    {
        WriteCyan(messages.GetMessage("READ_SAVE_JOBS_MENU_LABEL"));
       
        ShowAvailableSaveJobs(SaveJob.Instances);
    }

    /// <summary>
    /// Display the delete job menu
    /// </summary>
    private void DeleteSaveJobMenu()
    {
        WriteCyan(messages.GetMessage("DELETE_SAVE_JOB_MENU_LABEL"));
        List<SaveJob> availableJobs = SaveJob.Instances;
        if (!ShowAvailableSaveJobs(availableJobs))
        {
            return;
        }
        int jobToDeleteIndex = Int32.Parse(GetUserInput("ASK_SAVE_JOB_TO_DELETE_MESSAGE"));
        SaveJob jobToDelete = availableJobs[jobToDeleteIndex];

        jobToDelete.DeleteSave();

        bool jobDeleted = true;
        if (!jobDeleted)
        {
            WriteRed(messages.GetMessage("SAVE_JOB_DELETION_FAILED_MESSAGE"));
            return;
        }
        WriteGreen(string.Format(messages.GetMessage("SAVE_JOB_DELETED_SUCCESSFULLY"), jobToDelete));
    }

    /// <summary>
    /// Change the language of the program
    /// </summary>
    private void LanguageSelectionMenu()
    {
        int i = 0;
        foreach (CultureInfo culture in Messages.availableCultures)
        {
            WriteCyan($"{i} - {culture.NativeName}");
            i++;
        }
        string input = GetUserInput();
        if (int.TryParse(input, out int index) && index >= 0 && index < Messages.availableCultures.Count)
        {
            messages.SetCulture(Messages.availableCultures[index]);
            WriteGreen(messages.GetMessage("LANGUAGE_CHANGED_SUCCESSFULLY"));
        }
        else
        {
            WriteRed(messages.GetMessage("INVALID_INPUT_MESSAGE"));
        }
    }

    /// <summary>
    /// Display the current save jobs, or a warning message if no save job is available
    /// </summary>
    /// <param name="jobs"></param>
    /// <returns>True if at least one save job is available, false otherwise</returns>
    private bool ShowAvailableSaveJobs(List<SaveJob> jobs)
    {
        if (jobs.Count == 0)
        {
            WriteYellow(messages.GetMessage("NO_SAVE_JOB_MESSAGE"));
            return false;
        }

        for (int i = 0; i < jobs.Count; i++)
        {
            WriteCyan($"{i} - {jobs[i]}");
        }
        return true;
    }

    /// <summary>
    /// Method that ask the user for an input
    /// </summary>
    /// <param name="inputInfoMessage">The custom input message for the user</param>
    /// <returns>The string input of the user</returns>
    private string GetUserInput(string? inputInfoMessage = null)
    {
        if (inputInfoMessage == null)
        {
            Write(messages.GetMessage("ASK_USER_INPUT_MESSAGE")); // Default input info message
        }
        else
        {
            Write(messages.GetMessage(inputInfoMessage));
        }
        return Console.ReadLine();
    }

    /// <summary>
    /// Used in the update jobs
    /// </summary>
    /// <param name="userInput"></param>
    /// <returns></returns>
    private static List<int>? GetJobsToUpdate(string? userInput)
    {
        if (string.IsNullOrEmpty(userInput))
        {
            return null;
        }

        List<int> jobsToUpdate = [];
        userInput = userInput.Trim();
        int[] numbers;

        if (!userInput.Contains("-") && !userInput.Contains(";"))
        {
            try
            {
                jobsToUpdate.Add(Int32.Parse(userInput));
            }
            catch (Exception e)
            {
                if (e is FormatException || e is OverflowException)
                {
                    return null;
                }
                throw;
            }
            return jobsToUpdate;
        }

        if (userInput.Contains("-"))
        {
            try
            {
                numbers = userInput.Split('-').Select(int.Parse).ToArray();
            }
            catch (Exception e)
            {
                if (e is FormatException || e is OverflowException)
                {
                    return null;
                }
                throw;
            }
            int firstNumber = numbers[0];
            int secondNumber = numbers[1];

            if (firstNumber > secondNumber)
            {
                return null;
            }

            for (int i = firstNumber; i <= secondNumber; i++)
            {
                jobsToUpdate.Add(i);
            }

            return jobsToUpdate;
        }

        try
        {
            numbers = userInput.Split(';').Select(int.Parse).ToArray();
        }
        catch (Exception e)
        {
            if (e is FormatException || e is OverflowException)
            {
                return null;
            }
            throw;
        }

        foreach (int number in numbers)
        {
            jobsToUpdate.Add(number);
        }

        return jobsToUpdate;
    }

    private static void ClearConsole()
    {
        Console.Clear();
    }
    private void PressKeyToContinue(string? infoMessage = null)
    {
        if (infoMessage == null)
        {
            WriteGrey(messages.GetMessage("PRESS_KEY_TO_CONTINUE_MESSAGE"));
        }
        else
        {
            WriteGrey(messages.GetMessage(infoMessage));
        }
        Console.ReadKey();
    }
    private static void WriteColored(ConsoleColor color, params object[] args)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(string.Join(" ", args));
        Console.ResetColor();
    }

    public static void Write(params object[] args) => WriteColored(ConsoleColor.White, args);
    public static void WriteRed(params object[] args) => WriteColored(ConsoleColor.Red, args);
    public static void WriteGreen(params object[] args) => WriteColored(ConsoleColor.Green, args);
    public static void WriteYellow(params object[] args) => WriteColored(ConsoleColor.Yellow, args);
    public static void WriteCyan(params object[] args) => WriteColored(ConsoleColor.Cyan, args);
    public static void WriteMagenta(params object[] args) => WriteColored(ConsoleColor.Magenta, args);
    public static void WriteGrey(params object[] args) => WriteColored(ConsoleColor.Gray, args);
}
