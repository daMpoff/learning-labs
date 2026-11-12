namespace OptimalStrategy;

public class MaximalCriterion : ICriterion
{
    private int[,] matrix;

    public MaximalCriterion(int[,] matrix)
    {
        this.matrix = matrix;
    }

    public (double value, int strategy) FindOptimalStrategy()
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        // Шаг 1: Найти максимумы строк
        int[] rowMaxima = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            int max = int.MinValue;
            for (int j = 0; j < cols; j++)
            {
                if (matrix[i, j] > max)
                {
                    max = matrix[i, j];
                }
            }

            rowMaxima[i] = max;
        }

        // Шаг 2: Найти максимум среди максимумов
        int maxOfMaxima = rowMaxima.Max();

        // Шаг 3: Найти индекс строки с этим максимумом
        int optimalStrategyIndex = Array.IndexOf(rowMaxima, maxOfMaxima) + 1;

        return (maxOfMaxima, optimalStrategyIndex);
    }
}