using CoreLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UILib;

namespace ConveyorApp
{
    /// <summary>
    /// Interaction logic for PropertyGrid.xaml
    /// </summary>
    public partial class PropertyGrid : UserControl, IRefreshListener<IRefreshable>
    {
        public PropertyGrid()
        {
            InitializeComponent();
            RefreshManager<IRefreshable>.RegisterRefreshListener(this);
        }

        public static readonly DependencyProperty InspectedObjectProperty = 
            DependencyProperty.Register(nameof(InspectedObject), typeof(object), typeof(PropertyGrid), new UIPropertyMetadata(null, InspectedObjectPropertyChanged));

        public object? InspectedObject
        {
            get => GetValue(InspectedObjectProperty);
            set => SetValue(InspectedObjectProperty, value);
        }

        private static void InspectedObjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PropertyGrid)d).ApplyInspectedObject(e.NewValue);

        private void ApplyInspectedObject(object? obj)
        { 
            if (obj is null)
            {
                Rows = EmptyRows;
            }
            else
            {
                Rows = RowInfosByType.GetOrAdd(obj.GetType(), InitTypeRowList); // TODO GetOrAdd
            }
            ApplyRows();
            LoadObject(obj);
        }

        private static List<PropertyGridRowInfo> InitTypeRowList(Type type)
        {
            List<PropertyGridRowInfo> result = new();

            foreach (var property in type.GetProperties())
            {
                // TODO filter properties based on attributes
                PropertyGridRowInfo entry = new(property);
                result.Add(entry);
            }

            return result;
        }

        private void ApplyRows()
        {
            TheGrid.Children.Clear();
            TheGrid.RowDefinitions.Clear();
            int rowIdx = 0;

            if (InspectedObject is not null)
            {
                TheGrid.RowDefinitions.Add(new() { MinHeight = 20d});
                TextBlock typeLabel = new() { Text = InspectedObject.GetType().Name, Background = Brushes.Salmon, ToolTip = InspectedObject.GetType().Name };
                TheGrid.Children.Add(typeLabel);
                typeLabel.SetValue(Grid.RowProperty, rowIdx);
                typeLabel.SetValue(Grid.ColumnProperty, 0);
                typeLabel.SetValue(Grid.ColumnSpanProperty, 3);

                rowIdx++;
            }

            foreach (var row in Rows)
            {
                TheGrid.RowDefinitions.Add(new() { MinHeight = 20d });

                TheGrid.Children.Add(row.Label);
                row.Label.SetValue(Grid.RowProperty, rowIdx);
                row.Label.SetValue(Grid.ColumnProperty, 0);
                
                TheGrid.Children.Add(row.ValueControl);
                row.ValueControl.SetValue(Grid.RowProperty, rowIdx);
                row.ValueControl.SetValue(Grid.ColumnProperty, 1);
                
                TheGrid.Children.Add(row.TypeTextControl);
                row.TypeTextControl.SetValue(Grid.RowProperty, rowIdx);
                row.TypeTextControl.SetValue(Grid.ColumnProperty, 2);

                rowIdx++;
            }
        }

        WeakReference<object> ObjRef;

        private void LoadObject(object? obj)
        {
            if (ObjRef is { } objRef && objRef.TryGetTarget(out var oldObj) && !Equals(obj, oldObj))
            {
                RefreshManager<IRefreshable>.UnRegisterObserver(this, oldObj);
            }

            if (obj is not null)
            {
                RefreshObjectData(obj);

                RefreshManager<IRefreshable>.RegisterObserver(this, obj);
                ObjRef = new(obj);
            }
        }

        private void RefreshObjectData(object? obj)
        {
            foreach (var row in Rows)
            {
                var propertyValue = row.PropertyInfo.GetValue(obj);

                // CHEATING for now
                ((TextBox)row.ValueControl).Text = propertyValue?.ToString() ?? "<NULL>";
                ((TextBox)row.ValueControl).ToolTip = propertyValue?.ToString() ?? "<NULL>";
                ((TextBox)row.ValueControl).TextChanged += (s, e) =>
                {
                    // TODO add more types
                    if (row.PropertyInfo.PropertyType == typeof(double) && double.TryParse(((TextBox)s).Text, out var val) && row.PropertyInfo.CanWrite)
                    {
                        row.PropertyInfo.SetValue(obj, val);
                    }
                };
            }
        }

        private class PropertyGridRowInfo
        {
            public PropertyGridRowInfo(PropertyInfo propertyInfo)
            {
                PropertyInfo = propertyInfo;
                Label = new() { Text = PropertyInfo.Name, Background = Brushes.Gray, ToolTip = PropertyInfo.Name};
                ValueControl = CreateValueControl(PropertyInfo.PropertyType);
                TypeTextControl = new() { Text = PropertyInfo.PropertyType.Name, ToolTip = PropertyInfo.PropertyType.Name};
            }

            public PropertyInfo PropertyInfo { get; set; }
            public TextBlock Label { get; set; }
            public int Index { get; set; }
            public FrameworkElement ValueControl { get; set; }
            public TextBlock TypeTextControl { get; set; }
        }

        private static FrameworkElement CreateValueControl(Type propertyType)
        {
            // CHEATING for now
            return new TextBox();
        }

        public void Notify(IRefreshable obj)
        {
            RefreshObjectData(obj);
        }

        private List<PropertyGridRowInfo> Rows { get; set; } = EmptyRows;
        private static readonly List<PropertyGridRowInfo> EmptyRows = new();

        private static readonly Dictionary<Type, Control> ControlResolvers = new();

        private readonly Dictionary<Type, List<PropertyGridRowInfo>> RowInfosByType = new();
    }
}
