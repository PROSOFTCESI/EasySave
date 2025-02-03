using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace EasySave.Utils;
internal class ConsoleManager
{
    private bool isRunning = true;
    private readonly Messages messages = new(Messages.FR); // To switch between languages

    public void Launch()
    {
        while(isRunning)
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
            PressKeyToContinue(isRunning ? null : messages.GetMessage("THANKS_END_MESSAGE"));
        }
    }
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

    private void CreateSaveJobMenu()
    {
        WriteCyan(messages.GetMessage("CREATE_SAVE_JOB_MENU_LABEL"));
        string name = GetUserInput("ASK_SAVE_JOB_NAME_MESSAGE");
        string source = GetUserInput("ASK_SAVE_JOB_SOURCE_MESSAGE");
        string destination = GetUserInput("ASK_SAVE_JOB_DESTINATION_MESSAGE");
        string type = GetUserInput("ASK_SAVE_JOB_TYPE_MESSAGE");

        // TODO : Create the save job
        bool isCreated = true;
        if (!isCreated)
        {
            WriteRed(messages.GetMessage("SAVE_JOB_CREATION_FAILED_MESSAGE"));
            return;
        }
        WriteGreen(string.Format(messages.GetMessage("SAVE_JOB_CREATED_SUCCESSFULLY"), name));
    }

    private void UpdateSaveJobMenu()
    {
        WriteCyan(messages.GetMessage("UPDATE_SAVE_JOB_MENU_LABEL"));
        ShowAvailableSaveJobs();
        string jobsToUpdate = GetUserInput("ASK_JOBS_TO_UPDATE");

        WriteYellow(messages.GetMessage("UPDATING_JOBS_MESSAGE"));

        // TODO : Update the job(s)
        Thread.Sleep(2000);
        bool jobsUpdated = true;

        if (!jobsUpdated)
        {
            WriteRed(messages.GetMessage("SAVE_JOB_UPDATE_FAILED_MESSAGE"));
            return;
        }

        WriteGreen(string.Format(messages.GetMessage("SAVE_JOB_UPDATED_SUCCESSFULLY"), jobsToUpdate));
    }

    private void ReadSaveJobsMenu()
    {
        WriteCyan(messages.GetMessage("READ_SAVE_JOBS_MENU_LABEL"));
        ShowAvailableSaveJobs();
    }

    private void DeleteSaveJobMenu()
    {
        WriteCyan(messages.GetMessage("DELETE_SAVE_JOB_MENU_LABEL"));
        ShowAvailableSaveJobs();
        string name = GetUserInput("ASK_SAVE_JOB_TO_DELETE_MESSAGE");

        // TODO : Delete the save job
        bool jobDeleted = true;
        if (!jobDeleted)
        {
            WriteRed(messages.GetMessage("SAVE_JOB_DELETION_FAILED_MESSAGE"));
            return;
        }
        WriteGreen(string.Format(messages.GetMessage("SAVE_JOB_DELETED_SUCCESSFULLY"), name));
    }

    private void LanguageSelectionMenu()
    {
        int i = 0;
        foreach (CultureInfo culture in messages.availableCultures)
        {
            WriteCyan($"{i} - {culture.NativeName}");
            i++;
        }
        string input = GetUserInput();
        if (int.TryParse(input, out int index) && index >= 0 && index < messages.availableCultures.Count)
        {
            messages.SetCulture(messages.availableCultures[index]);
            WriteGreen(messages.GetMessage("LANGUAGE_CHANGED_SUCCESSFULLY"));
        }
        else
        {
            WriteRed(messages.GetMessage("INVALID_INPUT_MESSAGE"));
        }
    }

    private void ShowAvailableSaveJobs()
    {
        // TODO : Get all the save jobs and use the .toString() method to display them
        List<string> saveJobs = [
            "'Base de données' | Source : 'C:/BDD/Prod', Destination : 'C:/BDD/Backup', Sauvegarde totale, Etat : 'SAVING'",
            "'Dossier perso' | Source : 'C:/Perso':, Destination : 'D:/Perso', Sauvegarde différentielle, Etat : 'SAVED'",
            "'Dossier pro' | Source : 'C:/Pro', Destination : 'D:/Pro', Sauvegarde totale, Etat : 'SAVED'"
        ];
        for (int i = 0; i < saveJobs.Count; i++)
        {
            WriteCyan($"{i} - {saveJobs[i]}");
        }
    }

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
