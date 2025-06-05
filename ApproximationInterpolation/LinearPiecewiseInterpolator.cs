namespace ApproximationInterpolation;

public class LinearPiecewiseInterpolator : IInterpolator
{
    public string Name => "Кусочная линейная";
    private List<(double x, double y)> points;

    public void SetPoints(List<(double x, double y)> points)
    {
        this.points = points.OrderBy(p => p.x).ToList();
    }

    public double Interpolate(double x)
    {
        if (points == null || points.Count < 2)
            throw new InvalidOperationException("Не заданы точки для интерполяции");

        // Граничные значения
        if (x <= points.First().x) return points.First().y;
        if (x >= points.Last().x) return points.Last().y;

        // Находим интервал [x0, x1] такой, что x0 <= x <= x1
        for (int i = 0; i < points.Count - 1; i++)
        {
            var (x0, y0) = points[i];
            var (x1, y1) = points[i + 1];
            if (x0 <= x && x <= x1)
            {
                // Формула из презентации:
                // f(x) = f(x0) + (f(x1) - f(x0)) / (x1 - x0) * (x - x0)
                return y0 + (y1 - y0) / (x1 - x0) * (x - x0);
            }
        }
        throw new ArgumentException("x вне диапазона точек");
    }
}
