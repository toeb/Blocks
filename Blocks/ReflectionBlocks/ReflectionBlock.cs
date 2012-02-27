using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;

namespace Blocks
{
  public class ReflectionBlock : Block
  {
    public ReflectionBlock(object o)
      : base(o)
    {
      var infos = Utility.GetPropertyInfosWithAttribute(o.GetType(), typeof(SinkAttribute));
      foreach (var info in infos)
      {
        AddConnector(new PropertySink(this, info));
      }

      infos = Utility.GetPropertyInfosWithAttribute(o.GetType(), typeof(SourceAttribute));
      foreach (var info in infos)
      {
        AddConnector(new PropertySource(this, info));
      }
    }
  }
}
