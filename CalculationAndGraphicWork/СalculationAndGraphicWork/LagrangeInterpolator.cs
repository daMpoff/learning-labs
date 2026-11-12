namespace СalculationAndGraphicWork;

public class LagrangeInterpolator : IInterpolator
{
    public string Name => "Многочлен Лагранжа";

    private List<(double x, double y)> points;

    public void SetPoints(List<(double x, double y)> points)
    {
        this.points = points.ToList();
    }

    public double Interpolate(double x)
    {
        double result = 0;
        int n = points.Count;

        for (int i = 0; i < n; i++)
        {
            double xi = points[i].x;
            double yi = points[i].y;

            // Считаем базисный полином li(x)
            double li = 1;
            for (int j = 0; j < n; j++)
            {
                if (j == i) continue;
                double xj = points[j].x;
                li *= (x - xj) / (xi - xj);
            }

            result += yi * li;
        }

        return result;
    }
}