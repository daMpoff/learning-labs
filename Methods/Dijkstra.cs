namespace ShortestPathInGraph.Methods;

public class Dijkstra
{
    public static Dictionary<int, int> ShortestPath(Graph graph, int startVertex)
    {
        int vertexCount = graph.Vertices.Count;
        Dictionary<int, int> distances = new Dictionary<int, int>();
        bool[] visited = new bool[vertexCount];
        int[] previous = new int[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            distances[i] = int.MaxValue;
            visited[i] = false;
            previous[i] = -1;
        }

        distances[startVertex] = 0;

        for (int count = 0; count < vertexCount - 1; count++)
        {
            int currentVertex = MinDistance(distances, visited);
            visited[currentVertex] = true;

            foreach (var edge in graph.Vertices[currentVertex].Edges)
            {
                int neighbor = edge.To;
                if (!visited[neighbor] && distances[currentVertex] != int.MaxValue &&
                    distances[currentVertex] + edge.Weight < distances[neighbor])
                {
                    distances[neighbor] = distances[currentVertex] + edge.Weight;
                    previous[neighbor] = currentVertex;
                }
            }
        }

        return distances;
    }

    private static int MinDistance(Dictionary<int, int> distances, bool[] visited)
    {
        int min = int.MaxValue;
        int minIndex = -1;

        for (int v = 0; v < distances.Count; v++)
        {
            if (!visited[v] && distances[v] <= min)
            {
                min = distances[v];
                minIndex = v;
            }
        }

        return minIndex;
    }
}