/* MIT License

Copyright (c) 2023 Valur Zophoníasson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using System;
using System.Linq;
using System.Collections.Generic;

namespace LightGraph.Core
{
    /// <summary>
    /// A simple, lightweight bidirectional graph optimized around Dijkstra's shortest path 
    /// </summary>
    public sealed class Graph
    {
        private List<(int target, float weight)>[] _nodesAndEdges;
        private int _nodeCapacity;
        private int _edgesToNodesCapacity;

        /// <summary>
        /// Create a new graph with some initial capacity 
        /// </summary>
        /// <param name="nodeCapacity">The expected number of nodes this graph will contain</param>
        /// <param name="edgesToNodesCapacity">The expected number of outgoing edges that each node might have</param>
        public Graph(int nodeCapacity, int edgesToNodesCapacity = 8)
        {
            if (nodeCapacity <= 0)
                throw new Exception("NodeCapacity must be greater than 0");
            if (edgesToNodesCapacity <= 0)
                throw new Exception("EdgesToNodesCapacity must be greater than 0");

            _nodesAndEdges = new List<(int target, float weight)>[nodeCapacity];
            _edgesToNodesCapacity = edgesToNodesCapacity;
            _nodeCapacity = nodeCapacity;
        }

        /// <summary>
        ///  Add a bidirectional edge between between two nodes to the graph
        /// </summary>
        /// <param name="sourceNode">Source node</param>
        /// <param name="targetNode">Target node</param>
        public void AddEdge(int sourceNode, int targetNode, float weight = 1.0f)
        {
            if (sourceNode > _nodeCapacity)
                throw new Exception("Source node index is greater than the capacity of the graph");

            if (targetNode > _nodeCapacity)
                throw new Exception("Target node index is greater than the capacity of the graph");

            if (sourceNode < 0)
                throw new Exception("Source node must be a positive number");
            else if (targetNode < 0)
                throw new Exception("Target node must be a positive number");

            if (sourceNode == targetNode)
                throw new Exception("Source and target are the same");

            if (weight < 1)
                throw new Exception("Weight must be greater than 0");

            if (_nodesAndEdges[sourceNode] == null)
            {
                _nodesAndEdges[sourceNode] = new List<(int target, float weight)>(_edgesToNodesCapacity) { };
            }

            if (_nodesAndEdges[targetNode] == null)
            {
                _nodesAndEdges[targetNode] = new List<(int target, float weight)>(_edgesToNodesCapacity) { };
            }

            if (!_nodesAndEdges[sourceNode].Any(e => e.target == targetNode))
            {
                _nodesAndEdges[sourceNode].Add((targetNode, weight));
            }

            if (!_nodesAndEdges[targetNode].Any(e => e.target == sourceNode))
            {
                _nodesAndEdges[targetNode].Add((sourceNode, weight));
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
            if (_nodesAndEdges[nodeA] != null && _nodesAndEdges[nodeB] != null)
                return _nodesAndEdges[nodeA].Any(k => k.target == nodeB) && _nodesAndEdges[nodeB].Any(k => k.target == nodeA);
            else if (_nodesAndEdges[nodeA] == null)
                throw new Exception($"Node {nodeA} does not exist");
            else if (_nodesAndEdges[nodeB] == null)
                throw new Exception($"Node {nodeB} does not exist");

            return false;
        }

        /// <summary>
        /// Find a route between two nodes using Dijkstra's shortest path
        /// </summary>
        /// <param name="startNode">Start node</param>
        /// <param name="endNode">End node</param>
        /// /// <returns>A collection of nodes containing the shortest path and the total distance from startNode to endNode</returns>
        public (IEnumerable<int> nodes, float distance) GetRoute(int startNode, int endNode)
        {
            var predecessors = new int[_nodeCapacity];
            var currentNodes = new List<(int node, float distance)>(_edgesToNodesCapacity);
            var distances = GetDistanceArray(_nodeCapacity, startNode);
            currentNodes.Add((startNode, 0));
            while (currentNodes.Count > 0)
            {
                var currentNode = currentNodes[0].node;
                currentNodes.Remove(currentNodes[0]);
                for (int an = 0; an < _nodesAndEdges[currentNode].Count; an++)
                {
                    var adjacentNode = _nodesAndEdges[currentNode][an].target;
                    var newDistance = distances[currentNode] + _nodesAndEdges[currentNode][an].weight;
                    if (distances[adjacentNode] > newDistance)
                    {
                        predecessors[adjacentNode] = currentNode;
                        distances[adjacentNode] = newDistance;
                        currentNodes.Add((adjacentNode, newDistance));
                        currentNodes.Sort((k, v) => k.distance.CompareTo(v.distance));
                    }
                }
            }
            var shortestPath = new Stack<int>(_nodeCapacity);
            shortestPath.Push(endNode);
            int pathNode = endNode;
            while (distances[pathNode] != 0)
            {
                shortestPath.Push(predecessors[pathNode]);
                pathNode = predecessors[pathNode];
            }

            return (shortestPath, distances[endNode]);
        }

        private static float[] GetDistanceArray(int nodeCapacity, int startNode)
        {
            var distances = new float[nodeCapacity];
            for (int i = 0; i < nodeCapacity; i++)
                distances[i] = float.MaxValue;

            distances[startNode] = 0;
            return distances;
        }
    }
}