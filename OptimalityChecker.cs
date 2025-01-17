using System;
using System.Collections.Generic;

namespace SimplexMethod;

public static class OptimalityChecker
{
    public static List<double> CheckOptimality(double[,] simplexMatrix, double minOrMax)
    {
        bool optimally = true;
        double max = 0;
        int l = 0;

        for (int j = 1; j < simplexMatrix.GetLength(1) - 1; j++)
        {
            if (simplexMatrix[1, j] < 0)
            {
                optimally = false;
                if (Math.Abs(simplexMatrix[1, j]) > max)
                {
                    max = Math.Abs(simplexMatrix[1, j]);
                    l = j;
                }
            }
        }

        if (optimally)
        {
            return ExtractResults(simplexMatrix, minOrMax);
        }

        int k = FindLeadingRow(simplexMatrix, l);
        if (k == -1)
        {
            Console.WriteLine("ОДР задачи не ограниченна");
            return new List<double>();
        }

        simplexMatrix = MatrixTransformer.Transform(simplexMatrix, minOrMax, k, l);
        return AdmissibilityChecker.CheckAdmissibility(simplexMatrix, minOrMax);
    }

    private static int FindLeadingRow(double[,] simplexMatrix, int l)
    {
        double min = double.MaxValue;
        int k = -1;

        for (int i = 2; i < simplexMatrix.GetLength(0); i++)
        {
            if (simplexMatrix[i, simplexMatrix.GetLength(1) - 1] / simplexMatrix[i, l] < min &&
                simplexMatrix[i, simplexMatrix.GetLength(1) - 1] > 0 && simplexMatrix[i, l] > 0)
            {
                min = simplexMatrix[i, simplexMatrix.GetLength(1) - 1] / simplexMatrix[i, l];
                k = i;
            }
        }

        return k;
    }

    private static List<double> ExtractResults(double[,] simplexMatrix, double minOrMax)
    {
        List<double> results = new List<double>
        {
            simplexMatrix[1, simplexMatrix.GetLength(1) - 1] * minOrMax
        };

        for (int q = 1; q < simplexMatrix.GetLength(1) - 1; q++)
        {
            bool found = false;
            for (int i = 2; i < simplexMatrix.GetLength(0); i++)
            {
                if (simplexMatrix[i, 0] == q)
                {
                    found = true;
                    results.Add(simplexMatrix[i, simplexMatrix.GetLength(1) - 1]);
                    break;
                }
            }

            if (!found)
            {
                results.Add(0);
            }
        }

        return results;
    }
}