using System.ComponentModel;
using System.Runtime.CompilerServices;
using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers
{
    public class MatrixSolver : ISLAESolver, INotifyPropertyChanged
    {
        private bool isEnable;
        public string Name => "Матричный метод";

        public bool IsEnable
        {
            get => isEnable;
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;
                    OnPropertyChanged(nameof(IsEnable));
                }
            }
        }

        public double[] Solve(SLAEModel slae)
        {
            if (slae.VariablesCount != slae.RowsCount)
            {
                throw new InvalidOperationException("Количество уравнений и переменных не совпадает!");
            }
            // Находим обратную матрицу A
            double[,] A = slae.Coefficients;
            double[,] inverseMatrix = InverseMatrix(A);

            // Умножаем обратную матрицу на вектор B
            double[] B = slae.Constants;
            double[] x = MatrixVectorMultiply(inverseMatrix, B);

            return x;
        }

        private double[,] InverseMatrix(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] result = new double[n, n];
            double det = Determinant(matrix);

            if (Math.Abs(det) < 0.0000001)
            {
                throw new InvalidOperationException("Матрица вырожденная, обратной матрицы не существует.");
            }

            double[,] adjugateMatrix = AdjugateMatrix(matrix);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = adjugateMatrix[i, j] / det;
                }
            }

            return result;
        }

        private double Determinant(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            if (n == 1)
            {
                return matrix[0, 0];
            }
            else if (n == 2)
            {
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            }

            double det = 0;
            for (int j = 0; j < n; j++)
            {
                double[,] minor = Minor(matrix, 0, j);
                det += Math.Pow(-1, j) * matrix[0, j] * Determinant(minor);
            }

            return det;
        }

        private double[,] Minor(double[,] matrix, int row, int col)
        {
            int n = matrix.GetLength(0);
            double[,] minor = new double[n - 1, n - 1];

            for (int i = 0, r = 0; i < n; i++)
            {
                if (i == row) continue;
                for (int j = 0, c = 0; j < n; j++)
                {
                    if (j == col) continue;
                    minor[r, c] = matrix[i, j];
                    c++;
                }
                r++;
            }

            return minor;
        }

        private double[,] AdjugateMatrix(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] adjugate = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double[,] minor = Minor(matrix, i, j);
                    adjugate[j, i] = Math.Pow(-1, i + j) * Determinant(minor);
                }
            }

            return adjugate;
        }

        private double[] MatrixVectorMultiply(double[,] matrix, double[] vector)
        {
            int n = matrix.GetLength(0);
            double[] result = new double[n];

            for (int i = 0; i < n; i++)
            {
                result[i] = 0;
                for (int j = 0; j < n; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
            }

            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}