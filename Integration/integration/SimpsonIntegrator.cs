namespace integration;

public class SimpsonIntegrator : IIntegrator
{
    public string Name => "Параболы (Симпсона)";

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        int N = (int)Math.Round((b - a) / h);

        // Симпсон требует четное число шагов!
        if (N % 2 != 0) { N++; }

        h = (b - a) / N;

        double sum = f(a) + f(b);
        for (int i = 1; i < N; i++)
        {
            double x = a + i * h;
            sum += (i % 2 == 0 ? 2 : 4) * f(x);
        }

        return sum * h / 3.0;
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        int N = (int)Math.Ceiling((b - a) / h);
        if (N % 2 == 1) { N++; }
        double realH = (b - a) / N;

        for (int i = 0; i < N; i += 2)
        {
            double x0 = a + i * realH;
            double x1 = x0 + realH;
            double x2 = x0 + 2 * realH;
            if (x2 > b) { x2 = b; }

            double y0 = f(x0);
            double y1 = f(x1);
            double y2 = f(x2);

            // Генерируем гладкую параболу между x0 и x2 через x1
            int points = 20;
            double[] xs = new double[points + 1];
            double[] ys = new double[points + 1];
            for (int k = 0; k <= points; k++)
            {
                double t = (double)k / points;
                double x = x0 + t * (x2 - x0);
                // Интерполяция Лагранжа для параболы через три точки
                ys[k] = y0 * (x - x1) * (x - x2) / ((x0 - x1) * (x0 - x2))
                        + y1 * (x - x0) * (x - x2) / ((x1 - x0) * (x1 - x2))
                        + y2 * (x - x0) * (x - x1) / ((x2 - x0) * (x2 - x1));
                xs[k] = x;
            }

            // Строим многоугольник для заливки
            double[] fillX = new double[points + 3];
            double[] fillY = new double[points + 3];
            fillX[0] = x0;
            fillY[0] = 0;
            Array.Copy(xs, 0, fillX, 1, points + 1);
            Array.Copy(ys, 0, fillY, 1, points + 1);
            fillX[points + 2] = x2;
            fillY[points + 2] = 0;

            plot.Add.Polygon(fillX, fillY);

            // Нарисовать саму параболу (линией)
            plot.Add.Scatter(xs, ys);
        }
    }
}
