using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EasySaveRemote
{
    class TCPServer
    {
        private static int Port = 5000;
        private static bool isRunning = true;

        static void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Any, Port);
            server.Start();
            Console.WriteLine($"[EasySave.Remote] Serveur en écoute sur le port {Port}...");

            EasySaveController controller = new EasySaveController();

            while (isRunning)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client, controller));
                clientThread.Start();
            }
        }

        static async void HandleClient(TcpClient client, EasySaveController controller)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine($"Commande reçue : {command}");

                    string response = await controller.ExecuteCommand(command);

                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Erreur de lecture du flux réseau : {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
