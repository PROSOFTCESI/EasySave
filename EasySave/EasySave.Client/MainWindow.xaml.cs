using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace EasySaveClient
{
    public partial class MainWindow : Window
    {
        private const string ServerIp = "127.0.0.1"; // Remplace par l'IP du serveur
        private const int Port = 5000;

        public MainWindow()
        {
            InitializeComponent();
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
        }
    }
}
