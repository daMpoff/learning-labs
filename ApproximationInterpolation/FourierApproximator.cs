using MathNet.Symbolics;

namespace ApproximationInterpolation;

public class FourierApproximator : IApproximator
{
    public string Name => $"Ряд Фурье (ст. {Degree})";
    public int Degree { get; }
    public string FunctionString { get; }
    private double[] a;
    private double[] b;

    public FourierApproximator(string function, int degree)
    {
        Degree = degree;
        FunctionString = function;
        ComputeCoefficients(function, degree);
    }

    public void SetPoints(List<(double x, double y)> points)
    {
        /* Не используется */
    }

    private void ComputeCoefficients(string func, int n)
    {
        // Вычисляем коэффициенты Фурье на [-pi, pi]
        var expr = Infix.ParseOrThrow(func);
        int M = 2048; // Число точек для интегрирования
        double[] xs = new double[M];
        double[] ys = new double[M];
        double h = 2 * Math.PI / M;
        for (int i = 0; i < M; i++)
        {
            xs[i] = -Math.PI + i * h;
            ys[i] = Evaluate.Evaluate(new Dictionary<string, FloatingPoint> { ["x"] = xs[i] }, expr).RealValue;
        }

        a = new double[n + 1];
        b = new double[n + 1];
        // a0 отдельно
        a[0] = (1 / Math.PI) * Integrate(xs, ys, h);
        // остальные
        for (int k = 1; k <= n; k++)
        {
            // f(x) * cos(kx)
            double[] ycos = new double[M];
            double[] ysin = new double[M];
            for (int i = 0; i < M; i++)
            {
                ycos[i] = ys[i] * Math.Cos(k * xs[i]);
                ysin[i] = ys[i] * Math.Sin(k * xs[i]);
            }

            a[k] = (1 / Math.PI) * Integrate(xs, ycos, h);
            b[k] = (1 / Math.PI) * Integrate(xs, ysin, h);
        }
    }

    // Численный интеграл (метод трапеций)
    private double Integrate(double[] xs, double[] ys, double h)
    {
        double sum = 0;
        for (int i = 1; i < xs.Length; i++)
        { sum += (ys[i - 1] + ys[i]) * h / 2; }
        return sum;
    }

    // Аппроксимирующая сумма Фурье в точке x
    public double Approximate(double x)
    {
        double res = a[0] / 2;
        for (int n = 1; n <= Degree; n++)
        {
            res += a[n] * Math.Cos(n * x) + b[n] * Math.Sin(n * x);
        }
        return res;
    }
}