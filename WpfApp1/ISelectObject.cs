using System.Collections.Generic;

namespace WpfApp1;

public interface ISelectObject : ISelectable
{
    string Text { get; }
    Point[] SelectionBoundsPoints { get; }
    ISelectObject? SelectionParent { get; }

    void SetSelectionPoints(params Point[] points)
    {
        var boundsPoints = SelectionBoundsPoints;
        for (int i = 0; i < points.Length; i++)
        {
            boundsPoints[i] = points[i];
        }

        NotifyRefresh();
    }

    ISelectObject GetSelectRoot()
    {
        var parent = this;
        while (parent.SelectionParent is not null)
        {
            parent = parent.SelectionParent;
        }
        return parent;
    }

    IEnumerable<ISelectObject> GetSelectPath()
    {
        var parent = this;
        while(parent is not null)
        {
            yield return parent;
            parent = parent.SelectionParent;
        }
    }

    ISelectObject? FindPredecessorInPath(ISelectObject? oldSelectedObject)
    {
        var path = GetSelectPath();
        ISelectObject? result = null;
        foreach (var item in path)
        {
            result = item;
            if (item.SelectionParent == oldSelectedObject) break;
        }

        return result;
    }
}


