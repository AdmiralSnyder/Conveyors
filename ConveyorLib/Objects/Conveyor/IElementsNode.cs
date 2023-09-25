namespace ConveyorLib.Objects.Conveyor;

public interface IElementsNode<T>
{
    public LinkedListNode<IPathPart> ElementsNode { get; }
}
