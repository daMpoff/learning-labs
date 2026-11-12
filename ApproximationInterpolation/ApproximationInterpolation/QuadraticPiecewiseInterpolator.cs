namespace ApproximationInterpolation;

public class QuadraticPiecewiseInterpolator : IInterpolator
{
    public string Name => "Кусочная квадратичная";
    private List<(double x, double y)> points;

    // Для каждого подотрезка храним коэффициенты квадратичной параболы (a, b, c) и интервал (x0, x2)
    private List<(double x0, double x1, double x2, double a, double b, double c)> segments;

    public void SetPoints(List<(double x, double y)> points)
    {
        this.points = points.OrderBy(p => p.x).ToList();
        segments = new List<(double, double, double, double, double, double)>();

        if (points.Count < 3)
            throw new InvalidOperationException("Для квадратичной интерполяции нужно минимум 3 точки");

        // Строим параболу для каждой тройки подряд идущих точек
        for (int i = 0; i < this.points.Count - 2; i++)
        {
            var (x0, y0) = this.points[i];
            var (x1, y1) = this.points[i + 1];
            var (x2, y2) = this.points[i + 2];

            // Решаем СЛАУ для коэффициентов
            // | x0^2  x0  1 |   | a |   | y0 |
            // | x1^2  x1  1 | * | b | = | y1 |
            // | x2^2  x2  1 |   | c |   | y2 |

            // Выражаем a, b, c через определители (Крамер)
            double D = Det3(
                x0 * x0, x0, 1,
                x1 * x1, x1, 1,
                x2 * x2, x2, 1
            );
            double Da = Det3(
                y0, x0, 1,
                y1, x1, 1,
                y2, x2, 1
            );
            double Db = Det3(
                x0 * x0, y0, 1,
                x1 * x1, y1, 1,
                x2 * x2, y2, 1
            );
            double Dc = Det3(
                x0 * x0, x0, y0,
                x1 * x1, x1, y1,
                x2 * x2, x2, y2
            );

            double a = Da / D;
            double b = Db / D;
            double c = Dc / D;

            segments.Add((x0, x1, x2, a, b, c));
        }
    }

    public double Interpolate(double x)
    {
        if (segments == null || segments.Count == 0)
            throw new InvalidOperationException("Сначала вызовите SetPoints");

        // Ищем, в каком сегменте x лежит (по интервалу [x0, x2])
        foreach (var (x0, x1, x2, a, b, c) in segments)
        {
            if (x >= x0 && x <= x2)
                return a * x * x + b * x + c;
        }

        // Если вне диапазона — экстраполируем по ближайшей параболе
        if (x < segments[0].x0)
            return segments[0].a * x * x + segments[0].b * x + segments[0].c;
        else
        {
            var s = segments.Last();
            return s.a * x * x + s.b * x + s.c;
        }
    }

    // Вычисление определителя 3x3
    private double Det3(
        double a11, double a12, double a13,
        double a21, double a22, double a23,
        double a31, double a32, double a33)
    {
        return
            a11 * a22 * a33 + a12 * a23 * a31 + a13 * a21 * a32
            - a13 * a22 * a31 - a12 * a21 * a33 - a11 * a23 * a32;
    }
}