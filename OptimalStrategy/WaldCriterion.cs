namespace OptimalStrategy;

public class WaldCriterion : ICriterion
{
    private int[,] matrix;

    public WaldCriterion(int[,] inputMatrix)
    {
        matrix = inputMatrix;
    }

    public (double value, int strategy) FindOptimalStrategy()
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        // Шаг 1: Найти минимумы строк
        int[] rowMinimal = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            int min = int.MaxValue;
            for (int j = 0; j < cols; j++)
            {
                if (matrix[i, j] < min)
                {
                    min = matrix[i, j];
                }
            }

            rowMinimal[i] = min;
        }

        // Шаг 2: Найти максимум среди минимумов
        int maxOfMinimals = rowMinimal.Max();

        // Шаг 3: Найти индекс строки с этим максимумом
        int optimalStrategyIndex = Array.IndexOf(rowMinimal, maxOfMinimals) + 1;

        return (maxOfMinimals, optimalStrategyIndex);
    }
}