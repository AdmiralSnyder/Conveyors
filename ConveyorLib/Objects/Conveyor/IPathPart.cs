namespace ConveyorLib.Objects.Conveyor;

public interface IPathPart
{
    void RegisterLanes();
    void RebuildLanes();
    void UpdateLengths();
}
