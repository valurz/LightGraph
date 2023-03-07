namespace Tests;
using LightGraph.Core;

public class UnitTests
{
    [Fact]
    public void IsConnected()
    {
        var graph = new Graph(10);
        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(1, 4);
        graph.AddEdge(1, 5);
        Assert.True(graph.IsConnected(1, 5));
    }

    [Fact]
    public void IsNotConnected()
    {
        var graph = new Graph(10);
        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(1, 4);
        graph.AddEdge(1, 5);
        graph.AddEdge(1, 8);
        graph.AddEdge(2, 8);

        Assert.False(graph.IsConnected(0, 8));
        Assert.False(graph.IsConnected(0, 3));
    }

    [Fact]
    public void CannotAddNegative()
    {
        var graph = new Graph(10000);
        Assert.Throws<Exception>(() => graph.AddEdge(-1, 0));
        Assert.Throws<Exception>(() => graph.AddEdge(0, -29));
    }

    [Fact]
    public void CannotAddTheSame()
    {
        var graph = new Graph(10000);
        Assert.Throws<Exception>(() => graph.AddEdge(0, 0));
        Assert.Throws<Exception>(() => graph.AddEdge(10, 10));
    }

    [Fact]
    public void CannotAddNegativeWeight()
    {
        var graph = new Graph(10000);
        Assert.Throws<Exception>(() => graph.AddEdge(10, 10, -15));
        Assert.Throws<Exception>(() => graph.AddEdge(10, 10, -1515.15f));
        Assert.Throws<Exception>(() => graph.AddEdge(10, 10, -0.001f));
        Assert.Throws<Exception>(() => new Graph(-5));
        Assert.Throws<Exception>(() => new Graph(0));
    }

    [Fact]
    public void CanRemoveEdge()
    {
        var graph = new Graph(10000);
        graph.AddEdge(0, 5);
        graph.RemoveEdge(0, 5);
        graph.AddEdge(0, 5);

        Assert.True(true);
    }

    [Fact]
    public void CheckDistance()
    {
        var graph = new Graph(10000);
        graph.AddEdge(0, 1);
        graph.AddEdge(0, 2);
        graph.AddEdge(0, 3, 5);
        graph.AddEdge(3, 4, 5);

        Assert.Equal(10, graph.GetRoute(0, 4).Distance);
    }

    [Fact]
    public void CheckShortestRoute1()
    {
        var graph = new Graph(10000);
        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(2, 4, 7);
        graph.AddEdge(3, 4, 5);
        var route = graph.GetRoute(0, 4).Nodes;
        Assert.Equal(0, route[0]);
        Assert.Equal(1, route[1]);
        Assert.Equal(2, route[2]);
        Assert.Equal(3, route[3]);
        Assert.Equal(4, route[4]);
    }

    [Fact]
    public void CheckShortestRoute2()
    {
        var graph = new Graph(8);
        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 4);
        graph.AddEdge(4, 0);
        graph.AddEdge(0, 5);
        graph.AddEdge(5, 6);
        graph.AddEdge(6, 7);
        var route = graph.GetRoute(0, 7).Nodes;
        Assert.Equal(0, route[0]);
        Assert.Equal(5, route[1]);
        Assert.Equal(6, route[2]);
        Assert.Equal(7, route[3]);
    }

    [Fact]
    public void CheckShortestRoute3()
    {
        var graph = new Graph(8);
        graph.AddEdge(0, 1, 500);
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3, 300);
        graph.AddEdge(3, 4);
        graph.AddEdge(4, 0, 600);
        graph.AddEdge(0, 5, 600);
        graph.AddEdge(5, 6, 600);
        graph.AddEdge(6, 7, 600);
        var route = graph.GetRoute(0, 7).Nodes;
        Assert.Equal(0, route[0]);
        Assert.Equal(5, route[1]);
        Assert.Equal(6, route[2]);
        Assert.Equal(7, route[3]);
        Assert.Equal(1800, graph.GetRoute(0, 7).Distance);
    }

    [Fact]
    public void CheckShortestRoutePathChanged4()
    {
        var graph = new Graph(7);
        graph.AddEdge(0, 1);
        graph.AddEdge(0, 3, 5);
        graph.AddEdge(3, 4);
        graph.AddEdge(4, 5);
        graph.AddEdge(5, 2);
        graph.AddEdge(1, 2);
        graph.AddEdge(5, 6);
        var route1Nodes = graph.GetRoute(0, 4).Nodes;
        Assert.Equal(0, route1Nodes[0]);
        Assert.Equal(1, route1Nodes[1]);
        Assert.Equal(2, route1Nodes[2]);
        Assert.Equal(5, route1Nodes[3]);
        Assert.Equal(4, route1Nodes[4]);
        graph.RemoveEdge(0,1);
        graph.AddEdge(0,1,99);
        var route2Nodes = graph.GetRoute(0, 4).Nodes;
        Assert.Equal(0, route2Nodes[0]);
        Assert.Equal(3, route2Nodes[1]);
        Assert.Equal(4, route2Nodes[2]);
    }
}