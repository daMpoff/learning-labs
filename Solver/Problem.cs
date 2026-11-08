namespace lpp.Solver;

public enum Relation { LessOrEqual, GreaterOrEqual, Equal }
public enum Goal { Maximize, Minimize }
public enum SolutionStatus { Optimal, Infeasible, Unbounded }

public readonly record struct Point2(double X, double Y);

public readonly record struct Constraint(double A, double B, Relation Relation, double C);

public readonly record struct Objective(double A, double B, Goal Goal)
{
    public double Evaluate(Point2 point) => Math.FusedMultiplyAdd(A, point.X, B * point.Y);
}

public sealed record Solution(
    SolutionStatus Status, IReadOnlyList<Point2> Vertices, Point2? Optimum = null,
    double? Value = null, bool HasAlternatives = false);
