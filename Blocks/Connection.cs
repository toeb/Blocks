using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Blocks
{
  public class Connection : BlocksElement
  {
    private static int counter = 1;
    private int number;
    private Action<Source> sourceDataReadyAction;

    public Connection()
    {
      number = counter++;
      sourceDataReadyAction = sourceDataReady;
    }


    private void sourceDataReady(Source obj)
    {
      if (Sink == null) return;
      if (Source == null) return;
      if (!Source.IsDataReady) return;
      Sink.Value = Source.Value;
    }


    public override string Name
    {
      get
      {
        return ""+number;
      }
    }

    public void Disconnect()
    {
      Source.Connection = null;
      Sink.Connection = null;
    }

    public static bool CheckConnectorCompatibility(Connector c1, Connector c2)
    {
      var sink = c1 as Sink ?? c2 as Sink ;
      var source = c1 as Source ?? c2 as Source;

      if (source == null) return true;
      if (sink == null) return true;
      return source.Type == sink.Type || source.Type.IsSubclassOf(sink.Type);
    }

    [Display]
    public Source Source
    {
      get
      {
        return Predecessors.FirstOrDefault() as Source;
      }
      set
      {
        Contract.Assume(CheckConnectorCompatibility(value, Sink));
        if (Source == value) return;
        
        if (Predecessors.Count() > 0)
        {
          Source.DataReady -= sourceDataReadyAction;
          RemovePredecessor(Source);
        }
        if (value == null) return;
        value.DataReady += sourceDataReadyAction;
        AddPredecessor(value);
        sourceDataReady(Source);
      }
    }
    [Display]
    public Sink Sink
    {
      get
      {
        return Successors.FirstOrDefault() as Sink;
      }
      set
      {
        Contract.Assume(CheckConnectorCompatibility(Source,value));
        if (Sink == value) return;
        if (Successors.Count() > 0)
        {
          RemoveSuccessor(Successors.First());
        }
        if (value == null) return;
        AddSuccessor(value);
        sourceDataReady(Source);
      }
    }




  }
}
