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
        double sum = 0;
        List<double> results = new List<double>();
        double[,] upperTriangularMatrix = new double[slae.VariablesCount, slae.VariablesCount];

        for (int i = 0; i < slae.VariablesCount; i++)
        {
            for (int j = 0; j < slae.VariablesCount; j++)
            {
                sum = 0;
                if (i > j) upperTriangularMatrix[i, j] = 0;
                else if (i == j)
                {
                    for (int k = 0; k < i; k++)
                    {
                        sum += Math.Pow(upperTriangularMatrix[k, i], 2);
                    }

                    upperTriangularMatrix[i, i] = Math.Sqrt(slae.Coefficients[i, i] - sum);
                }
                else
                {
                    for (int k = 0; k < i; k++)
                    {
                        sum += upperTriangularMatrix[k, i] * upperTriangularMatrix[k, j];
                    }

                    upperTriangularMatrix[i, j] = (slae.Coefficients[i, j] - sum) / upperTriangularMatrix[i, i];
                }
            }
        }

        double[,] transposedUpperTriangularMatrix = new double[slae.VariablesCount, slae.VariablesCount];
        for (int i = 0; i < slae.VariablesCount; i++)
        {
            for (int j = 0; j < slae.VariablesCount; j++)
            {
                transposedUpperTriangularMatrix[j, i] = upperTriangularMatrix[i, j];
            }
        }

        for (int i = 0; i < slae.VariablesCount; i++)
        {
            sum = 0;
            for (int j = 0; j < slae.VariablesCount; j++)
            {
                if (i > j) sum += transposedUpperTriangularMatrix[i, j] * results[j];
            }

            results.Add((slae.Constants[i] - sum) / transposedUpperTriangularMatrix[i, i]);
        }

        for (int i = slae.VariablesCount - 1; i >= 0; i--)
        {
            sum = 0;
            for (int j = slae.VariablesCount - 1; j >= 0; j--)
            {
                if (i < j) sum += upperTriangularMatrix[i, j] * results[j];
            }

            results[i] = (results[i] - sum) / upperTriangularMatrix[i, i];
        }

        return results.ToArray();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}