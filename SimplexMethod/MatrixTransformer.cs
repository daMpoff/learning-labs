namespace SimplexMethod;

public static class MatrixTransformer
{
    public static double[,] Transform(double[,] simplexMatrix, double minOrMax, int k, int l)
    {
        double[,] newSimplexMatrix = new double[simplexMatrix.GetLength(0), simplexMatrix.GetLength(1)];

        for (int i = 0; i < simplexMatrix.GetLength(0); i++)
        {
            newSimplexMatrix[i, 0] = i == k ? simplexMatrix[0, l] : simplexMatrix[i, 0];
        }

        for (int j = 0; j < simplexMatrix.GetLength(1); j++)
        {
            newSimplexMatrix[0, j] = j == l ? simplexMatrix[k, 0] : simplexMatrix[0, j];
        }

        for (int i = 1; i < simplexMatrix.GetLength(0); i++)
        {
            for (int j = 1; j < simplexMatrix.GetLength(1); j++)
            {
                if (i == k && j == l)
                {
                    newSimplexMatrix[i, j] = 1 / simplexMatrix[k, l];
                }
                else if (j == l)
                {
                    newSimplexMatrix[i, j] = simplexMatrix[i, j] * (-1 / simplexMatrix[k, l]);
                }
                else if (i == k)
                {
                    newSimplexMatrix[i, j] = simplexMatrix[i, j] / simplexMatrix[k, l];
                }
                else
                {
                    newSimplexMatrix[i, j] = simplexMatrix[i, j] - (simplexMatrix[i, l] * simplexMatrix[k, j] / simplexMatrix[k, l]);
                }
            }
        }

        return newSimplexMatrix;
    }
}