using System.Windows;
using lpp.Solver;

namespace lpp.Views;

internal static class PlotGeometry
{
    public static List<Point> ClipRegion(Rect world, IReadOnlyList<Constraint> constraints)
    {
        if (world.Right < 0 || world.Bottom < 0)
            return [];

        var x = Math.Max(0, world.Left);
        var y = Math.Max(0, world.Top);
        var polygon = new List<Point>
        {
            new(x, y), new(world.Right, y), world.BottomRight, new(x, world.Bottom)
        };
        var buffer = new List<Point>(polygon.Count);

        foreach (var row in constraints)
        {
            if (row.Relation != Relation.GreaterOrEqual)
                Clip(row.A, row.B, row.C);
            if (row.Relation != Relation.LessOrEqual)
                Clip(-row.A, -row.B, -row.C);
            if (polygon.Count == 0)
                break;
        }

        return polygon;

        void Clip(double a, double b, double c)
        {
            if (polygon.Count == 0)
                return;

            buffer.Clear();
            var previous = polygon[^1];
            var previousValue = SignedDistance(previous);
            foreach (var point in polygon)
            {
                var value = SignedDistance(point);
                if ((value <= 0) != (previousValue <= 0))
                {
                    var fraction = previousValue / (previousValue - value);
                    buffer.Add(previous + (point - previous) * fraction);
                }
                if (value <= 0)
                    buffer.Add(point);
                previous = point;
                previousValue = value;
            }

            (polygon, buffer) = (buffer, polygon);

            double SignedDistance(Point p)
            {
                var first = a * p.X;
                var second = b * p.Y;
                var delta = Math.FusedMultiplyAdd(a, p.X, second - c);
                var error = 2e-14 * Math.Max(Math.Abs(c), Math.Max(Math.Abs(first), Math.Abs(second)));
                return Math.Abs(delta) <= error ? 0 : delta;
            }
        }
    }

    public static bool ClipLine(Rect world, double a, double b, double c, out Point start, out Point end)
    {
        start = end = default;
        if (a == 0 && b == 0)
            return false;

        var freeX = Math.Abs(b) > Math.Abs(a);
        var pivot = freeX ? b : a;
        var slope = -(freeX ? a : b) / pivot;
        var offset = c / pivot;
        var low = freeX ? world.Left : world.Top;
        var high = freeX ? world.Right : world.Bottom;
        var minFixed = freeX ? world.Top : world.Left;
        var maxFixed = freeX ? world.Bottom : world.Right;
        if (slope == 0)
        {
            if (offset < minFixed || offset > maxFixed)
                return false;
        }
        else
        {
            var t1 = (minFixed - offset) / slope;
            var t2 = (maxFixed - offset) / slope;
            low = Math.Max(low, Math.Min(t1, t2));
            high = Math.Min(high, Math.Max(t1, t2));
            if (low > high)
                return false;
        }

        start = At(low);
        end = At(high);
        return true;

        Point At(double t)
        {
            var fixedCoordinate = Math.Clamp(Math.FusedMultiplyAdd(slope, t, offset), minFixed, maxFixed);
            return freeX ? new Point(t, fixedCoordinate) : new Point(fixedCoordinate, t);
        }
    }
}
