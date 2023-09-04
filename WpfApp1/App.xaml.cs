using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ConveyorLib.Wpf;
using WpfLib;

namespace ConveyorApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UIHelpers.Instance = new UIHelpersInstanceWpf();
            ShapeFunc.Instance = new ShapeFuncInstanceWpf();
            GeometryProvider.Instance = new GeometryProviderInstanceWpf();
        }
    }
}
