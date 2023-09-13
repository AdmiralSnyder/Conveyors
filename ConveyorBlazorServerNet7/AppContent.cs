using AutomationLib;
using Blazor.Extensions.Canvas.Canvas2D;
using ConveyorAutomationLib;
using ConveyorLib;
using ConveyorLib.Shapes;
using ConveyorLibWeb;
using ConveyorLibWeb.Shapes;
using CoreLib;
using Microsoft.CodeAnalysis.Scripting;
using ScriptingLib;
using UILib;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;

namespace ConveyorBlazorServerNet7;

public class WebCanvas
{
    public List<WebShape> Children { get; } = new();
}

public class WebCanvasInfo : CanvasInfo<WebCanvas>, IConveyorCanvasInfo
{
    public override TShape AddToCanvas<TShape>(TShape shape)
    {
        Canvas.Children.Add(((WebCanvasShape)(object)shape).BackingShape);

        return shape;
    }

    public override void BeginInvoke<T>(IShape shape, Action<T> action, T value)
    {
        throw new NotImplementedException();
    }

    IConveyorShapeProvider IConveyorCanvasInfo.ShapeProvider
    {
        get => (IConveyorShapeProvider)ShapeProvider;
        set => ShapeProvider = value;
    }

    public override TShape RemoveFromCanvas<TShape>(TShape shape)
    {
        //Canvas.Remove((IAppObject)shape);
        return shape;
    }
}

public class AppContent
{
    public static void Init()
    {
        AutoRoot = ConveyorAutomationObject.CreateAutomationObject(out var context);
        AutoContext = context;
        ShapeProvider = new();

        CanvasInfo = new() { Canvas = new(), ShapeProvider = ShapeProvider };
        AutoRoot.Init(CanvasInfo);

        ScriptRunner.InitializeScriptingEnvironment(AutoRoot, null, null, null, ex =>
        {
            throw ex;
        },
            new[] { typeof(Point), typeof(ConveyorAutomationLib.ConveyorAutomationObject) },
            new[] { typeof(Point), typeof(ConveyorAutomationLib.ConveyorAutomationObject) });
    }

    public static WebCanvasInfo CanvasInfo { get; private set; }

    public static IAutomationContext AutoContext { get; private set; }
    public static IGeneratedConveyorAutomationObject AutoRoot { get; private set; }
    public static WebCanvasConveyorShapeProvider ShapeProvider { get; private set; }

    public static ScriptRunner ScriptRunner { get; private set; } = new();
}
