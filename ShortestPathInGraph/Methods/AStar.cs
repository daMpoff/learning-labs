namespace ShortestPathInGraph.Methods;
public class AStar
{
    public static Dictionary<int, int> ShortestPath(Graph graph, int startVertex, int goalVertex)
    {
        int vertexCount = graph.Vertices.Count;
        Dictionary<int, int> distances = new Dictionary<int, int>();
        Dictionary<int, int> previous = new Dictionary<int, int>();
        PriorityQueue<(int vertex, int cost), int> openSet = new PriorityQueue<(int vertex, int cost), int>();

        for (int i = 0; i < vertexCount; i++)
        {
            distances[i] = int.MaxValue;
            previous[i] = -1;
        }

        distances[startVertex] = 0;
        openSet.Enqueue((startVertex, 0), 0);

        while (openSet.Count > 0)
        {
            var (currentVertex, currentCost) = openSet.Dequeue();

            if (currentVertex == goalVertex)
            {
                break;
            }

            foreach (var edge in graph.Vertices[currentVertex].Edges)
            {
                int neighbor = edge.To;
                int newCost = currentCost + edge.Weight;

                if (newCost < distances[neighbor])
                {
                    distances[neighbor] = newCost;
                    previous[neighbor] = currentVertex;
                    openSet.Enqueue((neighbor, newCost), newCost);
                }
            }
        }

        return distances;
    }
}