# LightGraph
A simple, lightweight bidirectional graph optimized around Dijkstra's shortest path.

## Example

```csharp
// Create with some initial capacity
Graph graph = new Graph(5);

// Add bidirectional edges between two nodes with optional weight
graph.Add(0,1,1);
graph.Add(1,2,1);
graph.Add(2,3,1);
graph.Add(3,4,42)
graph.Add(2,4,1);

// Find shortest path between two nodes
var route = graph.GetRoute(0,4);

// Do something with the route tuple
Console.WriteLine($"Route: {string.Join(" -> ", route.Nodes)}");
Console.WriteLine($"Total distance: {route.Distance}");

// Output:
// Route: 0 -> 1 -> 2 -> 4
// Distance: 4

```
