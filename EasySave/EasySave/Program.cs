using EasySave.Utils;
using EasySave;

Console.WriteLine("Hello, World!");
FullSave save = new FullSave("TEST1", "C:\\Users\\Milan\\Desktop\\projetCESI\\Tests\\Tests1", "C:\\Users\\Milan\\Desktop\\projetCESI\\Saves\\");
save.CreateSave();

new ConsoleManager().Launch();
