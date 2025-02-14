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
using EasySave.CustomExceptions;
using EasySave.Utils;
using EasySave.Utils.JobStates;


namespace EasySave.Graphic;


/// <summary>
/// Logique d'interaction pour CreateJobMenu.xaml
/// </summary>
public partial class CreateJobMenu : Page
{
    private readonly Messages messages = Messages.GetInstance();
    public CreateJobMenu()
    {
        InitializeComponent();
        TitleTextBlock.Text = messages.GetMessage("CREATE_SAVE_JOB_MENU_LABEL");
        JobNameLabel.Text = messages.GetMessage("ASK_SAVE_JOB_NAME_MESSAGE");
        SourcePathLabel.Text = messages.GetMessage("ASK_SAVE_JOB_SOURCE_MESSAGE");
        DestinationPathLabel.Text = messages.GetMessage("ASK_SAVE_JOB_DESTINATION_MESSAGE");
        SaveTypeLabel.Text = messages.GetMessage("JUST_SAVE_TITLE");
        FullSaveRadioButton.Content = messages.GetMessage("FULL_SAVE_JOB_TITLE");
        DifferentialSaveRadioButton.Content = messages.GetMessage("DIFFERENTIAL_SAVE_JOB_TITLE");
        CreateButton.Content = messages.GetMessage("CREATE_SAVE_JOB_MENU_LABEL");
        BackButton.Content = messages.GetMessage("BACK");
    }

    private void CreateJob_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string saveJobName = JobNameTextBox.Text;
            string sourcePathJob = SourcePathTextBox.Text;
            string destPathJob = DestinationPathTextBox.Text;
            string saveType = FullSaveRadioButton.IsChecked == true ? "Totale" : "Différentielle";

            SaveJob newJob;

            if (FullSaveRadioButton.IsChecked ?? false)
            {
                newJob = new FullSave(saveJobName, sourcePathJob, destPathJob, true);
            }
            else
            {
                newJob = new DifferentialSave(saveJobName, sourcePathJob, destPathJob, true);
            }

            bool isCreated = newJob.CreateSave();

            if (!isCreated)
            {
                MessageBoxDisplayer.DisplayError("SAVE_JOB_CREATION_FAILED_MESSAGE");
                return;
            }

            MessageBoxDisplayer.DisplayConfirmation("SAVE_JOB_CREATED_SUCCESSFULLY", saveJobName);

        }
        catch (Exception ex)
        {
            if (ex is BusinessSoftwareRunningException)
            {
                MessageBoxDisplayer.DisplayError("BUSINESS_SOFTWARE_DETECTED_ERROR");
                return;
            }

            if (ex is Exception)
            {
                MessageBoxDisplayer.DisplayError("SAVE_JOB_CREATION_FAILED_MESSAGE");
                return;
            }
        }
    }

    private void GoBack_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();

    }
}
