namespace integration;

public class GaussIntegrator : IIntegrator
{
    public string Name => $"Гаусс-Лежандр (n={Order})";
    public int Order { get; }

    public GaussIntegrator(int order = 5)
    {
        if (order < 2 || order > 10)
            throw new ArgumentException("Гауссов порядок должен быть в диапазоне 2-10");
        Order = order;
    }

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        int n = Order;
        double[] t = new double[n]; // корни (узлы)
        double[] A = new double[n]; // веса

        int m = (n + 1) / 2; // Только половина, остальные зеркалим
        for (int i = 0; i < m; i++)
        {
            double ti = Math.Cos(Math.PI * (4 * i + 3) / (4 * n + 2));
            double tiPrev;
            int iter = 0, maxIter = 100;
            do
            {
                tiPrev = ti;
                double pn = LegendreP(n, ti);
                double pn1 = LegendreP_Deriv(n, ti);
                ti = tiPrev - pn / pn1;
                iter++;
                if (iter > maxIter)
                    throw new Exception("Ошибка вычисления корней: метод Ньютона не сошёлся.");
            } while (Math.Abs(ti - tiPrev) > 1e-14);

            t[i] = ti;
            t[n - 1 - i] = -ti; // зеркально
            double pn1_final = LegendreP_Deriv(n, ti);
            double w = 2 / ((1 - ti * ti) * pn1_final * pn1_final);
            A[i] = w;
            A[n - 1 - i] = w;
        }

        // Интегрируем на [a, b]
        double xm = (a + b) / 2.0, xr = (b - a) / 2.0;
        double result = 0;
        for (int i = 0; i < n; i++)
        {
            double xi = xm + xr * t[i];
            result += A[i] * f(xi);
        }

        return xr * result;
    }

    // Рекуррентное вычисление полинома Лежандра P_n(t)
    private static double LegendreP(int n, double t)
    {
        if (n == 0) return 1;
        if (n == 1) return t;
        double p0 = 1, p1 = t;
        for (int k = 2; k <= n; k++)
        {
            double pk = ((2 * k - 1) * t * p1 - (k - 1) * p0) / k;
            p0 = p1;
            p1 = pk;
        }

        return p1;
    }

    // Производная полинома Лежандра P'_n(t)
    private static double LegendreP_Deriv(int n, double t)
    {
        return n / (1 - t * t) * (LegendreP(n - 1, t) - t * LegendreP(n, t));
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        // График функции + отметим узлы
        int points = 500;
        double[] xs = Enumerable.Range(0, points).Select(i => a + (b - a) * i / (points - 1)).ToArray();
        double[] ys = xs.Select(f).ToArray();
        plot.Add.Scatter(xs, ys);

        // Покажем узлы Гаусса точками (вычисляем по тому же алгоритму)
        int n = Order;
        double[] t = new double[n];
        int m = (n + 1) / 2;
        for (int i = 0; i < m; i++)
        {
            double ti = Math.Cos(Math.PI * (4 * i + 3) / (4 * n + 2));
            double tiPrev;
            int iter = 0, maxIter = 100;
            do
            {
                tiPrev = ti;
                double pn = LegendreP(n, ti);
                double pn1 = LegendreP_Deriv(n, ti);
                ti = tiPrev - pn / pn1;
                iter++;
                if (iter > maxIter)
                    throw new Exception("Ошибка вычисления корней: метод Ньютона не сошёлся.");
            } while (Math.Abs(ti - tiPrev) > 1e-14);

            t[i] = ti;
            t[n - 1 - i] = -ti;
        }

        double xm = (a + b) / 2.0, xr = (b - a) / 2.0;
        double[] xNodes = t.Select(ti => xm + xr * ti).ToArray();
        double[] yNodes = xNodes.Select(f).ToArray();
        plot.Add.ScatterPoints(xNodes, yNodes, color: ScottPlot.Colors.OrangeRed);
    }
}