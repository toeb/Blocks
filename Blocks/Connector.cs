using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Blocks
{

  public abstract class Connector : BlocksElement
  {
    public Connector(Block block)
    {
      Contract.Assume(block != null);
      Block = block;
    }

    #region abstract members
    abstract protected void SetBlock(Block block);
    abstract protected Block GetBlock();
    abstract protected void SetConnection(Connection connection);
    abstract protected Connection GetConnection();
    [Display]
    public abstract object Value
    {
      get;
      set;
    }
    [Display]
    public abstract Type Type
    {
      get;
    }
    #endregion

    [Display]
    public bool IsConnected
    {
      get
      {
        return Connection != null;
      }
    } 

    [Display]
    public Connection Connection
    {
      get
      {
        return GetConnection();
      }
      set
      {
        if (value == Connection) return;
        SetConnection(value);
        RaisePropertyChanged();
      }
    }
    [Display]
    public Block Block
    {
      get
      {
        return GetBlock();
      }
      set
      {
        if (value == Block) return;
        SetBlock(value);
        RaisePropertyChanged();
      }
    }

    private void ElementChanged(BlocksElement blocksElement)
    {
      if (blocksElement is Block)
      {
        RaisePropertyChanged("Block");
        return;
      }
      if(blocksElement is Connection)
      {
        RaisePropertyChanged("Connection");
        return;
      }
    }

    protected override void StorePredecessor(BlocksElement predecessor)
    {
      base.StorePredecessor(predecessor);
      ElementChanged(predecessor);
    }
    protected override void StoreSuccessor(BlocksElement successor)
    {
      base.StoreSuccessor(successor);
      ElementChanged(successor);
    }

    protected override void DeletePredecessor(BlocksElement predecessor)
    {
      base.DeletePredecessor(predecessor);
      ElementChanged(predecessor);
    }

    protected override void DeleteSuccessor(BlocksElement successor)
    {
      base.DeleteSuccessor(successor);
      ElementChanged(successor);
    }

    protected override void OnPropertyChanged(string name)
    {
      base.OnPropertyChanged(name);
      switch (name)
      {
        case "Connection":
          RaisePropertyChanged("IsConnected");
          break;
      }
    }
  }
}
