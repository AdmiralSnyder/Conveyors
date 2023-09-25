// The method bodies, field initializers, and property accessor bodies have been eliminated for brevity.
using System.Collections.Generic;
using Xunit;
using PointDef;


public class QuadTreeTests
{

    [Fact]
    public void Add_AddsItemToListWhenNodesIsEmpty()
    {
        // Arrange
        var maxItemsPerNode = 2;
        var bounds = new QRect(10, 10, 10, 10);

        var item = "testItem";
        var itemBounds = new QRect(15, 15, 2, 2);

        var quadTree = new QuadTree<string>(bounds, maxItemsPerNode);

        // Act
        quadTree.Add(item, itemBounds);

        // Assert
        Assert.Single(quadTree.Items);
        Assert.Equal(item, quadTree.Items[0]);
    }

    [Fact]
    public void Add_CreatesNewNodesWhenMaxItemCountIsExceeded()
    {
        // Arrange
        var maxItemsPerNode = 2;
        var bounds = new QRect(10, 10, 10, 10);

        var items = new List<string>() { "test", "test", "test", "test" };
        var itemBounds = new List<QRect>() {
            new QRect(1, 1, 1, 1),
            new QRect(3, 3, 1, 1),
            new QRect(5, 5, 1, 1),
            new QRect(7, 7, 1, 1)
        };

        var quadTree = new QuadTree<string>(bounds, maxItemsPerNode);

        // Act
        for (int i = 0; i < items.Count; i++)
        {
            quadTree.Add(items[i], itemBounds[i]);
        }

        // Assert
        Assert.Equal(4, quadTree._items.Count);
        Assert.NotNull(quadTree._nodes);
        Assert.Equal(4, quadTree._nodes.Count);
    }

    [Fact]
    public void Query_ReturnsItemsWithinBounds()
    {
        // Arrange
        var maxItemsPerNode = 2;
        var bounds = new QRect(10, 10, 10, 10);

        var items = new List<string>() { "test", "test", "test", "test" };
        var itemBounds = new List<QRect>() {
            new QRect(1, 1, 1, 1),
            new QRect(3, 3, 1, 1),
            new QRect(5, 5, 1, 1),
            new QRect(7, 7, 1, 1)
        };

        var quadTree = new QuadTree<string>(bounds, maxItemsPerNode);

        for (int i = 0; i < items.Count; i++)
        {
            quadTree.Add(items[i], itemBounds[i]);
        }

        // Act
        var results = quadTree.Query(new QRect(0, 0, 10, 10));

        // Assert
        Assert.Equal(4, results.Count);
        Assert.Equal(items, results);
    }

    [Fact]
    public void Query_ReturnsEmptyListWhenBoundsDoNotIntersect()
    {
        // Arrange
        var maxItemsPerNode = 2;
        var bounds = new QRect(10, 10, 10, 10);

        var items = new List<string>() { "test", "test", "test", "test" };
        var itemBounds = new List<QRect>() {
            new QRect(1, 1, 1, 1),
            new QRect(3, 3, 1, 1),
            new QRect(5, 5, 1, 1),
            new QRect(7, 7, 1, 1)
        };

        var quadTree = new QuadTree<string>(bounds, maxItemsPerNode);

        for (int i = 0; i < items.Count; i++)
        {
            quadTree.Add(items[i], itemBounds[i]);
        }

        // Act
        var results = quadTree.Query(new QRect(100, 100, 1, 1));

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Add_AddsToNodesWhenItemIsNotContainedInExistingNodes()
    {
        // Arrange
        var maxItemsPerNode = 1;
        var bounds = new QRect(10, 10, 10, 10);

        var itemBounds1 = new QRect(1, 1, 1, 1);
        var itemBounds2 = new QRect(7, 7, 1, 1);

        var newNode = new QuadTree<string>(itemBounds1, maxItemsPerNode);

        var mockQuadTree = new Mock<QuadTree<string>>(new object[] { bounds, maxItemsPerNode }) { CallBase = true };
        mockQuadTree.Object._nodes.Add(newNode);

        var item = "testItem";

        // Act
        mockQuadTree.Object.Add(item, itemBounds2);

        // Assert
        Assert.Single(newNode._items);
        Assert.Equal(item, newNode._items[0]);
    }

    [Fact]
    public void Add_AddsToExistingNodesWhenItemIsContainedInExistingNodes()
    {
        // Arrange
        var maxItemsPerNode = 2;
        var bounds = new QRect(10, 10, 10, 10);

        var items = new List<string>() { "test", "test" };
        var itemBounds = new List<QRect>() {
            new QRect(5, 5, 1, 1),
            new QRect(6, 6, 1, 1)
        };

        var quadTree = new QuadTree<string>(bounds, maxItemsPerNode);

        for (int i = 0; i < items.Count; i++)
        {
            quadTree.Add(items[i], itemBounds[i]);
        }

        var newNode = quadTree._nodes[0];

        // Act
        quadTree.Add("testItem", new QRect(5, 6, 1, 1));

        // Assert
        Assert.Equal(3, quadTree._items.Count);
        Assert.Single(newNode._items);
        Assert.Equal("testItem", newNode._items[0]);
    }
}
