using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Core;
using System.Reflection;

namespace Blocks
{
  public class PropertySink : Sink
  {
    private SinkAttribute _inputAttribute;
    private PropertyInfo _info;

    public PropertySink(Block block, PropertyInfo info)
      : base(block)
    {
      Contract.Assume(info.CanWrite == true);
      _info = info;
      _inputAttribute = info.GetCustomAttributes(typeof(SinkAttribute), true).FirstOrDefault() as SinkAttribute;

    }

    public override bool Required
    {
      get { return _inputAttribute.Required; }
    }

    public override object Value
    {
      get
      {
        if (!_info.CanRead) return null;
        return _info.GetValue(Block.Content, null);
      }
      set
      {
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
