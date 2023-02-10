using System;

namespace RealEstateApp {
  internal class Program {
    static void Main(string[] args) {
      Database db = new Database();
      db.Load();
      Console.OutputEncoding = System.Text.Encoding.Unicode; //for cyrillic
      Console.InputEncoding = System.Text.Encoding.Unicode; //for cyrillic
      Console.WriteLine(Resource.textWelcome);
      Console.WriteLine(Resource.textAllTables);
      db.SendSQL("SHOW TABLES;");
      Console.WriteLine(Resource.textEnterCommand);

      string command;
      while ((command = Console.ReadLine()) != null)
      {
        if (command.ToLowerInvariant() == "exit")
          break;
        db.SendSQL(command);
      }
      db.Save();

    }
  }
}
