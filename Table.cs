using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstateApp {
  [Serializable]
  public class Table {
        public string TableName { get; private set; }
    private List<Column> columns;
    private List<Row> rows;
    public int MaxColumnIndex { get; private set; }
    public int MaxRowIndex { get; private set; }

    public Table (string Name, List<string> columnNames, List<string> columnTypes)
    {
      TableName = Name;
      this.columns = new List<Column>();
      rows = new List<Row>();
      MaxColumnIndex = -1;
      MaxRowIndex = -1;

      for (int i = 0; i < columnNames.Count; i++)
        AddColumn(columnNames[i], columnTypes[i]);
    }

    public int ColumnsCount => columns.Count;
    public int RowsCount => rows.Count;

    public void RenameTable(string newTableName)
    {
      TableName = newTableName;
    }

    private Column FindColumn(int index)
    {
      foreach (Column c in columns)
        if (c.Index == index)
          return c;
      return null;
    }
    public Column FindColumn(string columnName)
    {
      foreach (Column c in columns)
        if (c.Name == columnName)
          return c;
      return null;
    }
    public void AddColumn (string columnName, string type)
    {
      Column c = new Column(columnName, type, ++MaxColumnIndex);
      columns.Add(c);

      for (int i = 0; i < rows.Count; i++)
      {
        switch (c.Type)
        {
          case DataTypes.Int:
            rows[i].Records.Add(new Record<int>(MaxColumnIndex, 0));
            break;
          case DataTypes.Text:
            rows[i].Records.Add(new Record<string>(MaxColumnIndex, ""));
            break;
          case DataTypes.Bool:
            rows[i].Records.Add(new Record<bool>(MaxColumnIndex, false));
            break;
          case DataTypes.Double:
            rows[i].Records.Add(new Record<double>(MaxColumnIndex, 0));
            break;
          case DataTypes.Decimal:
            rows[i].Records.Add(new Record<decimal>(MaxColumnIndex, 0));
            break;
        }
      }
    }
    public void RenameColumn(string oldColumnName, string newColumnName)
    {
      Column c = FindColumn(oldColumnName);
      c.Name = newColumnName;
    }

    public void DeleteColumn(string columnName)
    {
      Column c = FindColumn(columnName);
      columns.Remove(c);
    }

    public void AddRow(List<string> columnsNames, List<string> values)
    {
      Row newRow = new Row(++MaxRowIndex);
      for (int i = 0; i < values.Count; i++)
      {
        int columnIndex;
        if (columnsNames.Count > 0)
          columnIndex = FindColumn(columnsNames[i]).Index;
        else
          columnIndex = i;
        Column c = FindColumn(columnIndex);
        switch (c.Type)
        {
          case DataTypes.Int:
            newRow.Records.Add(new Record<int>(columnIndex, values[i]));
            break;
          case DataTypes.Text:
            newRow.Records.Add(new Record<string>(columnIndex, values[i]));
            break;
          case DataTypes.Bool:
            newRow.Records.Add(new Record<bool>(columnIndex, values[i]));
            break;
          case DataTypes.Double:
            newRow.Records.Add(new Record<double>(columnIndex, values[i]));
            break;
          case DataTypes.Decimal:
            newRow.Records.Add(new Record<decimal>(columnIndex, values[i]));
            break;
        }
      }
      rows.Add(newRow);
    }

    public void DeleteRow(Row row)
    {
      rows.Remove(row);
    }

    public bool CheckRowCondition(Row row, string conditionColumnName, string columnCondition, string conditionValue)
    {
      Column _conditionColumn = FindColumn(conditionColumnName);
      bool conditionIsActual = false;

      UniversalRecord comparableRecord = null;
      switch (_conditionColumn.Type)
      {
        case DataTypes.Int:
          comparableRecord = new Record<int>(0, conditionValue);
          break;
        case DataTypes.Text:
          comparableRecord = new Record<string>(0, conditionValue);
          break;
        case DataTypes.Bool:
          comparableRecord = new Record<bool>(0, conditionValue);
          break;
        case DataTypes.Double:
          comparableRecord = new Record<double>(0, conditionValue);
          break;
        case DataTypes.Decimal:
          comparableRecord = new Record<decimal>(0, conditionValue);
          break;
      }

      //check condition actuality
      switch (columnCondition)
      {
        case "=":
          if (row.Records[_conditionColumn.Index].Equals(comparableRecord))
            conditionIsActual = true;
          break;
        case ">=":
          if (row.Records[_conditionColumn.Index].CompareTo(comparableRecord) >= 0)
            conditionIsActual = true;
          break;
        case "<=":
          if (row.Records[_conditionColumn.Index].CompareTo(comparableRecord) <= 0)
            conditionIsActual = true;
          break;
        case "<":
          if (row.Records[_conditionColumn.Index].CompareTo(comparableRecord) < 0)
            conditionIsActual = true;
          break;
        case ">":
          if (row.Records[_conditionColumn.Index].CompareTo(comparableRecord) > 0)
            conditionIsActual = true;
          break;
      }

      return conditionIsActual;
    }

    public bool CheckRowCondition(Row row1, string conditionColumnName, string columnCondition, Row row2)
    {
      Column _conditionColumn = FindColumn(conditionColumnName);
      bool conditionIsActual = false;

      UniversalRecord comparableRecord = row2.Records[_conditionColumn.Index];
 
      //check condition actuality
      switch (columnCondition)
      {
        case "=":
          if (row1.Records[_conditionColumn.Index].Equals(comparableRecord))
            conditionIsActual = true;
          break;
        case ">=":
          if (row1.Records[_conditionColumn.Index].CompareTo(comparableRecord) >= 0)
            conditionIsActual = true;
          break;
        case "<=":
          if (row1.Records[_conditionColumn.Index].CompareTo(comparableRecord) <= 0)
            conditionIsActual = true;
          break;
        case "<":
          if (row1.Records[_conditionColumn.Index].CompareTo(comparableRecord) < 0)
            conditionIsActual = true;
          break;
        case ">":
          if (row1.Records[_conditionColumn.Index].CompareTo(comparableRecord) > 0)
            conditionIsActual = true;
          break;
      }

      return conditionIsActual;
    }

    public void DeleteRows(string SingleColumnName, string SingleValue)
    {
      int columnIndex = FindColumn(SingleColumnName).Index;
      for (int i = 0; i < MaxRowIndex; i++)
      {
        Row row = GetRow(i);
        if (row!= null && row.Records[columnIndex].ToString() == SingleValue)
          DeleteRow(row);
      }
    }

    private void SetValue(ref Row r, string columnName, string value)
    {
      Column c = FindColumn(columnName);
      r.Records[c.Index].SetValue(value);
    }

    public void SetValueWithCondition(string columnName, string columnValue, string conditionColumnName, string columnCondition, string conditionValue)
    {
      Column _conditionColumn = FindColumn(conditionColumnName);
      Column _column = FindColumn(columnName);
      for (int i = 0; i < rows.Count; i++)
      {
        Row row = rows[i];
        bool conditionIsActual = false;

        UniversalRecord comparableRecord = null;
        switch (_conditionColumn.Type)
        {
          case DataTypes.Int:
            comparableRecord = new Record<int>(0, conditionValue);
            break;
          case DataTypes.Text:
            comparableRecord = new Record<string>(0, conditionValue);
            break;
          case DataTypes.Bool:
            comparableRecord = new Record<bool>(0, conditionValue);
            break;
          case DataTypes.Double:
            comparableRecord = new Record<double>(0, conditionValue);
            break;
          case DataTypes.Decimal:
            comparableRecord = new Record<decimal>(0, conditionValue);
            break;
        }

        //check condition actuality
        switch (columnCondition)
        {
          case "=":
            if (row.Records[_conditionColumn.Index].Equals(comparableRecord))
            conditionIsActual = true;
            break;
          case ">=":
            if (row.Records[_conditionColumn.Index].CompareTo(comparableRecord) >= 0)
              conditionIsActual = true;
            break;
          case "<=":
            if (row.Records[_conditionColumn.Index].CompareTo(comparableRecord) <= 0)
              conditionIsActual = true;
            break;
          case "<":
            if (row.Records[_conditionColumn.Index].CompareTo(comparableRecord) < 0)
              conditionIsActual = true;
            break;
          case ">":
            if (row.Records[_conditionColumn.Index].CompareTo(comparableRecord) > 0)
              conditionIsActual = true;
            break;
        }  

        if (conditionIsActual)
          SetValue(ref row, columnName, columnValue);
      }
    }
    public UniversalRecord GetRecord(string columnName, int rowIndex)
    {
      Column _column = FindColumn(columnName);
      return rows[rowIndex].Records[_column.Index];
    }
    public List<string> GetColumnNames()
    {
      return columns.Select(c => c.Name).ToList();
    }

    public Row GetRow(int index)
    {
      if (rows.Count > index)
        return rows[index];
      else
        return null;
    }

    public void SwapRows(Row row1, Row row2)
    {
      int newIndex = rows.IndexOf(row2);
      rows.Remove(row1);
      rows.Insert(newIndex, row1);
    }

    public void ClearAllRows()
    {
      rows.Clear();
    }

  }
}
