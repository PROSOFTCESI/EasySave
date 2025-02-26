namespace EasySave.Utils
{
    internal class SmartFileCopier
    {
        // Variables statiques partagées entre les instances
        private static int NbPriorityFiles;
        private static int NbRegularFiles;
        private static int NbLargeFiles;

        private string[] _priorityExtensions;
        private long _largeFileThreshold;

        static SmartFileCopier()
        {
            NbPriorityFiles = 0;
            NbRegularFiles = 0;
            NbLargeFiles = 0;
        }

        // Constructeur de la classe
        public SmartFileCopier()
        {
            _priorityExtensions = SettingsJson.GetInstance().GetContent().priorityFilesToTransfer.Split(" ").Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();
            int maxSize;
            Int32.TryParse(SettingsJson.GetInstance().GetContent().maxSizeTransferMB, out maxSize);

            _largeFileThreshold = 1024 * 1024 * maxSize; //Max size in MB
        }

        // Méthode pour copier un fichier
        public bool CopyFile(string source, string destination)
        {
            var fileInfo = new FileInfo(source);
            bool response;

            if (_priorityExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
            {
                response = CopyPriorityFile(source, destination);
            }
            else if (fileInfo.Length > _largeFileThreshold)
            {
                response = CopyLargeFile(source, destination);
            }
            else
            {
                response = CopyRegularFile(source, destination);
            }

            return response;
        }

        private bool CopyPriorityFile(string source, string destination)
        {
            try
            {
                // Incrémenter le compteur des fichiers prioritaires
                Interlocked.Increment(ref NbPriorityFiles);

                Console.WriteLine($"Copying priority file: {source} to {destination}");
                File.Copy(source, destination, true);

                return true;
            }
            finally
            {
                // Décrémenter le compteur des fichiers prioritaires
                Interlocked.Decrement(ref NbPriorityFiles);
            }
        }

        private bool CopyLargeFile(string source, string destination)
        {
            try
            {
                // Attendre que les fichiers prioritaires et larges soient traités
                while (NbLargeFiles > 0 || NbPriorityFiles > 0)
                {
                    Console.WriteLine("Waiting for larges files and priority files to finish...");
                    Thread.Sleep(1000);
                }

                Console.WriteLine($"Copying large file: {source} to {destination}");

                // Incrémenter le compteur des fichiers gros
                Interlocked.Increment(ref NbLargeFiles);
                File.Copy(source, destination, true);


                return true;
            }
            finally
            {
                // Décrémenter le compteur des fichiers gros
                Interlocked.Decrement(ref NbLargeFiles);
            }
        }

        private bool CopyRegularFile(string source, string destination)
        {
            try
            {
                // Attendre que les fichiers prioritaires soient traités
                while (NbPriorityFiles > 0)
                {
                    // Attendre que le compteur de fichiers prioritaires atteigne 0
                    Console.WriteLine("Waiting for priority files to finish...");
                    Thread.Sleep(1000);
                }

                // Incrémenter le compteur des fichiers réguliers
                Interlocked.Increment(ref NbRegularFiles);

                // Effectuer la copie ici (simuler le travail de copie)
                Console.WriteLine($"Copying regular file: {source} to {destination}");
                File.Copy(source, destination, true);

                return true;
            }
            finally
            {
                // Décrémenter le compteur des fichiers réguliers
                Interlocked.Decrement(ref NbRegularFiles);
            }
        }
    }
}
