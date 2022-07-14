namespace WpfApp1;

public interface IPathPart
{
    void RegisterLanes();
    void RebuildLanes();

    void UpdateLengths();
}
