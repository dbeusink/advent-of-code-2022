namespace advent_of_code_2022.Puzzles;

internal class Day12 : PuzzleBase
{
    public Day12() : base(nameof(Day12)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();

        var matrix = GetMatrix();
        var graph = GetGraph(matrix);
        var start = graph.AdjacencyList.Keys.Where(x => x.Elevation.Equals('S')).Single();
        var end = graph.AdjacencyList.Keys.Where(x => x.Elevation.Equals('E')).Single();

        graph.TryGetShortestPath(start, end, out var path, x => x.Visited = true);
        PrintMatrix(matrix);

        return path.Skip(1).Count().ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        var matrix = GetMatrix();
        var graph = GetGraph(matrix);
        var end = graph.AdjacencyList.Keys.Where(x => x.Elevation.Equals('E')).Single();

        var shortestPath = GetShortestPaths().OrderBy(x => x.Count).First();
        shortestPath.ForEach(x => x.Visited = true);
        PrintMatrix(matrix);

        return shortestPath.Skip(1).Count().ToString();

        IEnumerable<List<Location>> GetShortestPaths()
        {
            foreach (var location in graph!.AdjacencyList.Keys.Where(x => x.Elevation == 'a' || x.Elevation == 'S'))
            {
                if (graph.TryGetShortestPath(location, end, out var path))
                {
                    yield return path;
                }
            }
        }
    }

    private Location[][] GetMatrix()
        => Input!.Select((line, y) => line.Select((elevation, x) => new Location(x, y, elevation)).ToArray()).ToArray();

    private void PrintMatrix(Location[][] matrix)
    {
        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                if (matrix[i][j].Elevation == 'S')
                {
                    Console.Write($"\u001b[38;5;10mS\u001b[0m");
                }
                else if (matrix[i][j].Elevation == 'E')
                {
                    Console.Write($"\u001b[38;5;9mE\u001b[0m");
                }
                else if (matrix[i][j].Visited)
                {
                    Console.Write($"\u001b[38;5;81m{matrix[i][j].Elevation}\u001b[0m");
                }
                else
                {
                    Console.Write(matrix[i][j].Elevation);
                }
            }

            Console.WriteLine();
        }
    }

    private Graph<Location> GetGraph(Location[][] matrix)
    {
        var graph = new Graph<Location>();

        // Vertices
        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                graph.AddVertex(matrix[i][j]);
            }
        }

        // Edges
        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                if (i > 0)
                {
                    graph.AddEdgeWhen(matrix[i][j], matrix[i - 1][j], IsReachable); // Neighbor up
                }
                if (i < matrix.Length - 1)
                {
                    graph.AddEdgeWhen(matrix[i][j], matrix[i + 1][j], IsReachable); // Neighbor down
                }
                if (j > 0)
                {
                    graph.AddEdgeWhen(matrix[i][j], matrix[i][j - 1], IsReachable); // Neighbor left
                }
                if (j < matrix[i].Length - 1)
                {
                    graph.AddEdgeWhen(matrix[i][j], matrix[i][j + 1], IsReachable); // Neighbor right
                }
            }
        }

        return graph;
    }

    private bool IsReachable(Location self, Location neighbor)
    {
        var currentElevation = self.Elevation switch
        {
            'S' => 'a',
            'E' => 'z',
            _ => self.Elevation
        };

        var neighborElevation = neighbor.Elevation switch
        {
            'S' => 'a',
            'E' => 'z',
            _ => neighbor.Elevation
        };

        return neighborElevation - currentElevation <= 1;
    }
}

public class Location
{
    public int X { get; }
    public int Y { get; }
    public char Elevation { get; }
    public bool Visited { get; set; }

    public Location(int x, int y, char elevation)
    {
        X = x;
        Y = y;
        Elevation = elevation;
    }

    public override bool Equals(object? obj)
    {
        return obj is Location location &&
               X == location.X &&
               Y == location.Y &&
               Elevation == location.Elevation;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Elevation);
    }
}

// This is an exercise for me, never used graphs and BFS/DFS before
// Used examples from non-directional graph and BFS algorithm here: https://www.koderdojo.com/blog/breadth-first-search-and-shortest-path-in-csharp-and-net-core
public class Graph<T> where T : notnull
{
    public Dictionary<T, HashSet<T>> AdjacencyList { get; } = new();

    public Graph() { }
    public Graph(IEnumerable<T> vertices, IEnumerable<Tuple<T, T>> edges, bool directed)
    {
        foreach (var vertex in vertices)
        {
            AddVertex(vertex);
        }

        foreach (var edge in edges)
        {
            AddEdge(edge, directed);
        }
    }

    public void AddVertex(T vertex)
        => AdjacencyList[vertex] = new HashSet<T>();

    public void AddEdgeWhen(T a, T b, Func<T, T, bool> predicate)
    {
        if (predicate(a, b))
        {
            AddEdge(new Tuple<T, T>(a, b), true);
        }
    }

    public void AddEdge(Tuple<T, T> edge, bool directed)
    {
        if (AdjacencyList.ContainsKey(edge.Item1) && AdjacencyList.ContainsKey(edge.Item2))
        {
            AdjacencyList[edge.Item1].Add(edge.Item2);

            if (!directed)
            {
                AdjacencyList[edge.Item2].Add(edge.Item1);
            }
        }
    }

    // Shortest path finding using Breadth-first-search (BFS)
    public bool TryGetShortestPath(T start, T end, out List<T> path, Action<T>? onShortestPathNodeVisited = null)
    {
        path = new List<T>();
        var previous = new Dictionary<T, T>();
        var queue = new Queue<T>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            foreach (var neighbor in AdjacencyList[vertex])
            {
                if (previous.ContainsKey(neighbor))
                {
                    continue;
                }

                previous[neighbor] = vertex;
                queue.Enqueue(neighbor);
            }
        }

        var current = end;
        while (!current.Equals(start))
        {
            path.Add(current);
            onShortestPathNodeVisited?.Invoke(current);
            if (previous.TryGetValue(current, out var newCurrent))
            {
                current = newCurrent;
            }
            else
            {
                return false;
            }
        }
        path.Add(start);
        path.Reverse();
        return true;
    }
}