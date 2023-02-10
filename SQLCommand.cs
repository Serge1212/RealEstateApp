using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RealEstateApp {
  public enum SQLcommandTypes
  {
    UNKNOWN,
    SHOW_TABLES,
    CREATE_TABLE,
    INSERT_INTO,
    DELETE_FROM,
    DROP_TABLE,
    ALTER_TABLE_RENAME_TO,
    ALTER_TABLE_RENAME_COLUMN,
    ALTER_TABLE_DELETE_COLUMN,
    ALTER_TABLE_ADD_COLUMN,
    UPDATE,
    SELECT
  }

  public class SQLCommand {

    public bool IsValid = false;
    public string ErrorText;

    public SQLcommandTypes Type;
    public string TableName;
    public string TableNameNew;
    public string TableNameJoin;

    public List<string> ColumnNames;
    public List<string> ColumnTypes;
    public List<string> ColumnValues;

    public List<string> ColumnNamesWhere;
    public List<string> ColumnConditionWhere;
    public List<string> ColumnValuesWhere;

    public List<string> ColumnNamesOrderBy;
    public List<string> OrderByTypes;

    public string SingleColumnName;
    public string SingleColumnNameNew;
    public string SingleColumnNameJoin;

    public string SingleValue;

    public SQLCommand(string command)
    {
      if (command == null)
        command = "";

      if (command == "SHOW TABLES")
      {
        Type = SQLcommandTypes.SHOW_TABLES;
      }
      else if (new Regex(@"^CREATE TABLE [a-zA-Z0-9_-]+ \(([a-zA-Z0-9_-]+ [a-zA-Z]+,? ?)*\)$").IsMatch(command))
      {
        Type = SQLcommandTypes.CREATE_TABLE;
        Regex regex = new Regex(@"^CREATE TABLE ([a-zA-Z0-9_-]+) ?\((?:([a-zA-Z0-9_-]+) ([a-zA-Z]+),? ?)*\)$");
        Match match = regex.Match(command);
        TableName = match.Groups[1].Value;
        ColumnNames = new List<string>();
        ColumnNames.AddRange(match.Groups[2].Captures.Select(x => x.Value));
        ColumnTypes = new List<string>();
        ColumnTypes.AddRange(match.Groups[3].Captures.Select(x => x.Value));
      }
      else if (new Regex(@"^INSERT INTO [a-zA-Z0-9_-]+ ?(\(([a-zA-Z0-9_-]+,? ?)*\))? VALUES ?\(('[^']+',? ?)*\)$").IsMatch(command))
      {
        Type = SQLcommandTypes.INSERT_INTO;
        Regex regex = new Regex(@"^INSERT INTO ([a-zA-Z0-9_-]+) ?(?:\((?:([a-zA-Z0-9_-]+),? ?)*\))? VALUES ?\((?:'([^']+)',? ?)*\)$");
        Match match = regex.Match(command);
        TableName = match.Groups[1].Value;
        ColumnNames = new List<string>();
        ColumnNames.AddRange(match.Groups[2].Captures.Select(x => x.Value));
        ColumnValues = new List<string>();
        ColumnValues.AddRange(match.Groups[3].Captures.Select(x => x.Value));
      }
      else if (new Regex(@"^DELETE FROM ([a-zA-Z0-9_-]+)( WHERE ([a-zA-Z0-9_-]+) = '([^']+)')?$").IsMatch(command))
      {
        Type = SQLcommandTypes.DELETE_FROM;
        Regex regex = new Regex(@"^DELETE FROM ([a-zA-Z0-9_-]+)(?: WHERE ([a-zA-Z0-9_-]+) = '([^']+)')?$");
        Match match = regex.Match(command);
        TableName = match.Groups[1].Value;
        SingleColumnName = match.Groups[2].Value;
        SingleValue = match.Groups[3].Value;
      }
      else if (new Regex(@"^DROP TABLE [a-zA-Z0-9_-]+$").IsMatch(command))
      {
        Type = SQLcommandTypes.DROP_TABLE;
        TableName = command.Substring(10).Trim();
      }
      else if (new Regex(@"^ALTER TABLE [a-zA-Z0-9_-]+ RENAME TO [a-zA-Z0-9_-]+$").IsMatch(command))
      {
        Type = SQLcommandTypes.ALTER_TABLE_RENAME_TO;
        Regex regex = new Regex(@"^ALTER TABLE ([a-zA-Z0-9_-]+) RENAME TO ([a-zA-Z0-9_-]+)$");
        Match match = regex.Match(command);
        TableName = match.Groups[1].Value;
        TableNameNew = match.Groups[2].Value;
      }
      else if (new Regex(@"^ALTER TABLE [a-zA-Z0-9_-]+ RENAME COLUMN [a-zA-Z0-9_-]+ TO [a-zA-Z0-9_-]+$").IsMatch(command))
      {
        Type = SQLcommandTypes.ALTER_TABLE_RENAME_COLUMN;
        Regex regex = new Regex(@"^ALTER TABLE ([a-zA-Z0-9_-]+) RENAME COLUMN ([a-zA-Z0-9_-]+) TO ([a-zA-Z0-9_-]+)$");
        Match match = regex.Match(command);
        TableName = match.Groups[1].Value;
        SingleColumnName = match.Groups[2].Value;
        SingleColumnNameNew = match.Groups[3].Value;
      }
      else if (new Regex(@"^ALTER TABLE [a-zA-Z0-9_-]+ DELETE COLUMN [a-zA-Z0-9_-]+$").IsMatch(command))
      {
        Type = SQLcommandTypes.ALTER_TABLE_DELETE_COLUMN;
        Regex regex = new Regex(@"^ALTER TABLE ([a-zA-Z0-9_-]+) DELETE COLUMN ([a-zA-Z0-9_-]+)$");
        Match match = regex.Match(command);
        TableName = match.Groups[1].Value;
        SingleColumnName = match.Groups[2].Value;
      }
      else if (new Regex(@"^ALTER TABLE [a-zA-Z0-9_-]+ ADD COLUMN [a-zA-Z0-9_-]+ [a-zA-Z]+$").IsMatch(command))
      {
        Type = SQLcommandTypes.ALTER_TABLE_ADD_COLUMN;
        Regex regex = new Regex(@"^ALTER TABLE ([a-zA-Z0-9_-]+) ADD COLUMN ([a-zA-Z0-9_-]+) ([a-zA-Z]+)$");
        Match match = regex.Match(command);
        TableName = match.Groups[1].Value;
        ColumnNames = new List<string>();
        ColumnNames.Add(match.Groups[2].Value);
        ColumnTypes = new List<string>();
        ColumnTypes.Add(match.Groups[3].Value);
      }
      else if (new Regex(@"^UPDATE [a-zA-Z0-9_-]+ SET ([a-zA-Z0-9_-]+ ?= ?'[^']+',? ?)+ WHERE [a-zA-Z0-9_-]+ ?= ?'[^']+'$").IsMatch(command))
      {
        Type = SQLcommandTypes.UPDATE;
        Regex regex = new Regex(@"^UPDATE ([a-zA-Z0-9_-]+) SET (?:([a-zA-Z0-9_-]+) ?= ?'([^']+)',? ?)+ WHERE ([a-zA-Z0-9_-]+) = '([^']+)'$");
        Match match = regex.Match(command);
        TableName = match.Groups[1].Value;
        ColumnNames = new List<string>();
        ColumnNames.AddRange(match.Groups[2].Captures.Select(x => x.Value));
        ColumnValues = new List<string>();
        ColumnValues.AddRange(match.Groups[3].Captures.Select(x => x.Value));
        SingleColumnName = match.Groups[4].Value;
        SingleValue = match.Groups[5].Value;
      }
      else if (command.StartsWith("SELECT "))
      {
        #region split select command parts
        int joinPosition = command.IndexOf(" JOIN ");
        int wherePosition = command.IndexOf(" WHERE ");
        int orderbyPosition = command.IndexOf(" ORDER BY ");
        string commandPartSelect = command;
        if (joinPosition > 0 || wherePosition > 0 || orderbyPosition > 0)
        {
          commandPartSelect = command.Remove(new List<int>() { joinPosition, wherePosition, orderbyPosition }.Where(x => x > 0).Min()).Trim();
          command = command.Substring(commandPartSelect.Length).Trim();
        }
        else
          command = "";

        wherePosition = command.IndexOf("WHERE ");
        orderbyPosition = command.IndexOf("ORDER BY ");
        string commandPartJoin = "";
        if (joinPosition >= 0)
        {
          commandPartJoin = command;
          if (wherePosition > 0 || orderbyPosition > 0)
          {
            commandPartJoin = command.Remove(new List<int>() { wherePosition, orderbyPosition }.Where(x => x > 0).Min()).Trim();
            command = command.Substring(commandPartJoin.Length).Trim();
          }
          else
            command = "";
        }
        string commandPartWhere = "";
        orderbyPosition = command.IndexOf("ORDER BY ");
        if (wherePosition >= 0)
        {
          commandPartWhere = command;
          if (orderbyPosition > 0)
          {
            commandPartWhere = command.Remove(orderbyPosition).Trim();
            command = command.Substring(commandPartWhere.Length).Trim();
          }
          else
            command = "";
        }
        string commandPartOrderBy = command;
        #endregion

        Regex regexSelect = new Regex(@"^SELECT \((?:(?:(?:([a-zA-Z0-9_-]+)(?: ?= ?'([^']+)')?,? ?)+)|[\*])\) FROM ([a-zA-Z0-9_-]+)$");
        Regex regexJoin = new Regex(@"^JOIN ([a-zA-Z0-9_-]+) ON ([a-zA-Z0-9_-]+) = ([a-zA-Z0-9_-]+)$");
        Regex regexWhere = new Regex(@"^WHERE (?:([a-zA-Z0-9_-]+) ?([=<>]{1,2}) ?'([^']+)',? ?)+$");
        Regex regexOrderBy = new Regex(@"^ORDER BY (?:([a-zA-Z0-9_-]+) (ASC|DESC),? ?)+$");

        #region match parts
        if (regexSelect.IsMatch(commandPartSelect))
        {
          Type = SQLcommandTypes.SELECT;
          Match match = regexSelect.Match(commandPartSelect);
          ColumnNames = new List<string>();
          ColumnNames.AddRange(match.Groups[1].Captures.Select(x => x.Value));
          ColumnValues = new List<string>();
          ColumnValues.AddRange(match.Groups[2].Captures.Select(x => x.Value));
          TableName = match.Groups[3].Value;
        }

        if (regexJoin.IsMatch(commandPartJoin))
        {
          Match match = regexJoin.Match(commandPartJoin);
          TableNameJoin = match.Groups[1].Value;
          SingleColumnName = match.Groups[2].Value;
          SingleColumnNameJoin = match.Groups[3].Value;
        }

        if (regexWhere.IsMatch(commandPartWhere))
        {
          Match match = regexWhere.Match(commandPartWhere);
          ColumnNamesWhere = new List<string>();
          ColumnNamesWhere.AddRange(match.Groups[1].Captures.Select(x => x.Value));
          ColumnConditionWhere = new List<string>();
          ColumnConditionWhere.AddRange(match.Groups[2].Captures.Select(x => x.Value));
          ColumnValuesWhere = new List<string>();
          ColumnValuesWhere.AddRange(match.Groups[3].Captures.Select(x => x.Value));
        }
        if (regexOrderBy.IsMatch(commandPartOrderBy))
        {
          Match match = regexOrderBy.Match(commandPartOrderBy);
          ColumnNamesOrderBy = new List<string>();
          ColumnNamesOrderBy.AddRange(match.Groups[1].Captures.Select(x => x.Value));
          OrderByTypes = new List<string>();
          OrderByTypes.AddRange(match.Groups[2].Captures.Select(x => x.Value));
        }
        #endregion
      }
    }
  }
}
