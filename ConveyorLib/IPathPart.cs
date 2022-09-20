namespace ConveyorLib;

public interface IPathPart
{
    void RegisterLanes();
    void RebuildLanes();

    void UpdateLengths();
}
