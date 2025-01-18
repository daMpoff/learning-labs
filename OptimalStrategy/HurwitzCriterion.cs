namespace OptimalStrategy;

public class HurwitzCriterion : ICriterion
{
    private int[,] matrix;
    private double alpha;

    public HurwitzCriterion(int[,] matrix, double alpha)
    {
        this.matrix = matrix;
        this.alpha = alpha;
    }

    public (double value, int strategy) FindOptimalStrategy()
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        // Шаг 1: Рассчитать значение критерия Гурвица для каждой строки
        double[] hurwiczValues = new double[rows];
        for (int i = 0; i < rows; i++)
        {
            int max = int.MinValue;
            int min = int.MaxValue;
            for (int j = 0; j < cols; j++)
            {
                if (matrix[i, j] > max)
                {
                    max = matrix[i, j];
                }
                if (matrix[i, j] < min)
                {
                    min = matrix[i, j];
                }
            }
            hurwiczValues[i] = alpha * max + (1 - alpha) * min;
        }

        // Шаг 2: Найти максимум среди значений Гурвица
        double maxHurwiczValue = hurwiczValues.Max();

        // Шаг 3: Найти индекс строки с этим значением
        int optimalStrategyIndex = Array.IndexOf(hurwiczValues, maxHurwiczValue) + 1;

        return (maxHurwiczValue, optimalStrategyIndex);
    }
}