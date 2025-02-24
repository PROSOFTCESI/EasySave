using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveClient
{
    public partial class MainWindow : Window
    {
        private const string ServerIp = "127.0.0.1"; // Remplace par l'IP du serveur
        private const int Port = 5000;

        public MainWindow()
        {
            InitializeComponent();
            AvailableSaveJobs = new ObservableCollection<SaveJob>();
            SaveJobsList.ItemsSource = AvailableSaveJobs;
        }

        private void ConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            ServerIp = ServerIpBox.Text.Trim();
            
            string numberString = ServerPortBox.Text.Trim();
            if (!int.TryParse(numberString, out int ServerPort))
            {
                ResponseBox.Text = "Erreur Port";
                return;
            }
         
            if (CheckServerConnection(ServerIp, ServerPort))
            {
                ResponseBox.Text = $"✅ Connecté au serveur {ServerIp} au port {ServerPort}";
                isConnected = true;

                RefreshButton.IsEnabled = true;
                CreateButton.IsEnabled = true;
                RefreshJobs();
            }
            else
            {
                ResponseBox.Text = $"❌ Impossible de se connecter à {ServerIp} port {ServerPort}";
            }

        }
        private void TogglePlayPause_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string jobName = button.Tag.ToString();

            if (button.Content.ToString() == "⏸")
            {
                SendCommand($"pause_backup {jobName}");
                button.Content = "▶"; 
        }
            else 
            {
                SendCommand($"start_backup {jobName}");
                button.Content = "⏸"; 
            }
        }

        private void StartBackup_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("start_backup");
        }

        private void StopBackup_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("stop_backup");
        }

        private void CheckStatus_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("status");
        }

        private void SendCommand(string command)
        {
            try
            {
                using (TcpClient client = new TcpClient(ServerIp, Port))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(command);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    ResponseBox.Text = $"Réponse: {response}";
                }
            }
            catch (Exception ex)
            {
                ResponseBox.Text = $"Erreur: {ex.Message}";
            }
            catch (Exception ex) { return $"❌ Erreur: {ex.Message}"; }
        }
    }

    public class SaveJob { public string Name { get; set; } }
}
