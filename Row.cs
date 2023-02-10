using System.Collections.Generic;
using System;

namespace RealEstateApp {
[Serializable]
  public class Row
  {
    public int Index { get; }
    public List<UniversalRecord> Records { get; set; }
    public Row(int index)
    {
      Index = index;
      Records = new List<UniversalRecord>();
    }
  }

}
