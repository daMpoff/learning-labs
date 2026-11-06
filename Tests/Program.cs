using System.Diagnostics;
using lpp.Solver;
using static lpp.Solver.Goal;
using static lpp.Solver.Relation;
using static lpp.Solver.SolutionStatus;

if (args.Contains("--benchmark"))
{
    RunBenchmark();
    return;
}

var cases = 0;
Check("README example", new(3, 4, Maximize),
    [new(4, 9, LessOrEqual, 36), new(2, 1, LessOrEqual, 11), new(0, 1, LessOrEqual, 5)],
    Optimal, 21.5, new(4.5, 2), vertices: 4);
Check("axis-aligned and parallel", new(2, 3, Maximize),
    [new(1, 0, LessOrEqual, 5), new(3, 0, LessOrEqual, 12), new(0, 1, LessOrEqual, 2)],
    Optimal, 14, new(4, 2), vertices: 4);
Check("boundary through origin", new(0, 1, Maximize),
    [new(1, -1, Equal, 0), new(1, 0, LessOrEqual, 3)], Optimal, 3, new(3, 3), vertices: 2);
Check("inconsistent bounds", new(1, 1, Maximize),
    [new(1, 0, LessOrEqual, 2), new(1, 0, GreaterOrEqual, 3)], Infeasible, vertices: 0);
Check("inconsistent constant", new(1, 1, Maximize), [new(0, 0, Equal, 1)], Infeasible);
Check("unbounded objective", new(1, 1, Maximize), [new(0, 0, Equal, 0)], Unbounded, vertices: 1);
Check("unbounded minimum", new(0, -3, Minimize), [new(1, 0, Equal, 2)], Unbounded, vertices: 1);
Check("unbounded region, finite minimum", new(1, 1, Minimize),
    [new(1, 2, GreaterOrEqual, 4)], Optimal, 2, new(0, 2), vertices: 2);
Check("optimal ray", new(1, 0, Minimize),
    [new(1, 0, GreaterOrEqual, 2)], Optimal, 2, new(2, 0), alternatives: true);
Check("optimal edge", new(1, 1, Maximize), [new(1, 1, LessOrEqual, 4)],
    Optimal, 4, alternatives: true, vertices: 3);
Check("constant objective", new(0, 0, Minimize), [], Optimal, 0, new(0, 0), alternatives: true);
Check("unique feasible point", new(0, 0, Maximize),
    [new(1, 0, Equal, 2), new(0, 1, Equal, 3)], Optimal, 0, new(2, 3), vertices: 1);
Check("scaled decimal equality", new(1, 1, Maximize),
    [new(0.1, 0.3, Equal, 0.4), new(1, 3, Equal, 4)], Optimal, 4, new(4, 0), vertices: 2);
Check("near-parallel boundaries", new(0, 1, Maximize),
    [new(1, 1, Equal, 1), new(1, 1 + 1e-10, Equal, 1 + 5e-11)],
    Optimal, 0.5, new(0.5, 0.5), vertices: 1);
Check("small nonzero objective", new(1e-20, 0, Maximize), [], Unbounded);
Check("narrow recession ray", new(0, 1, Maximize),
    [new(-1e-20, 1, Equal, 0)], Unbounded);
Check("tiny positive bound", new(1, 0, Maximize),
    [new(1, 0, LessOrEqual, 1e-15), new(0, 1, Equal, 0)],
    Optimal, 1e-15, new(1e-15, 0), vertices: 2);

var random = new Random(257);
for (var i = 0; i < 2_000; i++)
{
    var rows = Enumerable.Range(0, random.Next(0, 9))
        .Select(_ => new Constraint(random.Next(-5, 6), random.Next(-5, 6),
            (Relation)random.Next(3), random.Next(-8, 13))).ToArray();
    var objective = new Objective(random.Next(-5, 6), random.Next(-5, 6), (Goal)random.Next(2));
    var (expected, value) = IntegerReference(objective, rows);
    try
    {
        Check($"integer reference {i}", objective, rows, expected, value);
    }
    catch
    {
        Console.Error.WriteLine(objective);
        Console.Error.WriteLine(string.Join(Environment.NewLine, rows));
        throw;
    }
}
Console.WriteLine($"Passed {cases} cases.");

void Check(string name, Objective objective, Constraint[] rows, SolutionStatus status, double? value = null,
    Point2? optimum = null, bool? alternatives = null, int? vertices = null)
{
    var result = LinearProgram.Solve(objective, rows);
    var mismatch = result.Status != status ||
        (value.HasValue && (result.Value is not { } found || !Close(value.Value, found))) ||
        (optimum is { } target && (result.Optimum is not { } actual ||
                                  !Close(target.X, actual.X) || !Close(target.Y, actual.Y))) ||
        (alternatives.HasValue && result.HasAlternatives != alternatives) ||
        (vertices.HasValue && result.Vertices.Count != vertices);
    if (mismatch)
        throw new InvalidOperationException($"{name}: expected {status}, {value}, {optimum}, {vertices} vertices. " +
            $"Got {result.Status}, {result.Value}, {result.Optimum}, {result.Vertices.Count} vertices.");

    foreach (var point in result.Vertices)
    {
        if (point.X < 0 || point.Y < 0 || rows.Any(row =>
            {
                var first = row.A * point.X;
                var second = row.B * point.Y;
                var lhs = first + second;
                var tolerance = 2e-10 * Math.Max(Math.Abs(row.C), Math.Max(Math.Abs(first), Math.Abs(second)));
                return row.Relation switch
                {
                    LessOrEqual => lhs - row.C > tolerance,
                    GreaterOrEqual => row.C - lhs > tolerance,
                    _ => Math.Abs(lhs - row.C) > tolerance
                };
            }))
            throw new InvalidOperationException($"{name}: infeasible vertex {point}.");
    }
    cases++;
}

static bool Close(double expected, double actual) =>
    expected == actual || Math.Abs(expected - actual) <= 2e-8 * Math.Max(Math.Abs(expected), Math.Abs(actual));

static (SolutionStatus Status, double? Value) IntegerReference(Objective objective, Constraint[] constraints)
{
    Constraint[] rows = [..constraints, new(1, 0, GreaterOrEqual, 0), new(0, 1, GreaterOrEqual, 0)];
    var direction = objective.Goal == Maximize ? 1 : -1;
    var a = (long)objective.A * direction;
    var b = (long)objective.B * direction;
    long bestNumerator = 0, bestDenominator = 0;
    for (var i = 0; i < rows.Length; i++)
    {
        var first = rows[i];
        for (var j = i + 1; j < rows.Length; j++)
        {
            var second = rows[j];
            var d = (long)(first.A * second.B - first.B * second.A);
            if (d == 0)
                continue;
            var x = (long)(first.C * second.B - first.B * second.C) * Math.Sign(d);
            var y = (long)(first.A * second.C - first.C * second.A) * Math.Sign(d);
            d = Math.Abs(d);
            if (!rows.All(row => Holds(row, x, y, d)))
                continue;
            var score = a * x + b * y;
            if (bestDenominator == 0 || score * bestDenominator > bestNumerator * d)
                (bestNumerator, bestDenominator) = (score, d);
        }
    }
    if (bestDenominator == 0)
        return (Infeasible, null);

    foreach (var row in rows)
    {
        var x = (long)row.B;
        var y = -(long)row.A;
        if ((a * x + b * y > 0 && rows.All(r => Holds(r, x, y, 0))) ||
            (a * x + b * y < 0 && rows.All(r => Holds(r, -x, -y, 0))))
            return (Unbounded, null);
    }
    return (Optimal, direction * (double)bestNumerator / bestDenominator);

    static bool Holds(Constraint row, long x, long y, long denominator)
    {
        var delta = (long)row.A * x + (long)row.B * y - (long)row.C * denominator;
        return row.Relation switch
        {
            LessOrEqual => delta <= 0,
            GreaterOrEqual => delta >= 0,
            _ => delta == 0
        };
    }
}

static void RunBenchmark()
{
    const int count = 256;
    const int trials = 21;
    var random = new Random(45);
    var constraints = Enumerable.Range(0, count).Select(i =>
    {
        var angle = i * 2 * Math.PI / count;
        var (b, a) = Math.SinCos(angle);
        return new Constraint(a, b, LessOrEqual, 30 + 100 * a + 100 * b);
    }).ToArray();
    random.Shuffle(constraints);
    Objective objective = new(1, 2, Maximize);

    var solution = LinearProgram.Solve(objective, constraints);
    var (reference, countReference) = Pairwise();
    if (solution.Status != Optimal || solution.Value is not { } value || !Close(value, reference) ||
        solution.Vertices.Count != countReference)
        throw new InvalidOperationException("Benchmark implementations returned different results.");

    for (var i = 0; i < 25; i++)
    {
        LinearProgram.Solve(objective, constraints);
        Pairwise();
    }

    var clipping = new double[trials];
    var pairwise = new double[trials];
    for (var i = 0; i < trials; i++)
    {
        var timestamp = Stopwatch.GetTimestamp();
        LinearProgram.Solve(objective, constraints);
        clipping[i] = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
        timestamp = Stopwatch.GetTimestamp();
        Pairwise();
        pairwise[i] = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
    }
    Array.Sort(clipping);
    Array.Sort(pairwise);
    Console.WriteLine($"{count} constraints, {solution.Vertices.Count} vertices, median of {trials} warm Release runs.");
    Console.WriteLine($"Boundary clipping: {clipping[trials / 2]:F3} ms");
    Console.WriteLine($"Pairwise intersection and feasibility checks: {pairwise[trials / 2]:F3} ms");

    (double Value, int Vertices) Pairwise()
    {
        var best = double.NegativeInfinity;
        var vertices = 0;
        for (var i = 0; i < constraints.Length; i++)
        {
            var first = constraints[i];
            for (var j = i + 1; j < constraints.Length; j++)
            {
                var second = constraints[j];
                var determinant = first.A * second.B - first.B * second.A;
                if (Math.Abs(determinant) <= 1e-12)
                    continue;

                var x = (first.C * second.B - first.B * second.C) / determinant;
                var y = (first.A * second.C - first.C * second.A) / determinant;
                if (x < 0 || y < 0)
                    continue;

                var feasible = true;
                foreach (var row in constraints)
                {
                    if (row.A * x + row.B * y <= row.C + 1e-8)
                        continue;
                    feasible = false;
                    break;
                }
                if (!feasible)
                    continue;

                best = Math.Max(best, objective.Evaluate(new Point2(x, y)));
                vertices++;
            }
        }
        return (best, vertices);
    }
}
