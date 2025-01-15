namespace ShortestPathInGraph;

class Program
{
    static void Main()
    {
        if (TryInitGraph(out GraphManager graphManager))
        {
            int startVertex = GetStartVertex();
            string algorithm = GetAlgorithmChoice();
            int goalVertex = GetGoalVertexIfNeeded(algorithm);

            try
            {
                var distances = graphManager.GetShortestPath(startVertex, algorithm, goalVertex);
                PrintDistances(startVertex, distances);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    static bool TryInitGraph(out GraphManager graphManager)
    {
        graphManager = null;
        Console.Write("Введите количество вершин: ");
        try
        {
            int vertexCount = int.Parse(Console.ReadLine());
            if (vertexCount > 0)
            {
                graphManager = GraphManager.GetInstance(vertexCount);
                graphManager.AddEdgeFromConsole();
                return true;
            }
            else
            {
                Console.WriteLine("Количество вершин должно быть больше нуля.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Возникла ошибка! Повторите попытку. Ошибка: {ex.Message}");
        }

        return false;
    }

    static int GetStartVertex()
    {
        Console.Write("Введите начальную вершину: ");
        try
        {
            return int.Parse(Console.ReadLine());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка ввода начальной вершины. Ошибка: {ex.Message}");
            return -1;
        }
    }

    static string GetAlgorithmChoice()
    {
        Console.Write("Введите алгоритм (1 - Алгоритм Дейкстры, 2 - Алгоритм A*, 3 - Алгоритм Ли): ");
        return Console.ReadLine();
    }

    static int GetGoalVertexIfNeeded(string algorithm)
    {
        if (!string.IsNullOrEmpty(algorithm) && algorithm.ToLower() == "2")
        {
            Console.Write("Введите конечную вершину: ");
            try
            {
                return int.Parse(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка ввода конечной вершины. Ошибка: {ex.Message}");
            }
        }
        return -1;
    }

    static void PrintDistances(int startVertex, Dictionary<int, int> distances)
    {
        Console.WriteLine("Кратчайшие расстояния от вершины " + startVertex + ":");
        foreach (var kvp in distances)
        {
            Console.WriteLine("Вершина " + (kvp.Key + 1) + ": " + kvp.Value);
        }
    }
}