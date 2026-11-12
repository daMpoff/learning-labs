namespace MaximumFlow;

public class GraphManager
{
    public Graph Graph { get; set; }
    private static GraphManager instance;
    public GraphManager(int vertexCount)
    {
        Graph = new Graph(vertexCount);
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
            int from = int.Parse(edgeInput[0]) - 1;
            int to = int.Parse(edgeInput[1]) - 1;
            int weight = int.Parse(edgeInput[2]);
            Graph.AddEdge(from, to, weight);
        }
    }
}