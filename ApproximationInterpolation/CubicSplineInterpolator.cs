namespace ApproximationInterpolation;

public class CubicSplineInterpolator : IInterpolator
{
    public string Name => "Кубический сплайн";

    private List<(double x, double y)> points;
    private int n; // число отрезков (точек - 1)
    private double[] a, b, c, d, x;

    public void SetPoints(List<(double x, double y)> points)
    {
        this.points = points.OrderBy(p => p.x).ToList();
        n = this.points.Count - 1;
        x = this.points.Select(p => p.x).ToArray();

        a = new double[n + 1];
        b = new double[n];
        c = new double[n + 1];
        d = new double[n];

        for (int i = 0; i <= n; i++)
        {
            a[i] = this.points[i].y;
        }

        // Шаги между x
        double[] h = new double[n];
        for (int i = 0; i < n; i++)
        {
            h[i] = x[i + 1] - x[i];
        }

        // Система уравнений на c (трёхдиагональная матрица)
        double[] alpha = new double[n];
        double[] beta = new double[n];

        // 1. Заполняем прогоночные коэффициенты
        alpha[0] = 0; // для натурального сплайна
        beta[0] = 0;

        // 2. Формируем правые части
        double[] A = new double[n];
        double[] B = new double[n];
        double[] C = new double[n];
        double[] F = new double[n];

        for (int i = 1; i < n; i++)
        {
            A[i] = h[i - 1];
            B[i] = 2 * (h[i - 1] + h[i]);
            C[i] = h[i];
            F[i] = 3 * ((a[i + 1] - a[i]) / h[i] - (a[i] - a[i - 1]) / h[i - 1]);
        }

        // 3. Прямой ход прогонки
        for (int i = 1; i < n; i++)
        {
            double denominator = B[i] - A[i] * alpha[i - 1];
            alpha[i] = C[i] / denominator;
            beta[i] = (F[i] - A[i] * beta[i - 1]) / denominator;
        }

        // 4. Обратный ход
        c[n] = 0; // естественный сплайн, крайнее условие
        for (int i = n - 1; i >= 1; i--)
        {
            c[i] = beta[i] - alpha[i] * c[i + 1];
            c[0] = 0;
        } // естественный сплайн, крайнее условие


        // 5. Остальные коэффициенты
        for (int i = 0; i < n; i++)
        {
            d[i] = (c[i + 1] - c[i]) / (3 * h[i]);
            b[i] = (a[i + 1] - a[i]) / h[i] - h[i] * (c[i + 1] + 2 * c[i]) / 3;
        }
    }

    public double Interpolate(double xx)
    {
        // Находим нужный сегмент
        int i = FindSegment(xx);

        double dx = xx - x[i];
        return a[i] + b[i] * dx + c[i] * dx * dx + d[i] * dx * dx * dx;
    }

    // Поиск сегмента, которому принадлежит x
    private int FindSegment(double xx)
    {
        // Если левее первого — возвращаем первый сегмент
        if (xx <= x[0])
            return 0;
        // Если правее последнего — последний сегмент
        if (xx >= x[n])
            return n - 1;

        // Бинарный поиск (быстро для большого числа точек)
        int left = 0, right = n;
        while (left < right - 1)
        {
            int mid = (left + right) / 2;
            if (xx < x[mid])
                right = mid;
            else
                left = mid;
        }

        return left;
    }
}