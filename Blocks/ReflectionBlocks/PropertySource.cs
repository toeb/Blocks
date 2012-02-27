using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.Reflection;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blocks
{
  public class PropertySource : Source
  {
    private SourceAttribute _outputAttribute = null;
    private PropertyInfo _info;

    public PropertySource(Block block, PropertyInfo info)
      : base(block)
    {
      Contract.Assume(info.CanRead == true);

      _outputAttribute = info.GetCustomAttributes(typeof(SourceAttribute), true).FirstOrDefault() as SourceAttribute;
      _info = info;
      IsDataReady = true;
      var inpc = block.Content as INotifyPropertyChanged;
      if (inpc == null) return;
      inpc.PropertyChanged += UnderlyingPropertyChanged;
      IsDataReady = true;
    }

    void UnderlyingPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == PropertyName)
      {
        RaisePropertyChanged("Value");
        RaiseDataReady();
      }
    }
    [Display]
    public string PropertyName
    {
      get
      {
        return _info.Name;
      }
    }

    public override object Value
    {
      get
      {
        
        return _info.GetValue(Block.Content, null);
      }
      set
      {
        if (_info.CanWrite) return;
        _info.SetValue(Block.Content, value, null);
      }
    }

    public override Type Type
    {
      get { return _info.PropertyType; }
    }

    public override string Name
    {
      get
      {
        return _info.Name;
      }
    }
  }
}
