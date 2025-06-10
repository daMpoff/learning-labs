namespace NumericalDifferentiation;

public class QuadraticDifferentiator : IDiscreteDifferentiator
{
    public string Name => "Квадратичная интерполяция";
    public int RequiredPoints => 3;

    public double Differentiate(IReadOnlyList<(double x, double y)> points, double x, out List<int> usedIndices)
    {
        if (points.Count < 3)
            throw new ArgumentException("Требуется минимум 3 точки");

        // Найдём тройку, в которую попадает x
        int idx = -1;
        for (int i = 0; i < points.Count - 2; i++)
        {
            if (x >= points[i].x && x <= points[i + 2].x)
            {
                idx = i;
                break;
            }
        }
        if (idx == -1)
            throw new ArgumentException("x вне диапазона точек!");

        var p0 = points[idx];
        var p1 = points[idx + 1];
        var p2 = points[idx + 2];

        // Проверим равномерность шага
        double h1 = p1.x - p0.x;
        double h2 = p2.x - p1.x;
        if (Math.Abs(h1 - h2) > 1e-8)
            throw new ArgumentException("Шаг по x должен быть постоянным для квадратичной интерполяции!");

        double h = h1;
        double q = (x - p0.x) / h;
        double delta_y0 = p1.y - p0.y;
        double delta2_y0 = p2.y - 2 * p1.y + p0.y;

        usedIndices = new List<int> { idx, idx + 1, idx + 2 };
        return (1 / h) * delta_y0 + ((2 * q - 1) / 2) * delta2_y0 / (h);
    }
}