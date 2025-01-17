namespace MaximumFlow;

static class Program
{
    static void Main(string[] args)
    {
        // TODO Поменять названия для переменных, вес на пропускную способность
        Console.Write("Введите количество вершин: ");
        int vertexCount = int.Parse(Console.ReadLine());

        GraphManager graphManager = GraphManager.GetInstance(vertexCount);
        graphManager.AddEdgeFromConsole();

        Console.Write("Введите исток: ");
        int source = int.Parse(Console.ReadLine()) - 1;

        Console.Write("Введите сток: ");
        int sink = int.Parse(Console.ReadLine()) - 1;

        IMaximumFlowSolve solver = new FordFalkersonAlgorithm();
        double maxFlow = solver.GetMaximumFlow(graphManager.Graph, source, sink);

        Console.WriteLine($"Максимальный возможный поток {maxFlow}");
    }
}