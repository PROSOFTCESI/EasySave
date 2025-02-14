using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySave.Utils;
using System.IO;

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
        ENCRYPT.Content = messages.GetMessage("ENCRYPTION"); 
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
        NavigationService.Navigate(new SettingsMenu());       
    }

    private void MainFrame_Navigated(object sender, NavigationEventArgs e)
    {

    }

    private void Encrypt_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new EncryptDecryptSave());
    }
}
