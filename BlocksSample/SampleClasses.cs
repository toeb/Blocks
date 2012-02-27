using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.ComponentModel.DataAnnotations;
using Blocks;
using System.Windows;


namespace BlocksSample
{

  public class SimpleSink : PropertyChangingClassBase
  {
    private string str;
    [Sink]
    public string Value
    {
      set { str = value; RaisePropertyChanged(); }
      get { return str; }
    }
  }
  public class Multiplexer : PropertyChangingClassBase
  {
    private string str;
    [Source]
    [Sink]
    public string Value
    {
      get
      {
        return str;
      }
      set
      {
        str = value;
        RaisePropertyChanged();
        RaisePropertyChanged("Value2");
      }
    }
    [Source]
    public string Value2
    {
      get
      {
        return Value;
      }
    }
  }

  public class StringSource : PropertyChangingClassBase
  {
    private string str1;
    [Source]
    public string Value
    {
      get { return str1; }
      set { str1 = value; RaisePropertyChanged(); }
    }
   
  }
  public class IntAddition : PropertyChangingClassBase
  {
    int a, b;
    [Sink]
    public int SummandA
    {
      get
      {
        return a;
      }
      set
      {
        if (value == a) return;
        a = value;
        RaisePropertyChanged();
        RaisePropertyChanged("Summe");
      }
    }
    [Sink]
    public int SummandB
    {
      get
      {
        return b;
      }
      set
      {
        if (value ==b) return;
        b = value;
        RaisePropertyChanged();
        RaisePropertyChanged("Summe");
      }
    }
    [Source]
    public int Summe
    {
      get
      {
        return SummandA + SummandB;
      }
    }
  }
  public class String2IntConverter : PropertyChangingClassBase
  {
    private string str;
    private int intVal;
    [Sink]
    [Source]
    [Required]
    public string StringValue
    {
      get
      {
        return str; 
      }
      set{
        str = value;
        RaisePropertyChanged();
        if (int.TryParse(str, out intVal))
        {
          RaisePropertyChanged("IntValue");
        }
      }
    }
    [Sink]
    [Source]
    public int IntValue
    {
      get
      {
        return intVal;
      }
      set
      {
        intVal = value;
        str = intVal.ToString();
        RaisePropertyChanged();
        RaisePropertyChanged("StringValue");
      }
    }
  }

  public class ObjectToMessageBox
  {
    [Sink]
    public object Value
    {
      set
      {
        if (value == null) MessageBox.Show("Null Value");
        else
        MessageBox.Show(value.ToString());
      }
    }
  }

  public class SampleClasses
  {
    static ICollection<object> _items = null;
    public static ICollection<object> Items
    {
      get
      {
        if (_items == null)
        {
          _items = CollectionFactory.CreateCollection<object>();
          Items.Add(new ReflectionBlock(new ObjectToMessageBox()));
          Items.Add(new ReflectionBlock(new SimpleSink()));
          Items.Add(new ReflectionBlock(new StringSource()));
          Items.Add(new ReflectionBlock(new SimpleSink()));
          Items.Add(new ReflectionBlock(new StringSource()));
          Items.Add(new ReflectionBlock(new SimpleSink()));
          Items.Add(new ReflectionBlock(new Multiplexer()));
          Items.Add(new ReflectionBlock(new String2IntConverter()));
          Items.Add(new ReflectionBlock(new String2IntConverter())); 
          Items.Add(new ReflectionBlock(new String2IntConverter()));
          Items.Add(new ReflectionBlock(new IntAddition()));
        }
        return _items;
      }
    }
  }
}
