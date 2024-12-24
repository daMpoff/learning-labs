namespace SNE.Utils;

public class LinearSystemSolver
{
    public static double[] SolveLinearSystem(SquareMatrix matrix, double[] b)
    {
        int n = matrix.Size;
        double[,] a = matrix.Data;
        double[] x = new double[n];

        // Прямой ход метода Гаусса
        for (int k = 0; k < n; k++)
        {
            for (int i = k + 1; i < n; i++)
            {
                double factor = a[i, k] / a[k, k];
                for (int j = k; j < n; j++)
                {
                    a[i, j] -= factor * a[k, j];
                }

                b[i] -= factor * b[k];
            }
        }

        // Обратный ход метода Гаусса
        for (int k = n - 1; k >= 0; k--)
        {
            double sum = 0;
            for (int j = k + 1; j < n; j++)
            {
                sum += a[k, j] * x[j];
            }

            x[k] = (b[k] - sum) / a[k, k];
        }

        return x;
    }
}