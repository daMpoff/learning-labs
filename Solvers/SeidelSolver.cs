using SLAE.Interfaces;
using SLAE.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAE.Solvers
{
    public class SeidelSolver : ISLAESolver, INotifyPropertyChanged
    {
        private bool isEnable;
        public string Name => "Метод Зейделя";
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
            double[] values = new double[slae.VariablesCount];
            double fault; // погрешность

            fault = 0.001;

            // Попытка перестановки строк для получения диагонально доминирующей матрицы
            if (!EnsureDiagonalDominance(slae.Coefficients, slae.RowsCount, slae.VariablesCount))
            {
                throw new Exception("Матрица не является диагонально доминирующей, даже после перестановки строк.");
            }

            // Итерационный метод Зейделя
            int iteration = 0;
            bool hasConvergence = false; // имеет сходимость

            while (!hasConvergence)
            {
                iteration++;
                hasConvergence = true;

                for (int i = 0; i < slae.VariablesCount; i++)
                {
                    double sum = slae.Constants[i]; // свободный член
                    for (int j = 0; j < slae.VariablesCount; j++)
                    {
                        if (j != i)
                        {
                            sum -= slae.Coefficients[i, j] * values[j];
                        }
                    }
                    double newValue = sum / slae.Coefficients[i, i];

                    if (Math.Abs(newValue - values[i]) > fault)
                    {
                        hasConvergence = false;
                    }

                    values[i] = newValue;
                }
            }

            // Вывод результатов
            MessageBox.Show($"{Name}\nРешение сходится до заданной точности за {iteration} итераций.");
            return values;
        }

        // Метод проверки диагональной доминантности
        private static bool IsDiagonallyDominant(double[,] matrix, int m, int n)
        {
            for (int i = 0; i < m; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        sum += Math.Abs(matrix[i, j]);
                    }
                }
                if (Math.Abs(matrix[i, i]) < sum)
                {
                    return false;
                }
            }
            return true;
        }

        // Метод для перестановки строк и обеспечения диагональной доминантности
        private static bool EnsureDiagonalDominance(double[,] matrix, int m, int n)
        {
            for (int i = 0; i < m; i++)
            {
                // Если строка i не является диагонально доминирующей, ищем строку для перестановки
                if (!IsRowDiagonallyDominant(matrix, i, n))
                {
                    bool found = false;
                    for (int k = i + 1; k < m; k++)
                    {
                        // Пробуем переставить строку i со строкой k
                        SwapRows(matrix, i, k, n + 1);
                        if (IsRowDiagonallyDominant(matrix, i, n))
                        {
                            found = true;
                            break;
                        }
                        // Если перестановка не помогла, возвращаем строки на место
                        SwapRows(matrix, i, k, n + 1);
                    }
                    if (!found)
                    {
                        return false; // Не удалось обеспечить диагональную доминантность
                    }
                }
            }
            return true;
        }

        // Метод проверки, является ли строка i диагонально доминирующей
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
            return Math.Abs(matrix[i, i]) >= sum;
        }

        // Метод для перестановки строк матрицы
        private static void SwapRows(double[,] matrix, int row1, int row2, int columns)
        {
            for (int j = 0; j < columns - 1; j++)
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
