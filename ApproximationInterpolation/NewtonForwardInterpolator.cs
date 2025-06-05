namespace ApproximationInterpolation;

public class NewtonForwardInterpolator : IInterpolator
{
    public string Name => "Многочлен Ньютона (прямая)";

    private List<(double x, double y)> points;
    private double h; // шаг между узлами
    private double x0;
    private double[,] diffTable; // таблица конечных разностей

    public void SetPoints(List<(double x, double y)> points)
    {
        this.points = points.OrderBy(p => p.x).ToList();
        int n = this.points.Count;

        // Проверяем равномерность узлов
        h = this.points[1].x - this.points[0].x;
        x0 = this.points[0].x;

        for (int i = 1; i < n - 1; i++)
        {
            double hi = this.points[i + 1].x - this.points[i].x;
            if (Math.Abs(hi - h) > 1e-8)
                throw new InvalidOperationException("Узлы должны быть равноотстоящими для многочлена Ньютона!");
        }

        // Таблица конечных разностей: строки — точки, столбцы — порядок разности
        diffTable = new double[n, n];
        for (int i = 0; i < n; i++)
            diffTable[i, 0] = this.points[i].y;

        // Строим все конечные разности (см. слайд 33)
        for (int j = 1; j < n; j++)
        for (int i = 0; i < n - j; i++)
            diffTable[i, j] = diffTable[i + 1, j - 1] - diffTable[i, j - 1];
    }

    public double Interpolate(double x)
    {
        int n = points.Count;
        double q = (x - x0) / h;
        double result = diffTable[0, 0];
        double qProd = 1;

        for (int k = 1; k < n; k++)
        {
            qProd *= (q - (k - 1));
            result += (qProd * diffTable[0, k]) / Factorial(k);
        }
        return result;
    }

    private double Factorial(int n)
    {
        double res = 1;
        for (int i = 2; i <= n; i++)
            res *= i;
        return res;
    }
}