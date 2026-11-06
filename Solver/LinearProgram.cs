namespace lpp.Solver;

public static class LinearProgram
{
    private const double Tolerance = 2e-12;

    // The two variables are nonnegative. Each boundary contributes at most two vertices.
    public static Solution Solve(Objective objective, IReadOnlyList<Constraint> constraints)
    {
        CheckFinite(objective.A, objective.B, 0);
        var planes = new List<HalfPlane>(constraints.Count + 2)
        {
            new(-1, 0, 0),
            new(0, -1, 0)
        };

        foreach (var constraint in constraints)
        {
            CheckFinite(constraint.A, constraint.B, constraint.C);
            var scale = Math.Max(Math.Abs(constraint.A), Math.Abs(constraint.B));
            var plane = scale == 0
                ? new HalfPlane(0, 0, constraint.C)
                : new HalfPlane(constraint.A / scale, constraint.B / scale, constraint.C / scale);
            if (!double.IsFinite(plane.C))
                throw new ArgumentException("The constraint is outside the numerical range.");

            if (constraint.Relation != Relation.GreaterOrEqual)
                planes.Add(plane);
            if (constraint.Relation != Relation.LessOrEqual)
                planes.Add(new HalfPlane(-plane.A, -plane.B, -plane.C));
        }

        if (planes.Any(p => p.A == 0 && p.B == 0 && p.C < 0))
            return new Solution(SolutionStatus.Infeasible, []);

        var vertices = FindVertices(planes);
        if (vertices.Count == 0)
            return new Solution(SolutionStatus.Infeasible, vertices);

        var scaleObjective = Math.Max(Math.Abs(objective.A), Math.Abs(objective.B));
        var direction = objective.Goal == Goal.Maximize ? 1 : -1;
        var a = scaleObjective == 0 ? 0 : direction * objective.A / scaleObjective;
        var b = scaleObjective == 0 ? 0 : direction * objective.B / scaleObjective;

        // Scale a direction so its larger component is 1, retaining small slopes near both axes.
        var rayScore = Math.Max(BestRayScore(freeX: true), BestRayScore(freeX: false));
        if (rayScore > 0)
            return new Solution(SolutionStatus.Unbounded, vertices);

        var best = vertices[0];
        foreach (var vertex in vertices)
        {
            if (CompareScore(a, b, vertex, best) > 0)
                best = vertex;
        }

        var alternatives = rayScore == 0 || vertices.Any(p => p != best && CompareScore(a, b, p, best) == 0);
        var value = objective.Evaluate(best);
        if (!double.IsFinite(value))
            throw new ArgumentException("The objective value is outside the numerical range.");

        return new Solution(SolutionStatus.Optimal, vertices, best, value, alternatives);

        int BestRayScore(bool freeX)
        {
            var range = new Interval(0, 1);
            foreach (var plane in planes)
            {
                if (!range.Restrict(freeX ? plane.A : plane.B, freeX ? -plane.B : -plane.A))
                    return -1;
            }

            var low = freeX ? new Point2(range.Low, 1) : new Point2(1, range.Low);
            var high = freeX ? new Point2(range.High, 1) : new Point2(1, range.High);
            return Math.Max(CompareScore(a, b, low, default), CompareScore(a, b, high, default));
        }
    }

    private static List<Point2> FindVertices(List<HalfPlane> planes)
    {
        var candidates = new HashSet<Point2>();
        foreach (var boundary in planes)
        {
            if (boundary.A == 0 && boundary.B == 0)
                continue;

            // Use the larger coefficient to express one coordinate in terms of the other.
            var freeX = Math.Abs(boundary.B) > Math.Abs(boundary.A);
            var pivot = freeX ? boundary.B : boundary.A;
            var slope = -(freeX ? boundary.A : boundary.B) / pivot;
            var offset = boundary.C / pivot;
            var range = new Interval(0, double.PositiveInfinity);
            var feasible = true;

            foreach (var plane in planes)
            {
                var fixedCoefficient = freeX ? plane.B : plane.A;
                var freeCoefficient = freeX ? plane.A : plane.B;
                var factor = SumProducts(fixedCoefficient, slope, freeCoefficient);
                var bound = SumProducts(-fixedCoefficient, offset, plane.C);
                if (!range.Restrict(factor, bound))
                {
                    feasible = false;
                    break;
                }
            }

            if (!feasible)
                continue;

            AddEndpoint(range.Low);
            if (double.IsFinite(range.High))
                AddEndpoint(range.High);

            void AddEndpoint(double parameter)
            {
                var fixedCoordinate = Math.Max(0, SumProducts(slope, parameter, offset));
                var point = freeX ? new Point2(parameter, fixedCoordinate) : new Point2(fixedCoordinate, parameter);
                if (!double.IsFinite(point.X) || !double.IsFinite(point.Y))
                    throw new ArgumentException("A vertex is outside the numerical range.");
                candidates.Add(point);
            }
        }

        var ordered = candidates.OrderBy(p => p.X).ThenBy(p => p.Y);
        var vertices = new List<Point2>(candidates.Count);
        foreach (var candidate in ordered)
        {
            var duplicate = false;
            for (var i = vertices.Count - 1; i >= 0 && NearlyEqual(vertices[i].X, candidate.X); i--)
            {
                if (NearlyEqual(vertices[i].Y, candidate.Y))
                {
                    duplicate = true;
                    break;
                }
            }

            if (!duplicate)
                vertices.Add(candidate);
        }

        return vertices;
    }

    private static bool NearlyEqual(double a, double b) =>
        a == b || (double.IsFinite(a) && double.IsFinite(b) &&
                   Math.Abs(a - b) <= Tolerance * Math.Max(Math.Abs(a), Math.Abs(b)));

    private static double SumProducts(double a, double b, double c)
    {
        var product = a * b;
        var value = Math.FusedMultiplyAdd(a, b, c);
        // FMA can expose the rounding error of a product used earlier to form c.
        return product == -c ? 0 : value;
    }

    private static int CompareScore(double a, double b, Point2 p, Point2 q)
    {
        var x = p.X - q.X;
        var y = p.Y - q.Y;
        var value = SumProducts(a, x, b * y);
        var error = Tolerance * Math.Max(Math.Abs(a * x), Math.Abs(b * y));
        return Math.Abs(value) <= error ? 0 : Math.Sign(value);
    }

    private static void CheckFinite(double a, double b, double c)
    {
        if (!double.IsFinite(a) || !double.IsFinite(b) || !double.IsFinite(c))
            throw new ArgumentException("Coefficients must be finite.");
    }

    private readonly record struct HalfPlane(double A, double B, double C);

    // Intersect with factor * t <= bound. Relative tolerance only reconciles nearby endpoints.
    private struct Interval(double low, double high)
    {
        public double Low { get; private set; } = low;
        public double High { get; private set; } = high;

        public bool Restrict(double factor, double bound)
        {
            if (factor == 0)
                return bound >= 0;

            var endpoint = bound / factor;
            if (factor > 0)
                High = Math.Min(High, endpoint);
            else
                Low = Math.Max(Low, endpoint);

            if (Low <= High)
                return true;
            if (!double.IsFinite(High) || !NearlyEqual(Low, High))
                return false;

            High = Low;
            return true;
        }
    }
}
