using System.ComponentModel;
using System.Runtime.CompilerServices;
using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers;

public class LUDecompositionSolver : ISLAESolver, INotifyPropertyChanged
{
    private bool isEnable;
    public string Name => "Метод LU-разложения";

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
        double[,] matrix = new double[n, n + 1];

        // Заполнение матрицы коэффициентов и вектора свободных членов
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = slae.Coefficients[i, j];
            }

            matrix[i, n] = slae.Constants[i];
        }

        // Вызов метода Calculation для решения системы
        List<double> results = Calculation(matrix);

        // Преобразование List<double> в массив
        return results.ToArray();
    }

    private List<double> Calculation(double[,] matrix)
    {
        double[,] L = new double[matrix.GetLength(1) - 1, matrix.GetLength(1) - 1];
        double[,] U = new double[matrix.GetLength(1) - 1, matrix.GetLength(1) - 1];
        double sum = 0;
        List<double> results = new List<double>();
        for (int k = 0; k < matrix.GetLength(1) - 1; k++)
        {
            for (int l = 0; l < matrix.GetLength(1) - 1; l++)
            {
                sum = 0;
                if (l >= k)
                {
                    for (int i = 0; i < k; i++)
                    {
                        sum += L[l, i] * U[i, k];
                    }

                    L[l, k] = matrix[l, k] - sum;
                }

                sum = 0;
                if (k < l)
                {
                    for (int i = 0; i < k; i++)
                    {
                        sum += L[k, i] * U[i, l];
                    }

                    U[k, l] = (matrix[k, l] - sum) / L[k, k];
                }
                else if (k == l) U[k, l] = 1;
            }
        }

        for (int k = 0; k < matrix.GetLength(1) - 1; k++)
        {
            sum = 0;
            for (int l = 0; l < matrix.GetLength(1) - 1; l++)
            {
                if (k > l)
                {
                    sum += L[k, l] * results[l];
                }
            }

            sum = matrix[k, matrix.GetLength(1) - 1] - sum;
            results.Add(sum / L[k, k]);
        }

        for (int k = matrix.GetLength(1) - 2; k >= 0; k--)
        {
            sum = 0;
            for (int l = matrix.GetLength(1) - 2; l >= 0; l--)
            {
                if (k < l)
                {
                    sum += U[k, l] * results[l];
                }
            }

            results[k] = results[k] - sum;
        }

        return results;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}