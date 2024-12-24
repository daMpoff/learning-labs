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
            return SolveSLAE(slae.Coefficients, slae.Constants);
        }

        private double[] SolveSLAE(double[,] A, double[] B)
        {
            int n = B.Length;

            for (int k = 0; k < n; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    double factor = A[i, k] / A[k, k];
                    for (int j = k; j < n; j++)
                    {
                        A[i, j] -= factor * A[k, j];
                    }

                    B[i] -= factor * B[k];
                }
            }

            double[] X = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                X[i] = B[i];
                for (int j = i + 1; j < n; j++)
                {
                    X[i] -= A[i, j] * X[j];
                }

                X[i] /= A[i, i];
            }

            return X;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}