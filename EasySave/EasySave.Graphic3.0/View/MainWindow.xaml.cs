using System.Windows;
using EasySave.Graphic3._0.View;

namespace EasySave.Graphic3._0.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new MainMenu());
    }
}