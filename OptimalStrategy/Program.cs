namespace OptimalStrategy;

class Program
{
    static void Main(string[] args)
    {
        int[,] matrix =
        {
            { 5, 25, 19, 21 },
            { 9, 10, 8, 14 },
            { 18, 15, 24, 16 },
            { 12, 19, 23, 13 }
        };

        WaldCriterion wald = new WaldCriterion(matrix);
        var result = wald.FindOptimalStrategy();

        Console.WriteLine($"Оптимальное значение V: {result.value}");
        Console.WriteLine($"Оптимальная стратегия: {result.strategy}");
    }
}