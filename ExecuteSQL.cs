using System;
using System.Linq;

namespace RealEstateApp {
  public partial class Database {
    void ExecuteSQL (SQLCommand command)
    {

      Table t;
      switch (command.Type)
      {
        case SQLcommandTypes.SHOW_TABLES:
          ShowTables();
          break;
        case SQLcommandTypes.CREATE_TABLE:
          CreateTable(command.TableName, command.ColumnNames, command.ColumnTypes);
          break;
        case SQLcommandTypes.INSERT_INTO:
          t = FindTable(command.TableName);
          t.AddRow(command.ColumnNames, command.ColumnValues);
          break;
        case SQLcommandTypes.DELETE_FROM:
          t = FindTable(command.TableName);
          if (string.IsNullOrEmpty(command.SingleColumnName))
            t.ClearAllRows();
          else
            t.DeleteRows(command.SingleColumnName, command.SingleValue);
          break;
        case SQLcommandTypes.DROP_TABLE:
          DeleteTable(command.TableName);
          break;
        case SQLcommandTypes.ALTER_TABLE_RENAME_TO:
          t = FindTable(command.TableName);
          t.RenameTable(command.TableNameNew);
          break;
        case SQLcommandTypes.ALTER_TABLE_RENAME_COLUMN:
          t = FindTable(command.TableName);
          t.RenameColumn(command.SingleColumnName, command.SingleColumnNameNew);
          break;
        case SQLcommandTypes.ALTER_TABLE_DELETE_COLUMN:
          t = FindTable(command.TableName);
          t.DeleteColumn(command.SingleColumnName);
          break;
        case SQLcommandTypes.ALTER_TABLE_ADD_COLUMN:
          t = FindTable(command.TableName);
          t.AddColumn(command.ColumnNames[0], command.ColumnTypes[0]);
          break;
        case SQLcommandTypes.UPDATE:
          t = FindTable(command.TableName);
          for (int i = 0; i < command.ColumnNames.Count; i++)
            t.SetValueWithCondition(command.ColumnNames[i], command.ColumnValues[i], command.SingleColumnName, "=", command.SingleValue);
          break;
        case SQLcommandTypes.SELECT:
          MakeDisplayedTable(command);
          ShowDisplayedTable();
          break;
      }
    }

    public bool ValidateSQLCommand(ref SQLCommand SQLcommand)
    {
      SQLcommand.IsValid = true;
      switch (SQLcommand.Type)
      {
        case SQLcommandTypes.SHOW_TABLES:
          return true;
        case SQLcommandTypes.CREATE_TABLE:
          if (FindTable(SQLcommand.TableName) != null) //errorTableWithSameNameExists
          {
            SQLcommand.ErrorText = string.Format(Resource.errorTableWithSameNameExists, SQLcommand.TableName);
            SQLcommand.IsValid = false;
            return false;
          }
          if (SQLcommand.ColumnNames.Distinct().Count() != SQLcommand.ColumnNames.Count()) //errorColumnNamesAreTheSame
          {
            SQLcommand.ErrorText = Resource.errorColumnNamesAreTheSame;
            SQLcommand.IsValid = false;
            return false;
          }
          if ((SQLcommand.ColumnTypes.Where(x => Array.IndexOf(Enum.GetNames(typeof(DataTypes)), x) == -1)).Any()) //errorNoSuchType
          {
            SQLcommand.ErrorText = Resource.errorNoSuchType;
            SQLcommand.IsValid = false;
            return false;
          }
          break;
        case SQLcommandTypes.INSERT_INTO:
          Table t = FindTable(SQLcommand.TableName);
          if (t == null)
          {
            SQLcommand.ErrorText = string.Format(Resource.errorTableNotExists, SQLcommand.TableName);
            SQLcommand.IsValid = false;
            return false;
          }
          if (SQLcommand.ColumnNames.Any() && SQLcommand.ColumnNames.Count() != SQLcommand.ColumnValues.Count())
          {
            SQLcommand.ErrorText = Resource.errorDifferentCountOfColumnsAndValues;
            SQLcommand.IsValid = false;
            return false;
          }
          break;
        case SQLcommandTypes.DROP_TABLE:
          Table t1 = FindTable(SQLcommand.TableName);
          if (t1 == null)
          {
            SQLcommand.ErrorText = string.Format(Resource.errorTableNotExists, SQLcommand.TableName);
            SQLcommand.IsValid = false;
            return false;
          }
          break;

      }
      return SQLcommand.IsValid;
    }

    public void SendSQL(string SQLcommand)
    {
      string[] subCommands = SQLcommand.Split(";", StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < subCommands.Length; i++)
        subCommands[i] = subCommands[i].Replace(System.Environment.NewLine, "").Trim();
      foreach (string _subCommand in subCommands)
      {
        SQLCommand command = new SQLCommand(_subCommand);
        if (command.Type == SQLcommandTypes.UNKNOWN)
          Console.WriteLine();
        ValidateSQLCommand(ref command);
        if (!command.IsValid)
          Console.WriteLine(command.ErrorText);
        else
          ExecuteSQL(command);
      }
    }
  }
}
