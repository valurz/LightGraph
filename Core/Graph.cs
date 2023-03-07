using System;
using System.Linq;
using System.Collections.Generic;

namespace LightGraph.Core;
/// <summary>
/// A simple, lightweight bidirectional graph optimized around Dijkstra's shortest path 
/// </summary>
public class Graph
{
    private Dictionary<int, List<(int target, float weight)>> _nodesAndEdges;
    private Dictionary<int, float> _distances;
    private Dictionary<int, List<(int node, float weight)>> _shortestPaths;
    private int _nrOfEdges;

    /// <summary>
    /// Create a new graph with some initial capacity 
    /// </summary>
    /// <param name="capacity">The expected number of nodes this graph will contain</param>
    public Graph(int capacity)
    {
        if (capacity <= 0)
            throw new Exception("Capacity must be greater than 0");

        _nodesAndEdges = new Dictionary<int, List<(int target, float weight)>>(capacity * 2);
        _shortestPaths = new Dictionary<int, List<(int node, float distance)>>(capacity);
        _distances = new Dictionary<int, float>(capacity);
    }

    /// <summary>
    ///  Add a bidirectional edge between between two nodes to the graph
    /// </summary>
    /// <param name="sourceNode">Source node</param>
    /// <param name="targetNode">Target node</param>
    public void AddEdge(int sourceNode, int targetNode, float weight = 1)
    {
        if (sourceNode < 0)
            throw new Exception("Source node must be a positive number");
        else if (targetNode < 0)
            throw new Exception("Target node must be a positive number");

        if (sourceNode == targetNode)
            throw new Exception("Source and target are the same");

        if (weight < 1)
            throw new Exception("Weight must be greater than 0");

        if (!_nodesAndEdges.ContainsKey(sourceNode))
        {
            _nodesAndEdges.Add(sourceNode, new List<(int target, float weight)>(8) { });
            _distances.Add(sourceNode, int.MaxValue);
            _shortestPaths.Add(sourceNode, new List<(int, float)>(20));
        }

        if (!_nodesAndEdges.ContainsKey(targetNode))
        {
            _nodesAndEdges.Add(targetNode, new List<(int target, float weight)>(8) { });
            _distances.Add(targetNode, int.MaxValue);
            _shortestPaths.Add(targetNode, new List<(int, float)>(20));
        }

        if (!_nodesAndEdges[sourceNode].Any(e => e.target == targetNode))
        {
            _nodesAndEdges[sourceNode].Add((targetNode, weight));
            _nodesAndEdges[sourceNode].Sort((a, b) => a.weight.CompareTo(b.weight));
            _nrOfEdges++;
        }

        if (!_nodesAndEdges[targetNode].Any(e => e.target == sourceNode))
        {
            _nodesAndEdges[targetNode].Add((sourceNode, weight));
            _nodesAndEdges[targetNode].Sort((a, b) => a.weight.CompareTo(b.weight));
            _nrOfEdges++;
        }
    }

    /// <summary>
    ///  Removes an edge between two nodes
    /// </summary>
    /// <param name="sourceNode">Source node</param>
    /// <param name="targetNode">Target node</param>
    public void RemoveEdge(int sourceNode, int targetNode)
    {
        if (!_nodesAndEdges[sourceNode].Any(n => n.target == targetNode))
            throw new Exception($"Edge from {sourceNode} to {targetNode} was not found");
        else if (!_nodesAndEdges[targetNode].Any(n => n.target == sourceNode))
            throw new Exception($"Edge from {targetNode} to {targetNode} was not found");

        var edge1 = _nodesAndEdges[sourceNode].Where(n => n.target == targetNode).Single();
        var edge2 = _nodesAndEdges[targetNode].Where(n => n.target == sourceNode).Single();
        _nodesAndEdges[sourceNode].Remove(edge1);
        _nodesAndEdges[targetNode].Remove(edge2);
    }

    /// <summary>
    ///  Check if two nodes are connected
    /// </summary>
    /// <param name="nodeA">First node to be checked</param>
    /// <param name="nodeB">Second node to be checked</param>
    /// <returns>True if the two nodes are connected</returns>
    public bool IsConnected(int nodeA, int nodeB)
    {
        if (_nodesAndEdges.ContainsKey(nodeA) && _nodesAndEdges.ContainsKey(nodeB))
            return _nodesAndEdges[nodeA].Any(k => k.target == nodeB) && _nodesAndEdges[nodeB].Any(k => k.target == nodeA);
        else if (!_nodesAndEdges.ContainsKey(nodeA))
            throw new Exception($"Node {nodeA} does not exist");
        else if (!_nodesAndEdges.ContainsKey(nodeB))
            throw new Exception($"Node {nodeB} does not exist");

        return false;
    }

    /// <summary>
    /// Find a route between two nodes using Dijkstra's shortest path
    /// </summary>
    /// <param name="startNode">Start node</param>
    /// <param name="endNode">End node</param>
    /// /// <returns>A list of nodes containing the shortest path and the total distance from startNode to endNode</returns>
    public (IReadOnlyList<int> Nodes, float Distance) GetRoute(int startNode, int endNode)
    {
        var shortestPaths = _shortestPaths.ToDictionary(k => k.Key, v => v.Value);
        var distances = _distances.ToDictionary(k => k.Key, v => v.Value);
        distances[startNode] = 0;
        var currentNodes = new Queue<int>();
        currentNodes.Enqueue(startNode);
        for (int i = 0; i < _nodesAndEdges[startNode].Count; i++)
        {
            currentNodes.Enqueue(_nodesAndEdges[startNode][i].target);
        }
        var visited = 0;
        while (visited++ < _nrOfEdges)
        {
            var currentNode = currentNodes.Dequeue();
            for (int an = 0; an < _nodesAndEdges[currentNode].Count; an++)
            {
                var newDistance = shortestPaths[currentNode].Sum(l => l.weight) + _nodesAndEdges[currentNode][an].weight;
                var adjacentNode = _nodesAndEdges[currentNode][an].target;
                var currentDistance = distances[adjacentNode];
                if (currentDistance > newDistance)
                {
                    shortestPaths[adjacentNode].Clear();
                    shortestPaths[adjacentNode].AddRange(shortestPaths[currentNode]);
                    shortestPaths[adjacentNode].Add((currentNode, _nodesAndEdges[currentNode][an].weight));
                    distances[adjacentNode] = newDistance;
                }
                for (int i = 0; i < _nodesAndEdges[adjacentNode].Count; i++)
                {
                    if (_nodesAndEdges[adjacentNode][i].target != currentNode)
                    {
                        currentNodes.Enqueue(_nodesAndEdges[adjacentNode][i].target);
                    }
                }
            }
        }
        shortestPaths[endNode].Add((endNode, 0));

        return (shortestPaths[endNode].Select(k => k.node).ToList(), distances[endNode]);
    }
}