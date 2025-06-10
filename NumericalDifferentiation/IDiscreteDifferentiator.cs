namespace NumericalDifferentiation;

public interface IDiscreteDifferentiator
{
    string Name { get; }
    int RequiredPoints { get; }
    /// <summary>
    /// points: отсортированный список (x, y), x — возрастают.
    /// x: точка, где считаем производную (должна лежать внутри интервала точек)
    /// usedIndices: индексы точек, реально используемых в формуле (для подсветки на графике)
    /// </summary>
    double Differentiate(IReadOnlyList<(double x, double y)> points, double x, out List<int> usedIndices);
}