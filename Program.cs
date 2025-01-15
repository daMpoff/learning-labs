namespace ShortestPathInGraph;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Введите количество вершин: ");
        int vertexCount = int.Parse(Console.ReadLine());

        GraphManager graphManager = GraphManager.GetInstance(vertexCount);
        graphManager.AddEdgeFromConsole();

        Console.Write("Введите начальную вершину: ");
        int startVertex = int.Parse(Console.ReadLine());

        Console.Write("Введите алгоритм (1 - Алгоритм Дейкстры\n, 2 - Алгоритм A*\n, 3 - Алгоритм Ли): ");
        string algorithm = Console.ReadLine();

        int goalVertex = -1;
        if ((!string.IsNullOrEmpty(algorithm) && algorithm.ToLower() == "2"))
        {
            Console.Write("Введите конечную вершину: ");
            goalVertex = int.Parse(Console.ReadLine());
        }

        var distances = graphManager.GetShortestPath(startVertex, algorithm, goalVertex);

        Console.WriteLine("Кратчайшие расстояния от вершины " + startVertex + ":");
        for (int i = 0; i < distances.Count; i++)
        {
            Console.WriteLine("Вершина " + (i + 1) + ": " + distances[i]);
        }
    }
}