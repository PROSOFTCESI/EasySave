using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySave.Utils;
using System.IO;
using LoggerLib;

namespace EasySave.Graphic;

/// <summary>
/// Logique d'interaction pour MainMenu.xaml
/// </summary>
public partial class MainMenu : Page
{
    private readonly Messages messages = Messages.GetInstance();
    public MainMenu()
    {
        InitializeComponent();
        CreateJobButton.Content = messages.GetMessage("CREATE_SAVE_JOB_MENU_LABEL");
        UpdateJobButton.Content = messages.GetMessage("UPDATE_SAVE_JOB_MENU_LABEL");
        ReadJobsButton.Content = messages.GetMessage("READ_SAVE_JOBS_MENU_LABEL");
        DeleteJobButton.Content = messages.GetMessage("DELETE_SAVE_JOB_MENU_LABEL");
        ChangeLanguageButton.Content = messages.GetMessage("CHANGE_LANGUAGE_MENU_LABEL");
        ExitButton.Content = messages.GetMessage("EXIT_MENU_LABEL");
        OpenSettingsButton.Content = messages.GetMessage("SETTINGS_BUTTON");
    }



    private void CreateJobMenu_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new CreateJobMenu());
    }

    private void UpdateSaveJob_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new UpdateSaveJobMenu());
    }

    private void ReadSaveJobs_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ReadSaveJobMenu());
    }

    private void DeleteSaveJob_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new DeleteSaveJobMenu());
    }

    private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ChangeLanguageMenu());
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {

        // Obtient le dossier AppData\Roaming de l'utilisateur courant
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string easySavePath = Path.Combine(appDataFolder, "EasySave");
        string fileName = "settings.json";
        string filePath = Path.Combine(easySavePath, fileName);
        try
        {
            Logger.GetInstance().Log(
             new
             {
                 Type = "Info",
                 Time = DateTime.Now,
                 Statue = "Start",
                 Message = "Start oppening setting in : " + filePath
             });

            ProcessStartInfo psi = new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Logger.GetInstance().Log(
            new
            {
                Type = "Setting",
                Time = DateTime.Now,
                Statue = "Error",
                Message = "Can't open Setting file in "+ filePath,
            });
            MessageBox.Show("Erreur lors de l'ouverture du fichier : " + ex.Message);
        }
    }

}
