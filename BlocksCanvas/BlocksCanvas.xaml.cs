using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Blocks;
using System.ComponentModel;
using Core;

namespace BlocksCanvas
{
  /// <summary>
  /// Interaction logic for BlocksCanvas.xaml
  /// </summary>
  public partial class BlocksCanvas : UserControl, INotifyPropertyChanged
  {
    private Source _selectedSource = null;
    private Sink _selectedSink = null;

    public event Action<Connection> ConnectionCreated;

    public BlocksCanvas()
    {
      InitializeComponent();
      var converter = blocksListBox.FindResource("MyConverter1") as MyConverter;
      converter.BlocksCanvas = this;
    }

    public ListBox BlocksListBox
    {
      get
      {
        return blocksListBox;
      }
    }


    private void SinksDoubleClicked(object sender, MouseButtonEventArgs e)
    {
      SelectedSink = GetSelectedListBoxItem(sender) as Sink;
    }
    private void SourcesDoubleClicked(object sender, MouseButtonEventArgs e)
    {
      SelectedSource = GetSelectedListBoxItem(sender) as Source;
    }

    public Source SelectedSource
    {
      get
      {
        return _selectedSource;
      }
      set
      {
        if (value != null && value.IsConnected)
        {
          value.Connection.Disconnect();
        }

        if (value == _selectedSource)
        {
          _selectedSource = null;
        }
        else
        {
          _selectedSource = value;
        }
        RaisePropertyChanged("SelectedSource");
        RaisePropertyChanged("SelectedConnector");
        Connect();
      }
    }
    public Sink SelectedSink
    {
      get
      {
        return _selectedSink;
      }
      set
      {
        if (value != null && value.IsConnected)
        {
          value.Connection.Disconnect();
        }

        if (value == _selectedSink)
        {
            _selectedSink = null;
        }
        else
        {
          _selectedSink = value;
        }

        RaisePropertyChanged("SelectedSink");
        RaisePropertyChanged("SelectedConnector");
        
        Connect();
      }
    }
    public Connector SelectedConnector
    {
      get
      {
        if (_selectedSource != null) return _selectedSource;
        if (_selectedSink != null) return _selectedSink;
        return null;
      }
    }

    private void Connect()
    {
      
      

      if (SelectedSink != null && SelectedSource != null)
      {
        if (SelectedSource.Connection != null && SelectedSource.Connection == SelectedSink.Connection)
        {
          SelectedSource.Connection.Disconnect();
          SelectedSink = null;
          SelectedSource = null;
          return;
        }
        if (SelectedSource.Connection != null 
          && SelectedSink.Connection != null 
          && SelectedSink.Connection != SelectedSource.Connection)
        {
          SelectedSource = null;
          SelectedSink = null;
          return;
        }
        if (!Connection.CheckConnectorCompatibility(SelectedSource, SelectedSink))
        {
          _selectedSink = null;
          return;
        }
        var connection = new Connection();
        connection.Sink = SelectedSink;
        connection.Source = _selectedSource;
        SelectedSink = null;
        SelectedSource = null;
        RaiseConnectionCreated(connection);
      }
    }

    private object GetSelectedListBoxItem(object listbox)
    {
      var lb = listbox as ListBox;
      if (lb.SelectedItem == null) return null;
      return  lb.SelectedItem;
    }

    


    protected virtual void RaiseConnectionCreated(Connection c)
    {
      if (ConnectionCreated != null) ConnectionCreated(c);
    }

    protected void RaisePropertyChanged(string propertyName)
    {
      if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler PropertyChanged;
  }
  public class Wrapper : PropertyChangingClassBase
  {
    public BlocksCanvas BlocksCanvas
    {
      get;
      set;
    }
    public Wrapper(Connector c, BlocksCanvas canvas) {
      Connector = c;
      BlocksCanvas = canvas;
      canvas.PropertyChanged += canvas_PropertyChanged;

    }

    void canvas_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "SelectedSource":
        case "SelectedSink":
          ChangeProperties();
          break;
      }
    }

    private void ChangeProperties()
    {
      IsSelected = BlocksCanvas.SelectedConnector == Connector;
      if (BlocksCanvas.SelectedConnector != null)
      {
        if (BlocksCanvas.SelectedConnector is Source && Connector is Sink
           || BlocksCanvas.SelectedConnector is Sink && Connector is Source)
        {
          if (Connection.CheckConnectorCompatibility(BlocksCanvas.SelectedConnector, Connector))
          {
            IsAcceptingConnections = true;
            return;
          }
        }

      }
      IsAcceptingConnections = false;
    }

    private bool _isSelected;
    public bool IsSelected
    {
      get
      {
        return _isSelected;
      }
      set
      {
        if (value == _isSelected) return;
        _isSelected = value;
        RaisePropertyChanged();
      }
    }

    private bool _acceptingConnection;
    public bool IsAcceptingConnections
    {
      get
      {
        return _acceptingConnection;
      }
      set
      {
        if (value == _acceptingConnection) return;
        _acceptingConnection = value;
        RaisePropertyChanged();
      }
    }

    private Connector _connector =null;
    public Connector Connector
    {
      get
      {
        return _connector;
      }
      set
      {
        _connector = value;
        RaisePropertyChanged();
      }
    }
    


  }
  public class MyConverter : IValueConverter
  {
    private ICollection<Wrapper> _wrappers = CollectionFactory.CreateCollection<Wrapper>();

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Connector)
      {
        var wrapper = _wrappers.FirstOrDefault(w => w.Connector == value);
        if (wrapper == null)
        {
          wrapper = new Wrapper(value as Connector, BlocksCanvas);
          _wrappers.Add(wrapper);
        }
        return wrapper;
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public BlocksCanvas BlocksCanvas { get; set; }
  }
  public class MarkingExtender : DependencyObject
  {
    // This is the dependency property we're exposing - we'll 
    // access this as DraggableExtender.CanDrag="true"/"false"
    public static readonly DependencyProperty ConnectorMarkedProperty =
        DependencyProperty.RegisterAttached("ConnectorMarked",
        typeof(bool),
        typeof(MarkingExtender),
        new UIPropertyMetadata(false, OnChangeConnectorMarkedProperty));


    // The expected static setter
    public static void SetConnectorMarked(UIElement element, bool o)
    {
      element.SetValue(ConnectorMarkedProperty, o);
    }

    // the expected static getter
    public static bool GetConnectorMarked(UIElement element)
    {
      return (bool)element.GetValue(ConnectorMarkedProperty);
    }


    private static void OnChangeConnectorMarkedProperty(DependencyObject d,
          DependencyPropertyChangedEventArgs e)
    {

    }

  
  }
}
