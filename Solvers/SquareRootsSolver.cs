using System.ComponentModel;
using System.Runtime.CompilerServices;
using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers;

public class SquareRootsSolver : ISLAESolver, INotifyPropertyChanged
{
    private bool isEnable;
    public string Name { get; } = "Метод квадратных корней";

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
        int n = slae.VariablesCount;
        double[,] A = slae.Coefficients;
        double[] b = slae.Constants;

        // Проверка симметричности матрицы
        if (!IsSymmetric(A, n))
        {
            throw new InvalidOperationException("Матрица не является симметричной.");
        }

        // Шаг 1: Разложение матрицы A на U^T * U
        double[,] U = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = i; j < n; j++)
            {
                double sum = 0;
                if (j == i)
                {
                    for (int k = 0; k < i; k++)
                    {
                        sum += Math.Pow(U[k, i], 2);
                    }

                    U[i, i] = Math.Sqrt(A[i, i] - sum);
                    if (double.IsNaN(U[i, i]) || U[i, i] == 0)
                    {
                        throw new InvalidOperationException("Матрица не является положительно определенной.");
                    }
                }
                else
                {
                    for (int k = 0; k < i; k++)
                    {
                        sum += U[k, i] * U[k, j];
                    }

                    U[i, j] = (A[i, j] - sum) / U[i, i];
                }
            }
        }

        // Шаг 2: Решение системы U^T * y = b
        double[] y = new double[n];
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < i; j++)
            {
                sum += U[j, i] * y[j];
            }

            y[i] = (b[i] - sum) / U[i, i];
        }

        // Шаг 3: Решение системы U * x = y
        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            double sum = 0;
            for (int j = i + 1; j < n; j++)
            {
                sum += U[i, j] * x[j];
            }

            x[i] = (y[i] - sum) / U[i, i];
        }

        return x;
    }

    private bool IsSymmetric(double[,] A, int n)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (A[i, j] != A[j, i])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}