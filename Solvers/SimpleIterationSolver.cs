using SLAE.Interfaces;
using SLAE.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAE.Solvers
{
    public class SimpleIterationSolver : ISLAESolver, INotifyPropertyChanged
    {
        private bool isEnable;
        public string Name => "Метод простых итераций";

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
            int m = slae.RowsCount;
            int n = slae.VariablesCount;
            double[,] matrix = new double[m, n + 1];
            double[] oldValues = new double[n];
            double[] newValues = new double[n];
            double fault = 0.001; ; // Погрешность
            // Заполняем коэффициентами и свободными членами матрицу, значениями из модели
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = slae.Coefficients[i, j];
                }

                matrix[i, n] = slae.Constants[i];
            }

            // Попытка перестановки строк для получения диагонально доминирующей матрицы
            if (!IsDiagonallyDominant(matrix, m, n))
            {
                throw new Exception("Матрица не является диагонально доминирующей. Преобразование системы невозможно.");
            }

            int iteration = 0;
            bool hasConvergence = false;
            // Итерационный процесс до достижения необходимой точности
            while (!hasConvergence)
            {
                iteration++;
                hasConvergence = true;
                for (int i = 0; i < n; i++)
                {
                    double sum = matrix[i, n];
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                        {
                            sum -= matrix[i, j] * oldValues[j];
                        }
                    }

                    newValues[i] = sum / matrix[i, i];
                }

                for (int i = 0; i < n; i++)
                {
                    if (Math.Abs(newValues[i] - oldValues[i]) > fault)
                    {
                        hasConvergence = false;
                        break;
                    }
                }

                // Обновление старых значений переменных
                Array.Copy(newValues, oldValues, n);
            }

            MessageBox.Show($"{Name}\nРешение сходится до заданной точности за {iteration} итераций.");
            return newValues;
        }

        // Метод для обеспечения диагональной доминантности матрицы
        private static bool IsDiagonallyDominant(double[,] matrix, int m, int n)
        {
            for (int i = 0; i < m; i++)
            {
                if (!IsRowDiagonallyDominant(matrix, i, n))
                {
                    bool found = false;
                    for (int k = i + 1; k < m; k++)
                    {
                        SwapRows(matrix, i, k, n + 1);
                        if (IsRowDiagonallyDominant(matrix, i, n))
                        {
                            found = true;
                            break;
                        }

                        SwapRows(matrix, i, k, n + 1);
                    }

                    if (!found)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Метод проверки, является ли строка диагонально доминирующей
        private static bool IsRowDiagonallyDominant(double[,] matrix, int i, int n)
        {
            double sum = 0;
            for (int j = 0; j < n; j++)
            {
                if (i != j)
                {
                    sum += Math.Abs(matrix[i, j]);
                }
            }

            return Math.Abs(matrix[i, i]) > sum;
        }

        // Метод для обмена строк в матрице
        private static void SwapRows(double[,] matrix, int row1, int row2, int columns)
        {
            for (int j = 0; j < columns; j++)
            {
                double temp = matrix[row1, j];
                matrix[row1, j] = matrix[row2, j];
                matrix[row2, j] = temp;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}