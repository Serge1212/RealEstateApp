using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RealEstateApp {
  [Serializable]
  public partial class Database {
    List<Table> tables;

    public void Save(string filename = "base.dat")
    {
      BinaryFormatter formatter = new BinaryFormatter();
      using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
      {
        formatter.Serialize(fs, tables);
      }
    }

    public void Load(string filename = "base.dat")
    {
      BinaryFormatter formatter = new BinaryFormatter();
      using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
      {
        if (fs.Length > 0)
        tables = (List<Table>)formatter.Deserialize(fs);
      }
    }

    public Database()
    {
      tables = new List<Table>();
    }

    private void ShowTables()
    {
      Console.WriteLine(new string ('=', 20));
      foreach (Table t in tables)
        Console.WriteLine(t.TableName);
      Console.WriteLine(new string('=', 20));
    }

    private void CreateTable(
      string tableName,
      List<string> columnNames,
      List<string> columnTypes)
    {
      tables.Add(new Table(tableName, columnNames, columnTypes));
    }

    private Table FindTable(string tableName)
    {
      foreach (Table t in tables)
        if (t.TableName == tableName)
          return t;
      return null;
    }

    private void DeleteTable(string tableName)
    {
      Table t = FindTable(tableName);
      tables.Remove(t);
    }

  }
}
