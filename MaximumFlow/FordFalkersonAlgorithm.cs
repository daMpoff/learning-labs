namespace MaximumFlow;

public class FordFalkersonAlgorithm : IMaximumFlowSolve
{
    public double GetMaximumFlow(Graph graph, int source, int sink)
    {
        int[] parent = new int[graph.Vertices.Count];
        double maxFlow = 0;

        while (Bfs(graph, source, sink, parent))
        {
            int pathFlow = int.MaxValue;
            for (int v = sink; v != source; v = parent[v])
            {
                int u = parent[v];
                foreach (var edge in graph.Vertices[u].Edges)
                {
                    if (edge.To == v && edge.Capacity - edge.Flow > 0)
                    {
                        pathFlow = Math.Min(pathFlow, edge.Capacity - edge.Flow);
                        break;
                    }
                }
            }

            for (int v = sink; v != source; v = parent[v])
            {
                int u = parent[v];
                foreach (var edge in graph.Vertices[u].Edges)
                {
                    if (edge.To == v && edge.Capacity - edge.Flow > 0)
                    {
                        edge.Flow += pathFlow;
                        foreach (var reverseEdge in graph.Vertices[v].Edges)
                        {
                            if (reverseEdge.To == u)
                            {
                                reverseEdge.Flow -= pathFlow;
                                break;
                            }
                        }

                        break;
                    }
                }
            }

            maxFlow += pathFlow;
        }

        return maxFlow;
    }

    private bool Bfs(Graph graph, int source, int sink, int[] parent)
    {
        bool[] visited = new bool[graph.Vertices.Count];
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(source);
        visited[source] = true;
        parent[source] = -1;

        while (queue.Count > 0)
        {
            int u = queue.Dequeue();

            foreach (var edge in graph.Vertices[u].Edges)
            {
                int v = edge.To;
                if (!visited[v] && edge.Capacity - edge.Flow > 0)
                {
                    if (v == sink)
                    {
                        parent[v] = u;
                        return true;
                    }

                    queue.Enqueue(v);
                    visited[v] = true;
                    parent[v] = u;
                }
            }
        }

        return false;
    }
}