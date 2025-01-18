namespace OptimalStrategy;

public class LaplaceCriterion : ICriterion
{
    private int[,] matrix;

    public LaplaceCriterion(int[,] inputMatrix)
    {
        matrix = inputMatrix;
    }

    public (double value, int strategy) FindOptimalStrategy()
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        // Шаг 1: Вычислить средние значения для каждой строки
        double[] rowAverages = new double[rows];
        for (int i = 0; i < rows; i++)
        {
            double sum = 0;
            for (int j = 0; j < cols; j++)
            {
                sum += matrix[i, j];
            }

            rowAverages[i] = sum / cols;
        }

        // Шаг 2: Найти максимальное среднее значение
        double maxAverage = rowAverages.Max();

        // Шаг 3: Найти индекс строки с этим максимальным значением
        int optimalStrategyIndex = Array.IndexOf(rowAverages, maxAverage) + 1;

        return (maxAverage, optimalStrategyIndex);
    }
}