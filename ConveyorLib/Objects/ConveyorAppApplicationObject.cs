using CoreLib.Definition;
using UILib.Shapes;

namespace ConveyorLib.Objects;

public abstract class ConveyorAppApplicationObject<TThis, TShape, TSource>
    : ConveyorAppApplicationObject<TThis, TShape, SimpleDefinition<TSource>, TSource>
    where TThis : ConveyorAppApplicationObject<TThis, TShape, TSource>, new()
    where TShape : IShape
{
    public TSource Source => Definition.Source;
}

public abstract class ConveyorAppApplicationObject<TThis, TShape, TDefinition, TSource> 
    : CanvasableObject<IConveyorCanvasInfo, ConveyorAppApplication, TShape>
    , IStorable, IStorable<TThis, TSource>
    where TThis : ConveyorAppApplicationObject<TThis, TShape, TDefinition, TSource>, new()
    where TShape : IShape
    where TDefinition : IDefinition<TSource>, new()
{
    protected override void AddToCanvasVirtual(TShape shape) => CanvasInfo.AddToCanvas(shape);
    protected override void SetTag(TShape shape, object tag) => shape.Tag = tag;

    public TDefinition Definition { get; private set; }

    public static TThis Create(TSource source)
    {
        var definition = new TDefinition();
        definition.ApplySource(source);
        return new() { Definition = definition };
    }

    public override bool IsSelectionMatch(Vector point) => Definition.IsSelectionMatch(point);

    public StorageObject Store() => StorageObject.Create(typeof(TThis), Definition.GetSource());
}
