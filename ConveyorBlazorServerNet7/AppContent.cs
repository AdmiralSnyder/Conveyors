using System.Security.Cryptography.X509Certificates;
using AutomationLib;
using Blazor.Extensions.Canvas.Canvas2D;
using ConveyorAutomationLib;
using ConveyorLibWeb;
using CoreLib;
using Microsoft.CodeAnalysis.Scripting;
using ScriptingLib;
using UILib;

namespace ConveyorBlazorServerNet7;

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
