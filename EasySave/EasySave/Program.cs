using EasySave;

Console.WriteLine("Hello, World!");
//FullSave save = new FullSave("TEST1", "C:\\Users\\Milan\\Desktop\\projetCESI\\Tests\\Tests1", "C:\\Users\\Milan\\Desktop\\projetCESI\\Saves\\");

DifferentialSave save1 = new DifferentialSave("TEST_2_DIFF", "C:\\Users\\Milan\\Desktop\\projetCESI\\Tests\\TestDiff", "C:\\Users\\Milan\\Desktop\\projetCESI\\Saves\\");
Console.WriteLine(save1.CreateSave());