using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasySave.Utils;
using LoggerLib;

namespace EasySave.Graphic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        public MainWindow()
        {
            Logger.GetInstance().Log(
          new
          {
              Type = "Launching",
              Time = DateTime.Now,
              Statue = "Start",
              Message = "Launching Main Window"
          });
            InitializeComponent();
            MainFrame.Navigate(new MainMenu()); // Charge la page principale
            if (SettingsJson.GetInstance().GetContent().logFormat.Equals("json"))
            {
                Logger.GetInstance().Initialize("EasySave", (Logger.LogExportType.json));
            }
            else if (SettingsJson.GetInstance().GetContent().logFormat.Equals("xml"))
            {
                Logger.GetInstance().Initialize("EasySave", (Logger.LogExportType.xml));
            }
        }
    }
}
