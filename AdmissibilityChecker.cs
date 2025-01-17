using System;
using System.Collections.Generic;

namespace SimplexMethod;

public static class AdmissibilityChecker
{
    public static List<double> CheckAdmissibility(double[,] simplexMatrix, double minOrMax)
    {
        int k = FindLeadingRow(simplexMatrix, out bool acceptable);
        if (acceptable)
        {
            return OptimalityChecker.CheckOptimality(simplexMatrix, minOrMax);
        }

        int l = FindLeadingColumn(simplexMatrix, k, out acceptable);
        if (!acceptable)
        {
            Console.WriteLine("Условия задачи несовместны и решений у нее нет.");
            return new List<double>();
        }

        simplexMatrix = MatrixTransformer.Transform(simplexMatrix, minOrMax, k, l);
        return CheckAdmissibility(simplexMatrix, minOrMax);
    }

    private static int FindLeadingRow(double[,] symplexMatrix, out bool acceptable)
    {
        acceptable = true;
        double max = 0;
        int k = 0;

        for (int i = 2; i < symplexMatrix.GetLength(0); i++)
        {
            if (symplexMatrix[i, symplexMatrix.GetLength(1) - 1] < 0)
            {
                acceptable = false;
                if (Math.Abs(symplexMatrix[i, symplexMatrix.GetLength(1) - 1]) > max)
                {
                    max = Math.Abs(symplexMatrix[i, symplexMatrix.GetLength(1) - 1]);
                    k = i;
                }
            }
        }

        return k;
    }

    private static int FindLeadingColumn(double[,] symplexMatrix, int k, out bool acceptable)
    {
        acceptable = true;
        double max = 0;
        int l = 0;

        for (int j = 1; j < symplexMatrix.GetLength(1) - 1; j++)
        {
            if (symplexMatrix[k, j] < 0)
            {
                if (Math.Abs(symplexMatrix[k, j]) > max)
                {
                    max = Math.Abs(symplexMatrix[k, j]);
                    l = j;
                }
            }
        }

        if (max == 0)
        {
            acceptable = false;
        }

        return l;
    }
}