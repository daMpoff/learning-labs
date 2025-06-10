namespace NumericalDifferentiation;

public static class FiniteDifferenceRegistry
{
    // Левая разность: (y1 - y0) / h
    public static double Left(double y0, double y1, double h) =>
        (y1 - y0) / h;

    // Правая разность: (y2 - y1) / h
    public static double Right(double y1, double y2, double h) =>
        (y2 - y1) / h;

    // Центральная разность: (y2 - y0) / (2h)
    public static double Central(double y0, double y2, double h) =>
        (y2 - y0) / (2 * h);
    
    // Прямые разности (Direct forward difference)
    private static double DirectDifference(IList<double> y, int k, int order)
    {
        if (order == 0) return y[k];
        return DirectDifference(y, k + 1, order - 1) - DirectDifference(y, k, order - 1);
    }

    // Производная n-го порядка (для функции, равномерный шаг)
    public static double HigherOrderDerivative(Func<double, double> f, double x, double h, int order)
    {
        var values = new List<double>();
        for (int i = 0; i <= order; i++)
        {
            values.Add(f(x + i * h));
        }
        return DirectDifference(values, 0, order) / Math.Pow(h, order);
    }

    // Производная n-го порядка (по массиву y)
    public static double HigherOrderDerivativeY(IList<double> y, int k, double h, int order) => DirectDifference(y, k, order) / Math.Pow(h, order);
}
