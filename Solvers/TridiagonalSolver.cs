using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers
{
    public class TridiagonalSolver : ISLAESolver
    {
        public string Name => "Метод прогонки";
        public bool IsEnable { get; set; }
        public double[] Solve(SLAEModel slae)
        {
            int n = slae.RowsCount;
            //if (slae.VariablesCount < 3)
            //{
            //    throw new ArgumentException("Метод прогонки применим только к трёхдиагональным системам.");
            //}

            double[] a = new double[n];   // нижняя диагональ (a_0 не используется)
            double[] b = new double[n];   // главная диагональ
            double[] c = new double[n];   // верхняя диагональ (c_n не используется)
            double[] d = new double[n];   // правая часть

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
    }
}
