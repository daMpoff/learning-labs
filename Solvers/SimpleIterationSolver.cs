using SLAE.Interfaces;
using SLAE.Models;
using System.Windows;

namespace SLAE.Solvers
{
    public class SimpleIterationSolver : ISLAESolver
    {
        public string Name => "Метод простых итераций";
        public bool IsEnable { get; set; }

        public double[] Solve(SLAEModel slae)
        {
            int m = slae.RowsCount;
            int n = slae.VariablesCount;
            double[,] matrix = new double[m, n + 1];
            double[] oldValues = new double[n];
            double[] newValues = new double[n];
            double fault; // погрешность
            // Копируем коэффициенты и свободные члены в матрицу
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = slae.Coefficients[i, j];
                }
                matrix[i, n] = slae.Constants[i];
            }
            // Попытка перестановки строк для получения диагонально доминирующей матрицы
            if (!EnsureDiagonalDominance(matrix, m, n))
            {
                throw new Exception("Матрица не является диагонально доминирующей. Преобразование системы невозможно.");
            }
            // Ввод допустимой погрешности (можно задать по умолчанию или передать как параметр)
            fault = 0.0001;
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
                // Проверка сходимости
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
            MessageBox.Show($"Решение сходится до заданной точности за {iteration} итераций.");
            return newValues;
        }

        // Метод проверки, является ли матрица диагонально доминирующей
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
                if (Math.Abs(matrix[i, i]) <= sum)
                {
                    return false;
                }
            }
            return true;
        }
        // Метод для обеспечения диагональной доминантности матрицы
        private static bool EnsureDiagonalDominance(double[,] matrix, int m, int n)
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
    }
}