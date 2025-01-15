namespace ShortestPathInGraph.Methods;

public class Lee
{
    public static Dictionary<int, int> ShortestPath(Graph graph, int startVertex)
    {
        int vertexCount = graph.Vertices.Count;
        Dictionary<int, int> distances = new Dictionary<int, int>();
        bool[] visited = new bool[vertexCount];
        Queue<int> queue = new Queue<int>();

        for (int i = 0; i < vertexCount; i++)
        {
            distances[i] = int.MaxValue;
            visited[i] = false;
        }

        distances[startVertex] = 0;
        queue.Enqueue(startVertex);

        while (queue.Count > 0)
        {
            int currentVertex = queue.Dequeue();
            visited[currentVertex] = true;

            foreach (var edge in graph.Vertices[currentVertex].Edges)
            {
                int neighbor = edge.To;

                if (!visited[neighbor])
                {
                    distances[neighbor] = distances[currentVertex] + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return distances;
    }
}