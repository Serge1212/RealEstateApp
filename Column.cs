using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp {
  [Serializable]
  public class Column {
    public string Name { get; set; }
    public DataTypes Type { get; set; }
    public int Index { get; }

    public Column (string name, string type, int index)
    {
      Name = name;
      Type = (DataTypes)Enum.Parse(typeof(DataTypes), type);
      Index = index;
    }

  }
}
