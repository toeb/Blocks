using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Core;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blocks
{
  [DebuggerDisplay("BlocksElement {Name}")]
  public class BlocksElement : NodeBase<BlocksElement>, INotifyPropertyChanged
  {
    [Display]
    public virtual string Description { get { return ""; } }

    public virtual string Name
    {
      get
      {
        return this.GetType().Name;
      }
    }

    #region INotifyPropertyChanged
    protected void RaisePropertyChanged()
    {
      var frame = new StackFrame(1);
      var propertyName = frame.GetMethod().Name.Substring(4);
      RaisePropertyChanged(propertyName);
    }
    protected void RaisePropertyChanged(string name)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(name));
        OnPropertyChanged(name);
      }
    }

    protected virtual void OnPropertyChanged(string name) { }

    public event PropertyChangedEventHandler PropertyChanged;
    #endregion
  }


 

 
}
