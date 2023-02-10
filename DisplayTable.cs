using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstateApp {
  public partial class Database {
    Table displayedTable;
    List<(int width, bool multiline)> displayedTableColumnWidth;
    
    const int minDisplayedColumnWidth = 5;
    const int maxDisplayedColumnWidth = 25;
    int maxDisplayedTableWidth = 200;

    void MakeDisplayedTable(SQLCommand sql)
    {
      Table fromTable = FindTable(sql.TableName);
      List<string> types = new List<string>();
      if (!sql.ColumnNames.Any())
        sql.ColumnNames = fromTable.GetColumnNames();

      foreach (string columnname in sql.ColumnNames)
      {
        if (columnname == sql.SingleColumnName)
          types.Add("Text");
        else
          types.Add(fromTable.FindColumn(columnname).Type.ToString());
      }

      displayedTable = new Table("", sql.ColumnNames, types);
      for (int i = 0; i < fromTable.RowsCount; i++)
      {

        List<string> values = new List<string>();
        foreach (string columnname in sql.ColumnNames)
          values.Add(fromTable.GetRecord(columnname, i).ToString());
        displayedTable.AddRow(sql.ColumnNames, values);
      }

      if (!string.IsNullOrEmpty(sql.TableNameJoin))
      {
        Table joinTable = FindTable(sql.TableNameJoin);
        for (int i = 0; i < joinTable.RowsCount; i++)
        {
          var row = joinTable.GetRow(i);
          displayedTable.SetValueWithCondition(sql.SingleColumnName, row.Records[1].ToString(), sql.SingleColumnName, "=", row.Records[0].ToString());
        }
      }

      //remove columns with wrong WHERE conditions
      if (sql.ColumnConditionWhere != null)
      {
        List<Row> deletedRows = new List<Row>();
        for (int i = 0; i < displayedTable.RowsCount; i++)
        {
          Row row = displayedTable.GetRow(i);
          int actual = 0;
          for (int c = 0; c < sql.ColumnNamesWhere.Count; c++)
          {
            if (displayedTable.CheckRowCondition(row, sql.ColumnNamesWhere[c], sql.ColumnConditionWhere[c], sql.ColumnValuesWhere[c]))
              actual++;
          }
          if (actual < sql.ColumnNamesWhere.Count)
            deletedRows.Add(row);
        }
        foreach (Row row in deletedRows)
        {
          displayedTable.DeleteRow(row);
        }
      }

      //change rows order
      if (sql.ColumnNamesOrderBy != null)
        for (int i = sql.ColumnNamesOrderBy.Count - 1; i >= 0; i--)
        {
          bool changed = true;
          while (changed)
          {
            changed = false;
            for (int r = 0; r < displayedTable.RowsCount - 1; r++)
            {
              var row1 = displayedTable.GetRow(r);
              var row2 = displayedTable.GetRow(r + 1);

              if ((sql.OrderByTypes[i] == "ASC" && displayedTable.CheckRowCondition(row1, sql.ColumnNamesOrderBy[i], ">", row2))
                || (sql.OrderByTypes[i] == "DESC" && displayedTable.CheckRowCondition(row1, sql.ColumnNamesOrderBy[i], "<", row2))
                )
              {
                displayedTable.SwapRows(row1, row2);
                changed = true;
              }
            }
          }
        }

      //rename columns
      for (int i = 0; i < sql.ColumnValues.Count; i++)
      {
        displayedTable.RenameColumn(sql.ColumnNames[i], sql.ColumnValues[i]);
      }

    }

    void ShowDisplayedTable(int startFromColumn = 0, int starTFromRow = 0)
    {
      CalculateColumnsWidth(); //calculate columns width
      int visibleTableWidth = 1;
      for (int i = startFromColumn; i < displayedTable.ColumnsCount; i++) // calculate visibleTableWidth
      {
        if (displayedTableColumnWidth[i].width + visibleTableWidth <= maxDisplayedTableWidth)
          visibleTableWidth += displayedTableColumnWidth[i].width + 1;
        else
        {
          //visibleTableWidth += i;
          break;
        }
      }

      //print table header
      Console.WriteLine("  " + new string('=', visibleTableWidth));
      var header = displayedTable.GetColumnNames();
      Console.Write("  |");
      for (int i = startFromColumn; i < header.Count; i++)
      {
        Console.Write(header[i] + new string(' ', displayedTableColumnWidth[i].width - header[i].Length) + "|");
      }
      Console.WriteLine();
      Console.WriteLine("  " + new string('=', visibleTableWidth));



      //print table content
      for (int r = starTFromRow; r < displayedTable.RowsCount; r++)
      {
        Row row = displayedTable.GetRow(r);
        Console.Write($"{r+1,3}|");
        for (int c = 0; c < displayedTable.ColumnsCount; c++)
        {
          string text = row.Records[c].ToString();
          Console.Write(text + new string(' ', displayedTableColumnWidth[c].width - text.Length) + "|");
        }
        Console.WriteLine();
      }
      Console.WriteLine("  " + new string('=', visibleTableWidth));
    }

    void CalculateColumnsWidth()
    {
      displayedTableColumnWidth = new List<(int width, bool multiline)>();
      var colNames = displayedTable.GetColumnNames();
      for (int c = 0; c < displayedTable.ColumnsCount; c++)  //initaliase displayedTableColumnWidth
        displayedTableColumnWidth.Add((Math.Max(colNames[c].Length, minDisplayedColumnWidth), false));

      for (int r = 0; r < displayedTable.RowsCount; r++)
      {
        Row row = displayedTable.GetRow(r);
        for (int c = 0; c < displayedTable.ColumnsCount; c++)
        {
          int len = row.Records[c].ToString().Length;
          if (displayedTableColumnWidth[c].width < len)
            displayedTableColumnWidth[c] = (len, false);
        }
      }

      for (int i = 0; i < displayedTableColumnWidth.Count; i++) //mark multiline columns
        if (displayedTableColumnWidth[i].width > maxDisplayedColumnWidth)
          displayedTableColumnWidth[i] = (displayedTableColumnWidth[i].width, true);
    }
  }
}