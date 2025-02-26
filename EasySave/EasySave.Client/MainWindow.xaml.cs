using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EasySaveClient
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<SaveJob> AvailableSaveJobs { get; set; } = new ObservableCollection<SaveJob>();

        private string ServerIp = "127.0.0.1";
        private int  ServerPort = 5000;
        private bool isConnected = false;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            AvailableSaveJobs = new ObservableCollection<SaveJob>();
            SaveJobsList.ItemsSource = AvailableSaveJobs;
            // Initialisation du timer
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isConnected)
            {
                RefreshJobs();
            }
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
                isConnected = false;
            }

        }
        //private async void TogglePlayPause_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    if (button == null) return;

        //    string jobName = button.Tag.ToString();

        //    if (button.Content.ToString() == "▶")
        //    {
        //        button.Content = "⏸";
        //        button.IsEnabled = false; 
        //        await SendCommand($"update_backup {jobName}");
        //        button.IsEnabled = true;
        //    }
        //    else
        //    {
        //        button.Content = "▶";
        //    }
        //}


        private async void RefreshJobs_Click(object sender, RoutedEventArgs e) => RefreshJobs();

        private async void PlayPause_Click(object sender, RoutedEventArgs e) => SendCommand($"update_backup {((FrameworkElement)sender).Tag}");


        private async void DeleteBackup_Click(object sender, RoutedEventArgs e)
        {
            string jobName = ((FrameworkElement)sender).Tag.ToString();
            MessageBoxResult result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le job {jobName} ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await SendCommand($"delete_backup {jobName}");
                RefreshJobs();
            }
        }

        private async void CreateJob_Click(object sender, RoutedEventArgs e)
        {
            string jobName = JobNameBox.Text.Trim();
            string sourcePath = SourcePathBox.Text.Trim();
            string targetPath = TargetPathBox.Text.Trim();
            string saveType = (SaveTypeBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(sourcePath) ||
                string.IsNullOrEmpty(targetPath) || string.IsNullOrEmpty(saveType))
            {
                ResponseBox.Text = "Tous les champs doivent être remplis.";
                return;
            }

            // Construire la commande en enveloppant chaque paramètre dans des guillemets
            string command = $"create_backup \"{jobName}\" \"{sourcePath}\" \"{targetPath}\" \"{saveType}\"";
            Console.WriteLine($"Command: {command}"); // Pour vérification
            string response = await SendCommand(command);
            ResponseBox.Text = response;
            RefreshJobs();
        }



        // Ajouter le nouveau travail à la collection
        private async void RefreshJobs()
        {
            AvailableSaveJobs.Clear();
            string response = await SendCommand("list_jobs");


            List<SaveJob>? jobsList = JsonSerializer.Deserialize<List<SaveJob>>(response);

            if (jobsList != null)
            {
                foreach (var job in jobsList)
                {
                    AvailableSaveJobs.Add(job);
                }
            }
        }


        private bool CheckServerConnection(string ip, int port)
        {
            try { using (var client = new TcpClient()) { client.Connect(ip, port); return true; } }
            catch { return false; }
        }

        private async Task<string> SendCommand(string command)
        {
            try
            {
                using (TcpClient client = new TcpClient(ServerIp, ServerPort))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(command);
                    await stream.WriteAsync(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                return $"❌ Erreur: {ex.Message}";
            }
        }


    }
    public class SaveJob
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string State { get; set; }
        public long? Progression { get; set; }
    }

}
