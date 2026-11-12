using System.Collections.Generic;

namespace SimplexMethod
{
    public class SimplexMethod
    {
        private readonly double[] objectiveFunction;
        private readonly double[,] systemLimits;
        private double[,] simplexMatrix;
        private double minOrMax;

        public SimplexMethod(double[] objectiveFunction, double[,] systemLimits)
        {
            this.objectiveFunction = objectiveFunction;
            this.systemLimits = systemLimits;
            minOrMax = objectiveFunction[objectiveFunction.Length - 1];
            InitializeSimplexMatrix();
        }

        private void InitializeSimplexMatrix()
        {
            int rows = systemLimits.GetLength(0) + 2;
            int cols = systemLimits.GetLength(1);
            simplexMatrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i == 0 && j != 0)
                    {
                        simplexMatrix[i, j] = j == cols - 1 ? 0 : j;
                    }
                    else if (i != 0 && j == 0)
                    {
                        simplexMatrix[i, j] = i == 1 ? 0 : cols - 1 + i - 2;
                    }
                    else if (i != 0 && j != 0)
                    {
                        if (i == 1)
                        {
                            simplexMatrix[i, j] = j == objectiveFunction.Length ? 0 : -objectiveFunction[j - 1] * minOrMax;
                        }
                        else
                        {
                            simplexMatrix[i, j] = systemLimits[i - 2, j] * systemLimits[i - 2, 0];
                        }
                    }
                }
            }
        }

        public List<double> Calculate()
        {
            return AdmissibilityChecker.CheckAdmissibility(simplexMatrix, minOrMax);
        }
    }
}
