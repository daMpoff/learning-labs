using System.ComponentModel;
using System.Runtime.CompilerServices;
using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers
{
    public class TridiagonalSolver : ISLAESolver, INotifyPropertyChanged
    {
        private bool isEnable;
        public string Name => "Метод прогонки";

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

            // Проверка, что матрица является трёхдиагональной и диагональные элементы не равны нулю
            if (!IsTridiagonalMatrix(slae.Coefficients, n))
            {
                throw new InvalidOperationException(
                    "Матрица не является трёхдиагональной или содержит нули на диагонали.");
            }

            double[] a = new double[n]; // нижняя диагональ (a_0 не используется)
            double[] b = new double[n]; // главная диагональ
            double[] c = new double[n]; // верхняя диагональ (c_n не используется)
            double[] d = new double[n]; // правая часть

            // Заполнение массивов a, b, c и d из матрицы коэффициентов и вектора свободных членов
            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                {
                    a[i] = slae.Coefficients[i, i - 1];
                }

                b[i] = slae.Coefficients[i, i];
                if (i < n - 1)
                {
                    c[i] = slae.Coefficients[i, i + 1];
                }

                d[i] = slae.Constants[i];
            }

            return TridiagonalSolve(a, b, c, d);
        }

        // Метод прогонки (метод Томаса) для решения трёхдиагональной системы
        private static double[] TridiagonalSolve(double[] a, double[] b, double[] c, double[] d)
        {
            int n = b.Length;
            double[] p = new double[n];
            double[] q = new double[n];
            double[] x = new double[n];

            // Прямой ход
            p[0] = -c[0] / b[0];
            q[0] = d[0] / b[0];

            for (int i = 1; i < n; i++)
            {
                double exp = b[i] + a[i] * p[i - 1];
                p[i] = -c[i] / exp;
                q[i] = (d[i] - a[i] * q[i - 1]) / exp;
            }

            // Обратный ход
            x[n - 1] = q[n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = p[i] * x[i + 1] + q[i];
            }

            return x;
        }

        // Проверка, что матрица является трёхдиагональной и диагональные элементы не равны нулю
        private static bool IsTridiagonalMatrix(double[,] matrix, int n)
        {
            for (int i = 0; i < n; i++)
            {
                // Проверка, что элементы вне трёх диагоналей равны нулю
                for (int j = 0; j < n; j++)
                {
                    if (i != j && i != j - 1 && i != j + 1)
                    {
                        if (matrix[i, j] != 0)
                        {
                            return false;
                        }
                    }
                }

                // Проверка, что диагональные элементы не равны нулю
                if (matrix[i, i] == 0)
                {
                    return false;
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
}