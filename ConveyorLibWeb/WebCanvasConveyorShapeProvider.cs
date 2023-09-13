using Blazor.Extensions.Canvas.Canvas2D;
using ConveyorLib.Shapes;
using ConveyorLibWeb.Shapes;
using UILib.Shapes;

namespace ConveyorLibWeb;

public class WebCanvasConveyorShapeProvider : ConveyorShapeProvider<WebCanvasEllipse, WebCanvasLine, WebCanvasPath>
{
    public override void AddAdornedShapeLayer(IShape shape) => throw new NotImplementedException();
}
