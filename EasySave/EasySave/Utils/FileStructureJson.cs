using EasySave.Utils.JobStates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Utils
{
    public class FileStructureJson
    {
        private static FileStructureJson? instance;
        public static FileStructureJson GetInstance()
        {
            instance ??= new FileStructureJson();
            return instance;
        }

        public string CreateFileStructure(string sourceDirectory, string jsonPath)
        {
            var jsonStructure = new JsonStructure();

            // Read Files
            var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                byte[] hash = MD5.Create().ComputeHash(File.OpenRead(file)); //Create mdr5 HASH
                jsonStructure.Files.Add(new FileItem { 
                    Name = file.Replace(sourceDirectory + Path.DirectorySeparatorChar, ""), 
                    Hash = BitConverter.ToString(hash),
                    Size = new FileInfo(file).Length.ToString(),
                });
            }

            // Create JSON
            string jsonOutput = JsonConvert.SerializeObject(jsonStructure, Formatting.Indented);

            // Save JSON File
            string jsonFile = Path.Combine(jsonPath, ".fileStructure.json");
            File.WriteAllText(jsonFile, jsonOutput);

            return jsonFile;
        }

        public string CreateDiffenretialFileStructure(string sourceDirectory, string jsonPath, string jsonSaved)
        {
            var jsonStructure = new JsonStructure();

            //Source Files
            var sourceFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

            // Saved Files in Full Save
            string jsonContent = File.ReadAllText(jsonSaved);
            var jsonSourceStructure = JsonConvert.DeserializeObject<JsonStructure>(jsonContent);
            var savedFiles = jsonSourceStructure.Files;

            foreach (var sourceFile in sourceFiles)
            {
                bool toDiffSave = true;

                byte[] sourceHash = MD5.Create().ComputeHash(File.OpenRead(sourceFile)); // Create mdr5 HASH

                foreach(var savedFile in savedFiles)
                {
                    if(savedFile.Name == sourceFile.Replace(sourceDirectory + Path.DirectorySeparatorChar, "")) 
                    {
                        if (BitConverter.ToString(sourceHash) == savedFile.Hash) // if hash ==, file has not been changed
                        {
                            toDiffSave = false;
                        }                        
                    }
                }                

                if(toDiffSave)
                {
                    jsonStructure.Files.Add(new FileItem
                    {
                        Name = sourceFile.Replace(sourceDirectory + Path.DirectorySeparatorChar, ""),
                        Hash = BitConverter.ToString(sourceHash),
                        Size = new FileInfo(sourceFile).Length.ToString(),
                    });
                }

            }

            // Create JSON
            string jsonOutput = JsonConvert.SerializeObject(jsonStructure, Formatting.Indented);

            // Save JSON File
            string jsonFile = Path.Combine(jsonPath, ".fileStructure.json");
            File.WriteAllText(jsonFile, jsonOutput);

            return jsonFile;
        }

        public void GetAdvancement(string jsonFilePath)
        {
            // faire ration SET/SAVED en %                      --> avancement save
            // faire ration tous fichiers / Encrypted en %      --> avancement encrypt
            // prendre en compte la taille des fichiers
            //          faire ration poids Set / poids SAVED

            int allFiles = 0;
            int savedFiles = 0;
            int encryptedFiles = 0;

            int allBytes = 0;
            int savedBytes = 0;
            int encryptedBytes = 0;

            string jsonContent = File.ReadAllText(jsonFilePath);
            var jsonStructure = JsonConvert.DeserializeObject<JsonStructure>(jsonContent);
            foreach (var item in jsonStructure.Files)
            {
                switch(item.Status)
                {
                    case "set":
                        allFiles++;
                        break;
                    case "saved":
                        savedBytes += int.Parse(item.Size);
                        allBytes += int.Parse(item.Size);
                        savedFiles++;
                        allFiles++;
                        break;
                    case "encrypted":
                        encryptedBytes += int.Parse(item.Size);
                        allBytes += int.Parse(item.Size);
                        encryptedFiles++;
                        allFiles++;
                        break;
                    case "decrypted":
                        savedBytes += int.Parse(item.Size);
                        allBytes += int.Parse(item.Size);
                        savedFiles++;
                        allFiles++;
                        break;
                }
            }

            jsonStructure.SavedBytes = (savedBytes + encryptedBytes).ToString();
            jsonStructure.SavedFiles = (savedFiles + encryptedFiles).ToString();
            jsonStructure.SetFiles = allFiles.ToString();
            jsonStructure.EncryptedFiles = encryptedFiles.ToString();
            jsonStructure.EncryptedBytes = encryptedBytes.ToString();


            if(allFiles > 0 && allBytes > 0)
            {
                jsonStructure.SaveAdvancement = (savedBytes + encryptedBytes) + "/" + allBytes;
                jsonStructure.EncryptAdvancement = encryptedBytes + "/" + allBytes;
            }

            string updatedJson = JsonConvert.SerializeObject(jsonStructure, Formatting.Indented);
            File.WriteAllText(jsonFilePath, updatedJson);
        }
    }

    public class FileItem
    {
        public string Name { get; set; }
        public string Status { get; set; } = "set";
        public string Hash { get; set; }
        public string Size { get; set; }
    }

    public class JsonStructure
    {
        public string Status { get; set; } = "set";
        public string SetFiles { get; set; } = "0";
        public string SavedFiles { get; set; } = "0";
        public string EncryptedFiles { get; set; } = "0";
        public string SavedBytes { get; set; } = "0";
        public string EncryptedBytes { get; set; } = "0";
        public string SaveAdvancement { get; set; } = "0";
        public string EncryptAdvancement { get; set; } = "0";

        public List<FileItem> Files { get; set; } = new List<FileItem>();
    }
}
