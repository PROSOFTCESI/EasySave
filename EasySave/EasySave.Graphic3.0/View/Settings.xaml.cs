using EasySave.Utils;
using LoggerLib;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EasySave.Graphic3._0.View;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class Settings : Page
{
    private SettingsJsonDefinition settings = new SettingsJsonDefinition();
    private readonly Messages messages = Messages.GetInstance();

    public Settings()
    {
        InitializeComponent();
        RefreshLabels();
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        try
        {
            // Get users current AppData\Roaming
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string easySavePath = Path.Combine(appDataFolder, "EasySave");
            string fileName = "settings.json";
            string filePath = Path.Combine(easySavePath, fileName);
            ProcessStartInfo psi = new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erreur lors de l'ouverture du fichier : " + ex.Message);
        }
    }

    private void GoBack_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new MainMenu());
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        settings.Name = NameTextBox.Text;
        settings.EncryptionKey = KeyTextBox.Text;
        settings.logFormat = (bool)JsonButton.IsChecked ? "json" : "xml";
        settings.extensionsToEncrypt = ExtentionsTextBox.Text;
        settings.businessSoftwares = BusinessSoftwareTextBox.Text;
        settings.maxSizeTransferMB = MaxSizeTextBox.Text;
        settings.priorityFilesToTransfer = PriorityFilesTextBox.Text;

        if ((bool)JsonButton.IsChecked)
            Logger.GetInstance().Initialize("EasySave", (Logger.LogExportType.json));
        if ((bool)XmlButton.IsChecked)
            Logger.GetInstance().Initialize("EasySave", (Logger.LogExportType.xml));

        SettingsJson.GetInstance().Update(settings);
    }

    private void RefreshLabels()
    {
        settings = SettingsJson.GetInstance().GetContent();

        //display info from settings.json
        NameTextBox.Text = settings.Name;
        KeyTextBox.Text = settings.EncryptionKey;
        ExtentionsTextBox.Text = settings.extensionsToEncrypt;
        if (settings.logFormat == "json")
        {
            JsonButton.IsChecked = true;
            XmlButton.IsChecked = false;
        }
        if (settings.logFormat == "xml")
        {
            JsonButton.IsChecked = false;
            XmlButton.IsChecked = true;
        }
        BusinessSoftwareTextBox.Text = settings.businessSoftwares;
        MaxSizeTextBox.Text = settings.maxSizeTransferMB;
        PriorityFilesTextBox.Text = settings.priorityFilesToTransfer;

        //Labels
        NameLabel.Text = messages.GetMessage("NAME");
        KeyLabel.Text = messages.GetMessage("ENCRYPTION_KEY");
        KeyIntLabel.Text = messages.GetMessage("ENCRYPTION_KEY_INT");
        ExtentionsLabel.Text = messages.GetMessage("EXTENTION_FILES");
        ExtentionsIntLabel.Text = messages.GetMessage("EXTENTION_FILES_INT");
        LogFormatLabel.Text = messages.GetMessage("LOG_FORMAT");
        LangageButton.Content = messages.GetMessage("CHANGE_LANGUAGE_MENU_LABEL");
        SettingsButton.Content = messages.GetMessage("SETTINGS_BUTTON_OPEN");
        BusinessSoftwareLabel.Text = messages.GetMessage("BUSINESS_SOFTWARE");
        BusinessSoftwareIntLabel.Text = messages.GetMessage("BUSINESS_SOFTWARE_INT");
        MaxSizeLabel.Text = messages.GetMessage("MAX_SIZE_MB");
        MaxSizeIntLabel.Text = messages.GetMessage("MAX_SIZE_MB_INT");
        PriorityFilesLabel.Text = messages.GetMessage("PRIORITY_FILES");
        PriorityFilesIntLabel.Text = messages.GetMessage("PRIORITY_FILES_INT");
        SaveButton.Content = messages.GetMessage("SAVE");
        BackButton.Content = messages.GetMessage("BACK");
    }

    private void Langage_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ChangeLanguageMenu("Settings"));
    }
}
