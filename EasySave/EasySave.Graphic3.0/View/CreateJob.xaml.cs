using EasySave.Graphic3._0.ViewModel;
using EasySave.Utils;
using LoggerLib;
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

namespace EasySave.Graphic3._0.View;

/// <summary>
/// Interaction logic for CreateJob.xaml
/// </summary>
public partial class CreateJob : Page
{
    private readonly Messages messages = Messages.GetInstance();
    public CreateJob()
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

    private async void CreateJob_Click(object sender, RoutedEventArgs e)
    {
        string saveJobName = JobNameTextBox.Text;
        string sourcePathJob = SourcePathTextBox.Text;
        string destPathJob = DestinationPathTextBox.Text;
        string saveType = FullSaveRadioButton.IsChecked == true ? "TOTAL" : "DIFFERENTIAL";

        var response = await CreateJobViewModel.Create(saveJobName, sourcePathJob, destPathJob, saveType);
        MessageBoxDisplayer.Display(response);
    }

    private void GoBack_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }
}
