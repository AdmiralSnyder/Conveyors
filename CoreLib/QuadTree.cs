using PointDef;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib;

public class QuadTree<TElement>
    where TElement : IBounded
{
    public Bounds Bounds { get; set; }
    public bool Extendable { get; set; }
    public int MaxItemsPerNode { get; private set; }

    //public List<TElement> Items { get; } = new List<TElement>();

    // possible optimization: use a linked list for items, so we can remove items from the middle of the list
    public Dictionary<Bounds, List<TElement>> Items = new();
    public IReadOnlyCollection<QuadTree<TElement>> Nodes => _Nodes.AsReadOnly();
    private readonly List<QuadTree<TElement>> _Nodes = new();

    public QuadTree(Bounds bounds, int maxItemsPerNode)
    {
        Bounds = bounds;
        MaxItemsPerNode = maxItemsPerNode;
    }
    public QuadTree()
    {
        Bounds = new Bounds((0, 0), (0, 0));
        Extendable = true;
        MaxItemsPerNode = 4;
    }

    private enum ExtendResults
    {
        NoExtensionNeeded,
        Extended,
        CannotExtend,
    }

    private ExtendResults TryExtend(Bounds bounds)
    {
        // TODO invoke split from here, with the direction we wanna split into

        if (Bounds.Contains(bounds)) return ExtendResults.NoExtensionNeeded;
        
        if (!Extendable) return ExtendResults.CannotExtend; 

        Bounds = Bounds.Union(bounds);
        
        return ExtendResults.Extended;
    }

    public bool Add(TElement item) => (QuadTree<TElement>.ExtendResults)TryExtend(item.Bounds) switch
    {
        ExtendResults.CannotExtend => false,
        ExtendResults.NoExtensionNeeded or ExtendResults.Extended => AddInternal(item),
        _ => throw new NotImplementedException("Missing Case"),
    };

    private bool AddInternal(TElement item)
    {
        if (!_Nodes.Any())
        {
            Items.AddInto(item.Bounds, item);
            if (Items.Keys.Count > MaxItemsPerNode)
            {
                Split();
            }

            return true;
        }

        // Determine which node the item belongs to
        foreach (var node in _Nodes)
        {
            if (node.Bounds.Contains(item.Bounds))
            {
                
                return node.AddInternal(item);
            }
        }

        // Item does not fit into any existing nodes, create a new one
        Items.AddInto(item.Bounds, item);

        // this splitting is an optimization to get finer granularity than 4 leaves
        // if (Items.Keys.Count > MaxItemsPerNode)
        // {
        //     Split();
        // }

        return true;
    }

    public void Remove(TElement item)
    {
        if (!_Nodes.Any())
        {
            Items.RemoveFrom(item.Bounds, item);
            return;
        }

        // Determine which node the item belongs to
        foreach (var node in _Nodes)
        {
            if (node.Bounds.Contains(item.Bounds))
            {
                node.Remove(item);
                break;
            }
        }
    }

    private void Split()
    {
        var subSize = Bounds.Size.Divide(2);

        var x = Bounds.Location.X;
        var y = Bounds.Location.Y;

        _Nodes.Add(new(new((x, y), subSize), MaxItemsPerNode));
        _Nodes.Add(new(new((x + subSize.X, y), subSize), MaxItemsPerNode));
        _Nodes.Add(new(new((x, y + subSize.Y), subSize), MaxItemsPerNode));
        _Nodes.Add(new(new((x + subSize.X, y + subSize.Y), subSize), MaxItemsPerNode));

        foreach (var item in Items.ToList())
        {
            // TODO Perf: unroll this loop with locals
            foreach (var node in _Nodes)
            {
                if (node.Bounds.Contains(item.Key))

                {
                    // TODO perf
                    foreach (var itemValue in item.Value)
                    {
                        node.Add(itemValue);
                    }
                    Items.Remove(item.Key);
                    break;
                }
            }
        }
    }

    public List<TElement> Query(Bounds queryBounds)
    {
        var results = new List<TElement>();

        // Check if the query bounds intersect with the quadtree bounds
        if (!Bounds.Intersects(queryBounds))
        {
            return results;
        }

        // Add items from nodes that intersect with the query bounds
        foreach (var item in Items)
        {
            if (item.Key.Contains(queryBounds))
            {
                results.AddRange(item.Value);
            }
        }

        foreach (var node in _Nodes)
        {
            results.AddRange(node.Query(queryBounds));
        }

        return results;
    }
}


public interface IBounded
{
    Bounds Bounds { get; }
}