using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.Reflection;
using System.Diagnostics.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Blocks
{

  

  public abstract class Source : Connector
  {

    public event Action<Source> DataReady;
    private bool _dataReady;

    protected void RaiseDataReady()
    {
      if (DataReady != null) DataReady(this);
    }



    [Display]
    public bool IsDataReady
    {
      get
      {
        return _dataReady;
      }
      set
      {
        if (IsDataReady == value) return;
        _dataReady = value;
        RaisePropertyChanged();
        RaiseDataReady();
      }
    }


    public Source(Block parent)
      : base(parent)
    {

    }

    protected override void SetBlock(Block block)
    {
      AddPredecessor(block);
    }
    protected override Block GetBlock()
    {
      return Predecessors.FirstOrDefault() as Block;
    }
    protected override void SetConnection(Connection connection)
    {
      if (Connection != null)
      {
        RemoveSuccessor(Connection);
      }
      if (connection == null) return;
      AddSuccessor(connection);
    }
    protected override Connection GetConnection()
    {
      return Successors.FirstOrDefault() as Connection;
    }


  }



}
