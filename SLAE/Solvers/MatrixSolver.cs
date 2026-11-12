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
            double[,] augmentedMatrix = new double[n, 2 * n];

            // Создаем расширенную матрицу
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmentedMatrix[i, j] = matrix[i, j];
                }
                augmentedMatrix[i, i + n] = 1;
            }

            // Приводим левую часть к единичной матрице
            for (int i = 0; i < n; i++)
            {
                // Делаем диагональный элемент равным 1
                double factor = 1.0 / augmentedMatrix[i, i];
                for (int j = 0; j < 2 * n; j++)
                {
                    augmentedMatrix[i, j] *= factor;
                }

                // Обнуляем элементы ниже и выше диагонали
                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        factor = augmentedMatrix[k, i];
                        for (int j = 0; j < 2 * n; j++)
                        {
                            augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                        }
                    }
                }
            }

            // Правая часть матрицы теперь содержит обратную матрицу
            double[,] inverseMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    inverseMatrix[i, j] = augmentedMatrix[i, j + n];
                }
            }

            return inverseMatrix;
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