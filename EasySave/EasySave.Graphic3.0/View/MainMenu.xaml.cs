using EasySave.Graphic;
using EasySave.Graphic3._0.ViewModel;
using EasySave.Utils.JobStates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace EasySave.Graphic3._0.View;

/// <summary>
/// Interaction logic for MainMenu.xaml
/// </summary>
public partial class MainMenu : Page, INotifyPropertyChanged
{
    private ObservableCollection<SaveJob> _availableSaveJobs;
    private readonly DispatcherTimer _timer;

    public ObservableCollection<SaveJob> AvailableSaveJobs
    {
        get => _availableSaveJobs;
        set
        {
            _availableSaveJobs = value;
            OnPropertyChanged(nameof(AvailableSaveJobs));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainMenu()
    {
        InitializeComponent();
        AvailableSaveJobs = new();
        RefreshJobs();
        DataContext = this;
    }

    private void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new CreateJob());
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshJobs();
    }

    private async void Play_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is SaveJob item)
        {
            var response = await UpdateJobViewModel.Update(item);
            MessageBoxDisplayer.Display(response);
        }
        RefreshJobs();
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is SaveJob item)
        {
            //StateJsonReader.GetInstance().GetJobs().Remove(item);
        }
        RefreshJobs();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new Settings());
    }

    private void RefreshJobs()
    {
        AvailableSaveJobs.Clear();
        foreach (var job in StateJsonReader.GetInstance().GetJobs(true))
        {
            AvailableSaveJobs.Add(job);
        }
    }

    // Manual scroll
    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var scrollViewer = sender as ScrollViewer;

        if (scrollViewer != null)
        {
            double newOffset = scrollViewer.VerticalOffset - (e.Delta / 6);

            if (newOffset < 0)
            {
                newOffset = 0;
            }
            else if (newOffset > scrollViewer.ScrollableHeight)
            {
                newOffset = scrollViewer.ScrollableHeight;
            }

            scrollViewer.ScrollToVerticalOffset(newOffset);

            e.Handled = true;
        }
    }
}
