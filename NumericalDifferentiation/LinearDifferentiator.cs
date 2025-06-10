namespace NumericalDifferentiation;

public class LinearDifferentiator : IDiscreteDifferentiator
{
    public string Name => "Линейная интерполяция";
    public int RequiredPoints => 2;

    public double Differentiate(IReadOnlyList<(double x, double y)> points, double x, out List<int> usedIndices)
    {
        if (points.Count < 2)
            throw new ArgumentException("Требуется минимум 2 точки");

        // ищем отрезок [x_i, x_{i+1}], в который попадает x
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (x >= points[i].x && x <= points[i + 1].x)
            {
                double dx = points[i + 1].x - points[i].x;
                if (dx == 0)
                    throw new ArgumentException("Нулевой шаг по x!");

                usedIndices = new List<int> { i, i + 1 };
                return (points[i + 1].y - points[i].y) / dx;
            }
        }
        throw new ArgumentException("x вне диапазона точек!");
    }
}