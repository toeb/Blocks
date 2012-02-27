using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Blocks
{
  public class Block : BlocksElement
  {
    private object _content;

    public object Content
    {
      get
      {
        return _content;
      }
    }

    public Block(object o)
    {
      _content = o;    
    }


    public void AddConnector(Connector c)
    {
      if (c is Source)
      {
        AddSuccessor(c);
      }
      if (c is Sink)
      {
        AddPredecessor(c);
      }
    }

    public override string Name
    {
      get
      {
        return _content.GetType().Name;
      }
    }

    public IEnumerable<Connector> Connectors
    {
      get
      {
        return Neighbors.OfType<Connector>();
      }
    }
    public IEnumerable<Sink> Sinks
    {
      get
      {
        var result = Predecessors.OfType<Sink>();
        return result;
      }
    }
    public IEnumerable<Source> Sources
    {
      get
      {
        return Successors.OfType<Source>();
      }
    }

  }
 
 
}
