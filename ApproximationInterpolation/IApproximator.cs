namespace ApproximationInterpolation;

public interface IApproximator
{
    string Name { get; }
    void SetPoints(List<(double x, double y)> points);
    double Approximate(double x);
}