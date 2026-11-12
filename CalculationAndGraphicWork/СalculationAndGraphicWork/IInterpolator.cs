namespace СalculationAndGraphicWork;

public interface IInterpolator
{
    string Name { get; }
    double Interpolate(double x);
    void SetPoints(List<(double x, double y)> points);
}