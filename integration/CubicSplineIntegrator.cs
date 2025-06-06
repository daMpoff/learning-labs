namespace integration;

using MathNet.Symbolics;

public class CubicSplineIntegrator : IIntegrator
{
    public string Name => "Сплайн";

    public double Integrate(Func<double, double> f, double a, double b, double h, string formula)
    {
        // 1. Парсим основную формулу как SymbolicExpression
        var expr = SymbolicExpression.Parse(formula);

        // 2. Вычисляем вторую производную
        var secondDerivative = expr.Differentiate("x").Differentiate("x").Expand();

        // 3. Лямбда для второй производной
        Func<double, double> f2 = x =>
        {
            var val = secondDerivative.Evaluate(new Dictionary<string, FloatingPoint> { { "x", x } });
            return val.RealValue;
        };

        // 4. Считаем значения функции в узлах
        int N = (int)Math.Ceiling((b - a) / h);
        double[] xs = new double[N + 1];
        double[] ys = new double[N + 1];
        for (int i = 0; i <= N; i++)
        {
            xs[i] = a + i * h;
            if (xs[i] > b) xs[i] = b;
            ys[i] = f(xs[i]);
        }

        // 5. Метод трапеций + поправка по второй производной
        double trap = 0.0, correction = 0.0;
        for (int i = 1; i <= N; i++)
        {
            double hi = xs[i] - xs[i - 1];
            trap += hi * (ys[i - 1] + ys[i]) / 2.0;
            correction += Math.Pow(hi, 3) * (f2(xs[i - 1]) + f2(xs[i])) / 12.0;
        }

        return trap - correction;
    }

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        throw new NotImplementedException("Используйте перегрузку с формулой (строкой).");
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        // Для наглядности — как у трапеций
        int N = (int)Math.Ceiling((b - a) / h);
        double[] xs = new double[N + 1];
        double[] ys = new double[N + 1];
        for (int i = 0; i <= N; i++)
        {
            xs[i] = a + i * h;
            if (xs[i] > b) xs[i] = b;
            ys[i] = f(xs[i]);
        }

        for (int i = 1; i <= N; i++)
        {
            double[] xSeg = { xs[i - 1], xs[i - 1], xs[i], xs[i] };
            double[] ySeg = { 0, ys[i - 1], ys[i], 0 };
            plot.Add.Polygon(xSeg, ySeg);
        }
    }
}
