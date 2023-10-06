using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ConveyorAutomationLib;
using ConveyorLib.Shapes;
using ConveyorLib.Wpf;
using UILib.Behaviors;

using WpfLib;
using WpfLib.Behaviors;

namespace ConveyorApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        UIHelpers.Instance = new UIHelpersInstanceWpf();
        MouseBehaviorManager.Instance = new MouseBehaviorManagerWpf();
        GeometryProvider.Instance = new GeometryProviderInstanceWpf();
        SelectBehaviorProvider.Instance = new SelectBehaviorProviderInstanceWpf();

        ShapeProvider = new WpfConveyorShapeProvider();

    }
}
