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

        ICriterion[] criterials = new ICriterion[]
        {
            new WaldCriterion(matrix),
            new MaximalCriterion(matrix),
            new HurwitzCriterion(matrix, 0.5),
            new SavageCriterion(matrix),
            new LaplaceCriterion(matrix)
        };

        string[] criterionNames = new string[]
        {
            "Минимаксный критерий Вальда",
            "Максимаксный критерий",
            "Критерий Гурвица",
            "Критерий Сэвиджа",
            "Критерий Лапласа"
        };

        for (int i = 0; i < criterials.Length; i++)
        {
            var result = criterials[i].FindOptimalStrategy();
            Console.WriteLine(criterionNames[i]);
            Console.WriteLine($"Оптимальное значение V: {result.value}");
            Console.WriteLine($"Оптимальная стратегия: {result.strategy}\n");
        }
    }
}