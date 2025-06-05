namespace ApproximationInterpolation;

public interface IInterpolator
{
    string Name { get; } // Имя для отображения в ComboBox
    double Interpolate(double x); // Интерполяция в точке x
    void SetPoints(List<(double x, double y)> points); // Задать точки
}
