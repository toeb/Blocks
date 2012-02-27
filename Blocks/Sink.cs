using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Blocks
{
 

  public abstract class Sink : Connector
  {
    public Sink(Block block):base(block)
    {
 
    }

    [Display]
    public virtual bool Required
    {
      get { return false; }
    }


    protected override void SetBlock(Block block)
    {
      AddSuccessor(block); 
    }

    protected override Block GetBlock()
    {
      return Successors.FirstOrDefault() as Block;
    }


    protected override void SetConnection(Connection connection)
    {
      if (Connection != null)
      {
        RemovePredecessor(Connection);
      }
      if (connection == null) return;
      AddPredecessor(connection);
    }
    protected override Connection GetConnection()
    {
      return Predecessors.FirstOrDefault() as Connection;
    }
  }
}
