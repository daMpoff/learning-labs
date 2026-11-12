using System.ComponentModel;
using System.Runtime.CompilerServices;
using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers;

public class GaussJordanSolver : ISLAESolver, INotifyPropertyChanged
{
    private bool isEnable;
    public string Name => "Метод Гаусса-Жордана";

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
        int n = slae.RowsCount;
        double[,] A = new double[n, n];
        double[] b = new double[n];

        // Заполнение матрицы коэффициентов и вектора свободных членов
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                A[i, j] = slae.Coefficients[i, j];
            }

            b[i] = slae.Constants[i];
        }

        // Приведение системы уравнений к треугольной матрице методом Гаусса
        for (int k = 0; k < n; k++)
        {
            // Поиск ведущего элемента
            int maxRow = k;
            for (int i = k + 1; i < n; i++)
            {
                if (Math.Abs(A[i, k]) > Math.Abs(A[maxRow, k]))
                {
                    maxRow = i;
                }
            }

            // Перестановка строк
            if (maxRow != k)
            {
                for (int j = 0; j < n; j++)
                {
                    double temp = A[k, j];
                    A[k, j] = A[maxRow, j];
                    A[maxRow, j] = temp;
                }

                double tempB = b[k];
                b[k] = b[maxRow];
                b[maxRow] = tempB;
            }

            // Нормировка строки
            double pivot = A[k, k];
            for (int j = k; j < n; j++)
            {
                A[k, j] /= pivot;
            }

            b[k] /= pivot;

            // Обратная подстановка
            for (int i = 0; i < n; i++)
            {
                if (i != k)
                {
                    double factor = A[i, k];
                    for (int j = k; j < n; j++)
                    {
                        A[i, j] -= factor * A[k, j];
                    }

                    b[i] -= factor * b[k];
                }
            }
        }

        // Приведение СЛАУ к единичной матрице
        for (int k = n - 1; k >= 0; k--)
        {
            b[k] /= A[k, k];
            A[k, k] = 1;
            for (int i = 0; i < k; i++)
            {
                b[i] -= A[i, k] * b[k];
                A[i, k] = 0;
            }
        }

        return b;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}