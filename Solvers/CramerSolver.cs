using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers
{
    public class CramerSolver : ISLAESolver
    {
        public string Name => "Метод Крамера";

        public bool IsEnable { get; set; }

        public double[] Solve(SLAEModel slae)
        {
            if (slae.VariablesCount != slae.RowsCount)
            {
                throw new Exception("Количество уравнений не равно количеству переменных!");
            }
            // Создаем расширенную матрицу
            double[,] augmentedMatrix = new double[slae.RowsCount, slae.VariablesCount + 1];
            for (int i = 0; i < slae.RowsCount; i++)
            {
                for (int j = 0; j < slae.VariablesCount; j++)
                {
                    augmentedMatrix[i, j] = slae.Coefficients[i, j];
                }
                augmentedMatrix[i, slae.VariablesCount] = slae.Constants[i];
            }

            // Используем метод Крамера для решения СЛАУ
            List<double> result = Calculation(augmentedMatrix);

            // Преобразуем результат в массив
            return result.ToArray();
        }
        public List<double> Calculation(double[,] matrix)
        {
            List<double> result = new List<double>();
            double[,] matrixDet = new double[matrix.GetLength(0), matrix.GetLength(0)];

            // Создаем матрицу коэффициентов
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    matrixDet[i, j] = matrix[i, j];
                }
            }

            double det = Determinant(matrixDet);

            for (int m = 0; m < matrix.GetLength(0); m++)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        if (m == j) matrixDet[i, j] = matrix[i, matrix.GetLength(1) - 1];
                        else matrixDet[i, j] = matrix[i, j];
                    }
                }
                result.Add(Determinant(matrixDet) / det);
            }

            return result;
        }

        // Метод Determinant из класса CramerSolver
        public double Determinant(double[,] matrixDet)
        {
            if (matrixDet.GetLength(0) == 2)
            {
                return matrixDet[0, 0] * matrixDet[1, 1] - matrixDet[1, 0] * matrixDet[0, 1];
            }

            double det = 0;
            double[,] minor = new double[matrixDet.GetLength(0) - 1, matrixDet.GetLength(0) - 1];

            for (int i = 0; i < matrixDet.GetLength(0); i++)
            {
                int minorIndex = 0;
                for (int m = 1; m < matrixDet.GetLength(0); m++)
                {
                    minorIndex = 0;
                    for (int n = 0; n < matrixDet.GetLength(0); n++)
                    {
                        if (n == i) continue;
                        minor[m - 1, minorIndex++] = matrixDet[m, n];
                    }
                }
                det += matrixDet[0, i] * Determinant(minor) * (i % 2 == 0 ? 1 : -1);
            }
            return det;
        }
    }
}
