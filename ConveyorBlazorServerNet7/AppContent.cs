using System.Security.Cryptography.X509Certificates;
using AutomationLib;
using Blazor.Extensions.Canvas.Canvas2D;
using ConveyorAutomationLib;
using ConveyorLib;
using ConveyorLib.Shapes;
using ConveyorLibWeb;
using ConveyorLibWeb.Shapes;
using CoreLib;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.Scripting;
using ScriptingLib;
using UILib;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;

namespace ConveyorBlazorServerNet7;

public class WebCanvas
{
    public List<WebShape> Children { get; } = new();

    public event Action<EventArgs> MouseDown;
    public event Action<EventArgs> MouseMove;

    public void DoMouseClick(MouseEventArgs args) => MouseDown?.Invoke(args);

    public void DoMouseMove(MouseEventArgs args) => MouseMove?.Invoke(args);
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
        Canvas.Children.Remove(((WebCanvasShape)(object)shape).BackingShape);
        return shape;
    }
}

public class AppContent
{
    public static void Init()
    {
        AutoRoot = ConveyorAutomationObject.CreateAutomationObject(out var context);
        AutoContext = context;
        ShapeProvider = new WebCanvasConveyorShapeProvider();


        CanvasInfo = new() { ShapeProvider = ShapeProvider };
        AutoRoot.Init(CanvasInfo);

        InputContext = new();

        ScriptRunner.InitializeScriptingEnvironment(AutoRoot, null, null, null, ex =>
        {
            throw ex;
        },
            new[] { typeof(Point), typeof(ConveyorAutomationLib.ConveyorAutomationObject) },
            new[] { typeof(Point), typeof(ConveyorAutomationLib.ConveyorAutomationObject) });

        Canvas = new();
    }

    private static WebCanvas _Canvas;
    public static WebCanvas Canvas
    {
        get =>_Canvas;
        set
        {
            _Canvas = value;
            CanvasInfo.Canvas = value;
            InputContext.Canvas = CanvasInfo;
        }
    }

    public static WebCanvasInputContext InputContext { get; private set; }

    public static WebCanvasInfo CanvasInfo { get; private set; }

    public static IAutomationContext AutoContext { get; private set; }

    public static ScriptRunner ScriptRunner { get; private set; } = new();
}
