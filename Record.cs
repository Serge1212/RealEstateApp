using System.Diagnostics.CodeAnalysis;
using System;

namespace RealEstateApp {
  public enum DataTypes
  {
    Int,
    Text,
    Bool,
    Double,
    Decimal
  }

  [Serializable]
  public abstract class UniversalRecord : IComparable, IComparable<UniversalRecord>
  {
    public int ColumnIndex { get; protected set; }
    public DataTypes Type { get; protected set; }
    public override string ToString() => throw new InvalidOperationException();
    public abstract void SetValue(int Value);
    public abstract void SetValue(string Value);
    public abstract void SetValue(bool Value);
    public abstract void SetValue(double Value);
    public abstract void SetValue(decimal Value);
    public abstract void GetValue(out string Value);
    public abstract void GetValue(out int Value);
    public abstract void GetValue(out bool Value);
    public abstract void GetValue(out double Value);
    public abstract void GetValue(out decimal Value);
    public abstract int CompareTo(object obj);
    public abstract int CompareTo([AllowNull] UniversalRecord other);
    public override bool Equals(object obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
  }

[Serializable]
  public class Record<T> : UniversalRecord where T :IComparable, IEquatable<T>
  {
    private T value;

    public override void SetValue(int Value)
    {
      if (!(value is int))
        throw new InvalidOperationException();
      else
        value = (T)Convert.ChangeType(Value,typeof(T));
    }

    public override void SetValue(string Value)
    {
      var t = GetType().GetGenericArguments()[0];
      if (t == typeof(string))
        value = (T)Convert.ChangeType(Value, typeof(T));
      else if (t == typeof(int) && int.TryParse(Value, out int intValue))
        value = (T)Convert.ChangeType(intValue, typeof(T));
      else if (t == typeof(bool) && bool.TryParse(Value, out bool boolValue))
        value = (T)Convert.ChangeType(boolValue, typeof(T));
      else if (t == typeof(double) && double.TryParse(Value, out double doubleValue))
        value = (T)Convert.ChangeType(doubleValue, typeof(T));
      else if (t == typeof(decimal) && decimal.TryParse(Value, out decimal decimalValue))
        value = (T)Convert.ChangeType(decimalValue, typeof(T));
    }

    public override void SetValue(bool Value)
    {
      if (!(value is bool))
        throw new InvalidOperationException();
      else
        value = (T)Convert.ChangeType(Value, typeof(T));
    }

    public override void SetValue(double Value)
    {
      if (!(value is double))
        throw new InvalidOperationException();
      else
        value = (T)Convert.ChangeType(Value, typeof(T));
    }

    public override void SetValue(decimal Value)
    {
      if (!(value is decimal))
        throw new InvalidOperationException();
      else
        value = (T)Convert.ChangeType(Value, typeof(T));
    }


    public override void GetValue(out string Value)
    {
      Value = value.ToString();
    }

    public override void GetValue(out int Value)
    {
      int? v = value as int?;
      if (v == null)
        throw new InvalidOperationException();
      else
      Value = (int)v;
    }
    public override void GetValue(out bool Value)
    {
      bool? v = value as bool?;
      if (v == null)
        throw new InvalidOperationException();
      else
        Value = (bool)v;
    }
    public override void GetValue(out double Value)
    {
      double? v = value as double?;
      if (v == null)
        throw new InvalidOperationException();
      else
        Value = (double)v;
    }
    public override void GetValue(out decimal Value)
    {
      decimal? v = value as decimal?;
      if (v == null)
        throw new InvalidOperationException();
      else
        Value = (decimal)v;
    }

    public override int CompareTo(object obj)
    {
      return obj is Record<T> r ? value.CompareTo(r.value) : 0;
    }
    public override int CompareTo([AllowNull] UniversalRecord other)
    {
      return other is Record<T> r ? value.CompareTo(r.value) : 0;
    }

    public override bool Equals(Object obj)
    {
      //Check for null and compare run-time types.
      if ((obj == null) || !this.GetType().Equals(obj.GetType()))
      {
        return false;
      }
      else
      {
        Record<T> r = (Record<T>)obj;
        return value.CompareTo(r.value) == 0;
      }
    }

    public override int GetHashCode()
    {
      return value.GetHashCode();
    }


    public override string ToString() => value.ToString();

    public Record(int column, T Value)
    {
      ColumnIndex = column;
      value = Value;
    }

    public Record(int column, string Value)
    {
      ColumnIndex = column;
      SetValue(Value);
    }
  }
}
