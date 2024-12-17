using System.ComponentModel;
using System.Runtime.CompilerServices;
using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers
{
    public class GaussianSolver : ISLAESolver, INotifyPropertyChanged
    {
        private bool isEnable;
        public string Name { get => "Метод Гаусса"; }
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

            // Прямой ход
            for (int i = 0; i < n; i++)
            {
                // Поиск максимального элемента для улучшения точности
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(slae.Coefficients[k, i]) > Math.Abs(slae.Coefficients[maxRow, i]))
                    {
                        maxRow = k;
                    }
                }

                // Меняем местами строки
                for (int k = i; k < n; k++)
                {
                    double temp = slae.Coefficients[maxRow, k];
                    slae.Coefficients[maxRow, k] = slae.Coefficients[i, k];
                    slae.Coefficients[i, k] = temp;
                }
                double tempRight = slae.Constants[maxRow];
                slae.Constants[maxRow] = slae.Constants[i];
                slae.Constants[i] = tempRight;

                // Приведение к единице ведущего элемента строки
                double divisor = slae.Coefficients[i, i];
                for (int j = i; j < n; j++)
                {
                    slae.Coefficients[i, j] /= divisor;
                }
                slae.Constants[i] /= divisor;

                // Прямой ход (зануление элементов ниже главной диагонали)
                for (int k = i + 1; k < n; k++)
                {
                    double factor = slae.Coefficients[k, i];
                    for (int j = i; j < n; j++)
                    {
                        slae.Coefficients[k, j] -= factor * slae.Coefficients[i, j];
                    }
                    slae.Constants[k] -= factor * slae.Constants[i];
                }
            }

            // Обратный ход
            double[] result = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                result[i] = slae.Constants[i];
                for (int j = i + 1; j < n; j++)
                {
                    result[i] -= slae.Coefficients[i, j] * result[j];
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
