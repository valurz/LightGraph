# LightGraph
A simple, lightweight bidirectional graph optimized around Dijkstra's shortest path contained in a single C# class.

![passing](https://img.shields.io/github/actions/workflow/status/valurz/lightgraph/dotnet.yml) ![license](https://img.shields.io/github/license/valurz/lightgraph)

## Example
```csharp
// Create with some initial capacity
Graph graph = new Graph(5);

// Add bidirectional edges between two nodes with optional weight
graph.AddEdge(0, 1);
graph.AddEdge(1, 2);
graph.AddEdge(2, 3);
graph.AddEdge(3, 4, weight: 42);
graph.AddEdge(2, 4);

// Find the shortest path between two nodes
var route = graph.GetRoute(0, 4);

// Do something with the route tuple
Console.WriteLine($"Route: {string.Join(" -> ", route.nodes)}");
Console.WriteLine($"Distance: {route.distance}");

// Output:
// Route: 0 -> 1 -> 2 -> 4
// Distance: 3

```
