using MathNet.Numerics.LinearAlgebra;

namespace NumericalDifferentiation;

public class UndeterminedCoefficientsDifferentiator : IDiscreteDifferentiator
{
    public string Name => "МНК (неопределённые коэффициенты)";
    // Рекомендуется минимум 3–5 точек для осмысленного результата
    public int RequiredPoints => 3;

    public double Differentiate(IReadOnlyList<(double x, double y)> points, double x, out List<int> usedIndices)
    {
        // Для лучшей устойчивости — выбираем ближайшие n точек к x (n = points.Count)
        int n = points.Count;
        if (n < RequiredPoints)
            throw new ArgumentException($"Минимум {RequiredPoints} точки");

        // Индексы всех точек (выделяем все, или можно брать n ближайших)
        usedIndices = Enumerable.Range(0, n).ToList();

        // Сдвигаем все x относительно x0 (центра), для численной устойчивости
        double x0 = x;

        // Собираем матрицу A и правую часть b
        var A = Matrix<double>.Build.Dense(n, n);
        var b = Vector<double>.Build.Dense(n);

        // Формируем систему моментов: Σ c_j * (x_j - x0)^k = δ_{k,1} (k = 0...n-1)
        for (int row = 0; row < n; row++)
        {
            for (int col = 0; col < n; col++)
            {
                A[row, col] = Math.Pow(points[col].x - x0, row);
            }
            // Правая часть: b[0]=0 (нулевая производная), b[1]=1 (первая), остальные=0
            b[row] = (row == 1) ? 1.0 : 0.0;
        }

        // Решаем систему
        var c = A.Solve(b);

        // Производная: c[0]*y_0 + ... + c[n-1]*y_{n-1}
        double result = 0;
        for (int i = 0; i < n; i++)
            result += c[i] * points[i].y;

        return result;
    }
}