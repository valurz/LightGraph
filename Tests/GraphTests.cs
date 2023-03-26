namespace Tests;

using System.Diagnostics;
using LightGraph.Core;

public class GraphTests
{
    [Fact]
    public void Nodes_are_connected()
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
    public void Nodes_are_not_connected()
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
    public void Negative_nodes_cannot_be_added()
    {
        var graph = new Graph(10000);

        Assert.Throws<Exception>(() => graph.AddEdge(-1, 0));
        Assert.Throws<Exception>(() => graph.AddEdge(0, -29));
    }

    [Fact]
    public void Nodes_cannot_be_connected_to_themselves()
    {
        var graph = new Graph(10000);
        Assert.Throws<Exception>(() => graph.AddEdge(0, 0));
        Assert.Throws<Exception>(() => graph.AddEdge(10, 10));
    }

    [Fact]
    public void Nodes_with_negative_weight_cannot_be_added()
    {
        var graph = new Graph(10000);
        Assert.Throws<Exception>(() => graph.AddEdge(10, 10, -15));
        Assert.Throws<Exception>(() => graph.AddEdge(10, 10, -1515.15f));
        Assert.Throws<Exception>(() => graph.AddEdge(10, 10, -0.001f));
        Assert.Throws<Exception>(() => new Graph(-5));
        Assert.Throws<Exception>(() => new Graph(0));
    }

    [Fact]
    public void Edge_can_be_removed()
    {
        var graph = new Graph(10000);

        graph.AddEdge(0, 5);
        graph.RemoveEdge(0, 5);
        graph.AddEdge(0, 5);

        Assert.True(true);
    }

    [Fact]
    public void Distance_matches_in_small_graph()
    {
        var graph = new Graph(300);

        graph.AddEdge(0, 1);
        graph.AddEdge(0, 2);
        graph.AddEdge(0, 3, 5);
        graph.AddEdge(3, 4, 5);

        Assert.Equal(10, graph.GetRoute(0, 4).distance);
    }

    [Fact]
    public void Check_route1()
    {
        var graph = new Graph(10000);
        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(2, 4, 7);
        graph.AddEdge(3, 4, 5);

        var route = graph.GetRoute(0, 4).nodes;

        Assert.Equal(0, route.ElementAt(0));
        Assert.Equal(1, route.ElementAt(1));
        Assert.Equal(2, route.ElementAt(2));
        Assert.Equal(3, route.ElementAt(3));
        Assert.Equal(4, route.ElementAt(4));
    }

    [Fact]
    public void Check_route2()
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

        var route = graph.GetRoute(0, 7).nodes;

        Assert.Equal(0, route.ElementAt(0));
        Assert.Equal(5, route.ElementAt(1));
        Assert.Equal(6, route.ElementAt(2));
        Assert.Equal(7, route.ElementAt(3));
    }


    [Fact]
    public void Check_route_and_distance()
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

        var route = graph.GetRoute(0, 7).nodes;

        Assert.Equal(0, route.ElementAt(0));
        Assert.Equal(5, route.ElementAt(1));
        Assert.Equal(6, route.ElementAt(2));
        Assert.Equal(7, route.ElementAt(3));
        Assert.Equal(1800, graph.GetRoute(0, 7).distance);
    }

    [Fact]
    public void Check_route_with_decimal_weights()
    {
        var graph = new Graph(10000);
        graph.AddEdge(0, 1, 1.0f);
        graph.AddEdge(0, 2, 1.5f);
        graph.AddEdge(2, 3, 1.2f);
        graph.AddEdge(1, 3, 1.0f);

        var route = graph.GetRoute(0, 3).nodes;

        Assert.Equal(0, route.ElementAt(0));
        Assert.Equal(1, route.ElementAt(1));
        Assert.Equal(3, route.ElementAt(2));
    }

    [Fact]
    public void Check_route_on_a_changed_path()
    {
        var graph = new Graph(7);
        graph.AddEdge(0, 1);
        graph.AddEdge(0, 3, 5);
        graph.AddEdge(3, 4);
        graph.AddEdge(4, 5);
        graph.AddEdge(5, 2);
        graph.AddEdge(1, 2);
        graph.AddEdge(5, 6);

        var route1nodes = graph.GetRoute(0, 4).nodes;

        Assert.Equal(0, route1nodes.ElementAt(0));
        Assert.Equal(1, route1nodes.ElementAt(1));
        Assert.Equal(2, route1nodes.ElementAt(2));
        Assert.Equal(5, route1nodes.ElementAt(3));
        Assert.Equal(4, route1nodes.ElementAt(4));

        graph.RemoveEdge(0, 1);
        graph.AddEdge(0, 1, 99);
        var route2nodes = graph.GetRoute(0, 4).nodes;
        Assert.Equal(0, route2nodes.ElementAt(0));
        Assert.Equal(3, route2nodes.ElementAt(1));
        Assert.Equal(4, route2nodes.ElementAt(2));
    }

    [Fact]
    public void Large_graph_distance_matches()
    {
        var graph = new Graph(1000000);
        for (int i = 0; i + 1 < 1000000; i++)
        {
            graph.AddEdge(i, i + 1);
        }

        var route = graph.GetRoute(0, 999999);

        Assert.True(graph.IsConnected(999998, 999999));
        Assert.Equal(999999, route.distance);
    }

    [Fact]
    public void Graph_can_be_resized()
    {
        var graph = new Graph(42);

        graph.AddEdge(0, 42);
        graph.AddEdge(0, 78);
        graph.AddEdge(0, 400);

        Assert.Equal(401, graph.Capacity);
        Assert.True(graph.IsConnected(0, 42));
        Assert.True(graph.IsConnected(0, 78));
    }

    [Fact]
    public void NodeCount_and_EdgeCount_matches()
    {
        var graph = new Graph(42);

        graph.AddEdge(0, 1);
        graph.AddEdge(0, 2);
        graph.AddEdge(0, 3);

        Assert.Equal(4, graph.NodesCount);
        Assert.Equal(6, graph.EdgesCount);
    }

    [Fact]
    public void NodeCount_and_EdgeCount_matches_after_removal()
    {
        var graph = new Graph(42);

        graph.AddEdge(0, 1);
        graph.AddEdge(0, 2);
        graph.AddEdge(0, 3);
        graph.RemoveEdge(0, 1);

        Assert.Equal(4, graph.NodesCount);
        Assert.Equal(4, graph.EdgesCount);
    }
}