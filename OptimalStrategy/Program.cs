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
        (double value, int strategy) result;

        WaldCriterion wald = new WaldCriterion(matrix);
        result = wald.FindOptimalStrategy();
        Console.WriteLine("Минимаксный критерий Вальда");
        Console.WriteLine($"Оптимальное значение V: {result.value}");
        Console.WriteLine($"Оптимальная стратегия: {result.strategy}\n");
        
        MaximalCriterion maximalCriterion = new MaximalCriterion(matrix);
        result = maximalCriterion.FindOptimalStrategy();
        Console.WriteLine("Максимаксный критерий");
        Console.WriteLine($"Оптимальное значение V: {result.value}");
        Console.WriteLine($"Оптимальная стратегия: {result.strategy}\n");
        
        HurwitzСriterion hurwitzСriterion = new HurwitzСriterion(matrix,0.5);
        result = hurwitzСriterion.FindOptimalStrategy();
        Console.WriteLine("Критерий Гурвица");
        Console.WriteLine($"Оптимальное значение V: {result.value}");
        Console.WriteLine($"Оптимальная стратегия: {result.strategy}\n");
        
        SavageCriterion savageCriterion = new SavageCriterion(matrix);
        result = savageCriterion.FindOptimalStrategy();
        Console.WriteLine("Критерий Сэвиджа");
        Console.WriteLine($"Оптимальное значение V: {result.value}");
        Console.WriteLine($"Оптимальная стратегия: {result.strategy}\n");
        
        LaplaceCriterion laplaceCriterion = new LaplaceCriterion(matrix);
        result = laplaceCriterion.FindOptimalStrategy();
        Console.WriteLine("Критерий Лапласа");
        Console.WriteLine($"Оптимальное значение V: {result.value}");
        Console.WriteLine($"Оптимальная стратегия: {result.strategy}\n");
    }
}