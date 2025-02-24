using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace EasySaveClient
{
    public partial class MainWindow : Window
    {
        private string ServerIp = "127.0.0.1";
        private const int Port = 5000;
        private bool isConnected = false;
        public ObservableCollection<SaveJob> AvailableSaveJobs { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            AvailableSaveJobs = new ObservableCollection<SaveJob>();
            SaveJobsList.ItemsSource = AvailableSaveJobs;
        }

        private void ConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            ServerIp = ServerIpBox.Text.Trim();

            if (CheckServerConnection(ServerIp))
            {
                ResponseBox.Text = $"✅ Connecté au serveur {ServerIp}";
                isConnected = true;

                RefreshButton.IsEnabled = true;
                CreateButton.IsEnabled = true;
                RefreshJobs();
            }
            else
            {
                ResponseBox.Text = $"❌ Impossible de se connecter à {ServerIp}";
            }

        }

        private void RefreshJobs_Click(object sender, RoutedEventArgs e) => RefreshJobs();

        private void StartBackup_Click(object sender, RoutedEventArgs e) => SendCommand($"start_backup {((FrameworkElement)sender).Tag}");

        private void PauseBackup_Click(object sender, RoutedEventArgs e) => SendCommand($"pause_backup {((FrameworkElement)sender).Tag}");

        private void DeleteBackup_Click(object sender, RoutedEventArgs e) => SendCommand($"delete_backup {((FrameworkElement)sender).Tag}");

        private void CreateJob_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("create_backup");
            RefreshJobs();
        }

        private void RefreshJobs()
        {
            AvailableSaveJobs.Clear();
            string response = SendCommand("list_jobs");

            foreach (string job in response.Split(','))
                if (!string.IsNullOrWhiteSpace(job))
                    AvailableSaveJobs.Add(new SaveJob { Name = job.Trim() });
        }

        private bool CheckServerConnection(string ip)
        {
            try { using (var client = new TcpClient()) { client.Connect(ip, Port); return true; } }
            catch { return false; }
        }

        private string SendCommand(string command)
        {
            try
            {
                using (TcpClient client = new TcpClient(ServerIp, Port))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(command);
                    stream.Write(data, 0, data.Length);
                    byte[] buffer = new byte[1024];
                    return Encoding.UTF8.GetString(buffer, 0, stream.Read(buffer, 0, buffer.Length));
                }
            }
            catch (Exception ex) { return $"❌ Erreur: {ex.Message}"; }
        }
    }

    public class SaveJob { public string Name { get; set; } }
}
