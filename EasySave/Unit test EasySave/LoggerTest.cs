using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LoggerLib;
using Newtonsoft.Json;
namespace Unit_test_EasySave;

public class ObjetALog
{
    public ObjetALog(string variable1, int variable2, ObjetALog? varibale3)
    {
        Variable1 = variable1;
        Variable2 = variable2;
        Varibale3 = varibale3;
    }
    public ObjetALog() {
        Variable1 = "";
        Variable2 = 10;
        Varibale3 = null;
    }
    public string Variable1 { get; set; } 
    public int Variable2 { get; set; } 
    public ObjetALog? Varibale3 { get; set; }
}

public class LoggerTest : IDisposable
{
    private readonly string logFoldderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LoggerTests");

    public LoggerTest()
    { 
        Logger.GetInstance().Initialize("LoggerTests", Logger.LogExportType.json);
       
    }

     void IDisposable.Dispose() 
    {
       Directory.Delete(logFoldderPath, true);
    }

    [Fact]
    public void FolderCreate()
    {
        Assert.True(Directory.Exists(Path.Combine(logFoldderPath,"logs")));
    }

    [Fact]
    public void JsonTest()
    {
        Logger.GetInstance().Initialize("LoggerTests", Logger.LogExportType.json);
        ObjetALog objectInterne = new("teste",10,null);
        ObjetALog objectAStocker = new("testeObjet a stocker", -10, objectInterne);
        Logger.GetInstance().Log(objectAStocker);
        Logger.GetInstance().Log(objectAStocker);
        Logger.GetInstance().Log(objectAStocker);
        
        string jsonString = File.ReadAllText(Path.Combine(logFoldderPath, "logs",DateTime.Now.Date.ToString("yyyy-MM-dd") + ".json"));

        // Diviser les objets JSON séparés par des virgules
        string[] jsonObjects = jsonString.Split(new[] { "}," }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var jsonObject in jsonObjects.Take(jsonObjects.Length-1))
        {
            // Ajouter la fermeture manquante pour chaque objet JSON
            string validJson = jsonObject.Trim() + "}";
          
            var objetRecuperer = Newtonsoft.Json.JsonConvert.DeserializeObject<ObjetALog>(validJson);

            Assert.Equal(objetRecuperer.ToString(), objectAStocker.ToString());
        }
    }


    private ObjetALog XMLDeserialise(string xmlObjetct)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ObjetALog));
        using StringReader reader = new StringReader(xmlObjetct);

        return (ObjetALog)serializer.Deserialize(reader);
    }
    
    [Fact]
    public void XMLTest()
    {
        Logger.GetInstance().Initialize("LoggerTests",Logger.LogExportType.xml);
        ObjetALog objectInterne = new("teste", 10, null);
        ObjetALog objectAStocker = new("testeObjet a stocker", -10, objectInterne);

        Logger.GetInstance().Log(new {var1="qfdqsf", var2=-151, var3 = objectInterne });
        Logger.GetInstance().Log(objectAStocker);
        Logger.GetInstance().Log(objectAStocker);
        string xmlString = File.ReadAllText(Path.Combine(logFoldderPath, "logs", DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xml"));

        foreach (var xmlObject in xmlString.Split("<?xml version=\"1.0\" encoding=\"utf-16\"?>").Skip(2))
        {
            XmlSerializer serializer = new(typeof(Logger.LogClass));
            using StringReader reader = new(xmlObject);

            var objetRecuperer = serializer.Deserialize(reader);
            Assert.Equal( objectAStocker.ToString(), objetRecuperer.ToString());

        }
    }
}
