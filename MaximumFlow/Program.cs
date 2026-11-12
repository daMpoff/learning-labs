namespace MaximumFlow;

static class Program
{
    public static void Main()
    {
        Console.Write("Введите количество вершин: ");
        int vertexCount = int.Parse(Console.ReadLine());

        GraphManager graphManager = GraphManager.GetInstance(vertexCount);
        graphManager.AddEdgeFromConsole();

        Console.Write("Введите исток: ");
        int source = int.Parse(Console.ReadLine()) - 1;

        Console.Write("Введите сток: ");
        int sink = int.Parse(Console.ReadLine()) - 1;

        Console.Write("Выберите алгоритм (1 - Линейный алгоритм, 2 - Алгоритм Форда-Фалкерсона): ");
        int algorithmChoice = int.Parse(Console.ReadLine());

        IMaximumFlowSolve solver;
        if (algorithmChoice == 1)
        {
            solver = new LinearAlgorithm();
        }
        else
        {
            solver = new FordFalkersonAlgorithm();
        }

        double maxFlow = solver.GetMaximumFlow(graphManager.Graph, source, sink);
        Console.WriteLine($"Максимальный возможный поток: {maxFlow}");
    }
}