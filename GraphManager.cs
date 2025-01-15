using ShortestPathInGraph.Methods;

namespace ShortestPathInGraph;

public class GraphManager
{
    private Graph graph;
    private static GraphManager instance;
    public GraphManager(int vertexCount)
    {
        graph = new Graph(vertexCount);
    }
    public static GraphManager GetInstance(int vertexCount)
    {
        if (instance == null)
        {
            instance = new GraphManager(vertexCount);
        }
        return instance;
    }

    public void AddEdgeFromConsole()
    {
        Console.Write("Введите количество ребер: ");
        var edgeCount = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

        for (int i = 0; i < edgeCount; i++)
        {
            Console.Write($"Введите ребро {i + 1} (Начальная_вершина Конечная_вершина Вес): ");
            string[] edgeInput = Console.ReadLine().Split(' ');
            int from = int.Parse(edgeInput[0]);
            int to = int.Parse(edgeInput[1]);
            int weight = int.Parse(edgeInput[2]);
            graph.AddEdge(from, to, weight);
        }
    }

    public Dictionary<int, int> GetShortestPath(int startVertex, string algorithm, int goalVertex = -1)
    {
        switch (algorithm)
        {
            case "1":
                return Dijkstra.ShortestPath(graph, startVertex - 1);
            case "2":
                if (goalVertex == -1)
                {
                    throw new ArgumentException("Для алгоритма A* необходимо указать целевую вершину.");
                }
                return AStar.ShortestPath(graph, startVertex - 1, goalVertex - 1);
            case "3":
                return Lee.ShortestPath(graph, startVertex - 1);
            default:
                throw new ArgumentException("Указан неверный алгоритм.");
        }
    }
}