namespace OptimalStrategy;

public class SavageCriterion : ICriterion
{
    private int[,] matrix;

    public SavageCriterion(int[,] inputMatrix)
    {
        matrix = inputMatrix;
    }

    private int[,] CalculateRiskMatrix()
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int[,] riskMatrix = new int[rows, cols];

        // Вычисление максимумов по столбцам
        int[] colMax = new int[cols];
        for (int j = 0; j < cols; j++)
        {
            colMax[j] = matrix.GetLength(0) > 0 ? matrix[0, j] : int.MinValue;
            for (int i = 1; i < rows; i++)
            {
                if (matrix[i, j] > colMax[j])
                {
                    colMax[j] = matrix[i, j];
                }
            }
        }

        // Вычисление матрицы рисков
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                riskMatrix[i, j] = colMax[j] - matrix[i, j];
            }
        }

        return riskMatrix;
    }

    public (double value, int strategy) FindOptimalStrategy()
    {
        int[,] riskMatrix = CalculateRiskMatrix();
        int rows = riskMatrix.GetLength(0);
        int cols = riskMatrix.GetLength(1);

        // Шаг 1: Найти максимумы строк
        int[] rowMaximals = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            int max = int.MinValue;
            for (int j = 0; j < cols; j++)
            {
                if (riskMatrix[i, j] > max)
                {
                    max = riskMatrix[i, j];
                }
            }

            rowMaximals[i] = max;
        }

        // Шаг 2: Найти минимум среди максимумов
        int minOfMaximals = rowMaximals.Min();

        // Шаг 3: Найти индекс строки с этим минимумом
        int optimalStrategyIndex = Array.IndexOf(rowMaximals, minOfMaximals) + 1;

        return (minOfMaximals, optimalStrategyIndex);
    }
}