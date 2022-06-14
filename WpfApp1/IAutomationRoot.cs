using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfLib;

namespace WpfApp1
{
    public interface IAutomationRoot
    {
        public void Init(object obj);
    }

    public interface IGeneratedConveyorAutomationObject: IAutomationRoot
    {
        List<Conveyor> Conveyors { get; }
        CanvasInfo CanvasInfo { get; }
        Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes);
    }

    public interface IAutomationContext
    {
        bool IsAutomated { get; set; }
        public Action<string> LogAction { get; set; }
    }

    public partial class ConveyorAutomationObject : IAutomationRoot, IGeneratedConveyorAutomationObject
    {
        public List<Conveyor> Conveyors { get; } = new();
        public CanvasInfo CanvasInfo { get; private set; }
        
        [Generated]
        public void Init(object obj)
        {
            var tuple = ((Canvas Canvas, ConveyorShapeProvider ShapeProvider))obj;
            CanvasInfo = new() { Canvas = tuple.Canvas, ShapeProvider = tuple.ShapeProvider};
        }

        public Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes)
        {
            var conv = Conveyor.Create(points, isRunning, lanes);
            Conveyor.AddToCanvas(conv, CanvasInfo);
            Conveyors.Add(conv);
            return conv;
        }

        [Generated]
        public static IGeneratedConveyorAutomationObject CreateAutomationObject() => CreateAutomationObject(out _);

        [Generated]
        public static IGeneratedConveyorAutomationObject CreateAutomationObject(out IAutomationContext context)
        {
            var result = new AutomationConveyorAutomationObject<ConveyorAutomationObject>();
            context = result;
            return result;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GeneratedAttribute : Attribute{ }

    [Generated]
    public partial class AutomationConveyorAutomationObject<TWrapper> : IGeneratedConveyorAutomationObject, IAutomationRoot, IAutomationContext
        where TWrapper : IGeneratedConveyorAutomationObject, IAutomationRoot, new()
    {
        private TWrapper AutomationObject { get; }

        public List<Conveyor> Conveyors => AutomationObject.Conveyors;
        public CanvasInfo CanvasInfo => AutomationObject.CanvasInfo;
        public Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes
            //, [CallerArgumentExpression("points")] string pointsArg = null
            //, [CallerArgumentExpression("isRunning")] string isRunningArg = null
            //, [CallerArgumentExpression("lanes")] string lanesArg = null
            )
        {
            if (!IsAutomated)
            {
                LogAction?.Invoke($"$.AddConveyor({points.Out()}, {isRunning.Out()}, {lanes.Out()})");
            }
            return AutomationObject.AddConveyor(points, isRunning, lanes);
        }

        public bool IsAutomated { get; set; }
        public Action<string> LogAction { get ; set; }

        public void Init(object obj) => AutomationObject.Init(obj);

        public AutomationConveyorAutomationObject() => AutomationObject = new();
    }

    public static class CSharpOutputHelpers
    {
        public static string Out<T>(this IEnumerable<T> items) => $"new {typeof(T).Name}[]{{{string.Join(", ", items.Select(i => i.Out()))}}}";

        public static string Out(this bool obj) => obj ? "true" : "false";

        public static string Out(this object? obj) => obj?.ToString() ?? "null";
    }
}
