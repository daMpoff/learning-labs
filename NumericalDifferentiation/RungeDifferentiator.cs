namespace NumericalDifferentiation;

public class RungeDifferentiator : IDiscreteDifferentiator
{
    public string Name => "Рунге (усиление точности, p=1, k=2)";
    public int RequiredPoints => 3; // для минимального h и 2h

    public double Differentiate(IReadOnlyList<(double x, double y)> points, double x, out List<int> usedIndices)
    {
        usedIndices = new List<int>();
        if (points.Count < 3)
            throw new ArgumentException("Для метода Рунге нужно минимум 3 точки");

        // Отсортировать по x
        var sorted = points.OrderBy(p => p.x).ToList();

        // Найти индекс точки, ближайшей к x и расположенной правее x
        int idx = sorted.FindIndex(p => p.x >= x);
        if (idx < 2) idx = 2;
        if (idx > sorted.Count - 1) idx = sorted.Count - 1;

        // Подбираем шаг h (равномерный шаг!)
        double h = sorted[idx].x - sorted[idx - 1].x;
        double h2 = sorted[idx].x - sorted[idx - 2].x;
        if (Math.Abs(h2 - 2 * h) > 1e-8)
            throw new ArgumentException("Точки должны быть равномерно распределены для метода Рунге по точкам.");

        // Левый дифференциатор с h
        double f_x = sorted[idx].y;
        double f_xh = sorted[idx - 1].y;
        double f_x2h = sorted[idx - 2].y;

        // f'(x) ~ (f(x) - f(x-h))/h
        double left_h = (f_x - f_xh) / h;
        // f'(x) ~ (f(x) - f(x-2h))/(2h)
        double left_2h = (f_x - f_x2h) / (2 * h);

        // Рунге (p=1, k=2)
        double runge = left_h + (left_h - left_2h) / (Math.Pow(2, 1) - 1);

        usedIndices.Add(idx);
        usedIndices.Add(idx - 1);
        usedIndices.Add(idx - 2);
        return runge;
    }
}