using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using lpp.Solver;
using lpp.ViewModels;

namespace lpp.Views;

public sealed record PlotData(Objective Objective, IReadOnlyList<Constraint> Constraints, Solution Solution);

public sealed class PlotView : FrameworkElement
{
    private static readonly Brush Panel = PlotPalette.Brush(0xF7FAFC);
    private static readonly Brush AxisInk = PlotPalette.Brush(0x4F6576);
    private static readonly Brush TextInk = PlotPalette.Brush(0x243C4F);
    private static readonly Brush Teal = PlotPalette.Brush(0x087667);
    private static readonly Brush Region = PlotPalette.Brush(0xCDEFE4);
    private static readonly Pen GridLine = PlotPalette.Pen(PlotPalette.Brush(0xDFE8EE), 1);
    private static readonly Pen BorderLine = PlotPalette.Pen(PlotPalette.Brush(0xCADBE5), 1);
    private static readonly Pen AxisLine = PlotPalette.Pen(PlotPalette.Brush(0x8399A9), 1.35);
    private static readonly Pen LevelLine = PlotPalette.Pen(Teal, 2, DashStyles.Dash);
    private static readonly Pen RegionLine = PlotPalette.Pen(Teal, 1.45);
    private static readonly Pen VertexRing = PlotPalette.Pen(Brushes.White, 1.6);
    private static readonly Pen BestRing = PlotPalette.Pen(Teal, 2);
    private static readonly Typeface TextFace = new("Segoe UI");
    private static readonly Typeface BoldFace = new(new FontFamily("Segoe UI"), FontStyles.Normal,
        FontWeights.SemiBold, FontStretches.Normal);

    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data), typeof(PlotData), typeof(PlotView),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
            (sender, _) => ((PlotView)sender).Fit()));

    private Point _center;
    private double _unitsPerDip;
    private double _fitScale;
    private Point? _dragOrigin;

    public PlotData? Data
    {
        get => (PlotData?)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    private Rect Frame => new(48, 8, Math.Max(0, ActualWidth - 64), Math.Max(0, ActualHeight - 37));

    public void Fit()
    {
        _fitScale = 0;
        InvalidateVisual();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        Fit();
    }

    protected override void OnRender(DrawingContext drawing)
    {
        base.OnRender(drawing);
        var frame = Frame;
        if (Data is not { } data || frame.Width <= 0 || frame.Height <= 0)
            return;

        drawing.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        drawing.DrawRoundedRectangle(Panel, BorderLine, frame, 5, 5);
        if (_fitScale == 0)
            SetInitialCamera(data, frame);

        var world = new Rect(_center.X - frame.Width * _unitsPerDip / 2,
            _center.Y - frame.Height * _unitsPerDip / 2, frame.Width * _unitsPerDip, frame.Height * _unitsPerDip);

        Point ToScreen(Point p) => new(frame.Left + (p.X - world.Left) / _unitsPerDip,
            frame.Top + (world.Bottom - p.Y) / _unitsPerDip);

        drawing.PushClip(new RectangleGeometry(frame, 5, 5));
        if (data.Solution.Status != SolutionStatus.Infeasible)
        {
            var polygon = PlotGeometry.ClipRegion(world, data.Constraints);
            if (polygon.Count > 0)
            {
                var shape = new StreamGeometry();
                using (var context = shape.Open())
                {
                    context.BeginFigure(ToScreen(polygon[0]), isFilled: true, isClosed: true);
                    for (var i = 1; i < polygon.Count; i++)
                        context.LineTo(ToScreen(polygon[i]), isStroked: true, isSmoothJoin: false);
                }
                shape.Freeze();
                drawing.DrawGeometry(Region, RegionLine, shape);
            }
        }

        var step = GridStep(_unitsPerDip * 58);
        var gridX = Ticks(world.Left, world.Right, step);
        var gridY = Ticks(world.Top, world.Bottom, step);
        foreach (var x in gridX)
        {
            var screenX = ToScreen(new Point(x, 0)).X;
            drawing.DrawLine(x == 0 ? AxisLine : GridLine,
                new Point(screenX, frame.Top), new Point(screenX, frame.Bottom));
        }

        foreach (var y in gridY)
        {
            var screenY = ToScreen(new Point(0, y)).Y;
            drawing.DrawLine(y == 0 ? AxisLine : GridLine,
                new Point(frame.Left, screenY), new Point(frame.Right, screenY));
        }

        for (var i = 0; i < data.Constraints.Count; i++)
        {
            var row = data.Constraints[i];
            DrawBoundary(row.A, row.B, row.C, PlotPalette.ConstraintPen(i));
        }

        var optimum = data.Solution.Optimum;
        if (data.Solution.Value is { } value)
            DrawBoundary(data.Objective.A, data.Objective.B, value, LevelLine);

        var labels = new List<Rect>();
        if (optimum is { } best)
            DrawVertex(best, "F*", isBest: true);

        for (var i = 0; i < data.Solution.Vertices.Count; i++)
        {
            var vertex = data.Solution.Vertices[i];
            if (vertex != optimum)
                DrawVertex(vertex, ViewModel.VertexName(i), isBest: false);
        }
        drawing.Pop();

        foreach (var x in gridX)
        {
            var screenX = ToScreen(new Point(x, 0)).X;
            if (screenX > frame.Right - 29 || screenX < frame.Left + 8)
                continue;
            var label = Text(ViewModel.Number(x), 11, AxisInk);
            drawing.DrawText(label, new Point(screenX - label.Width / 2, frame.Bottom + 6));
        }

        foreach (var y in gridY)
        {
            var screenY = ToScreen(new Point(0, y)).Y;
            if (screenY < frame.Top + 13)
                continue;
            var label = Text(ViewModel.Number(y), 11, AxisInk);
            drawing.DrawText(label, new Point(Math.Max(0, frame.Left - label.Width - 8), screenY - label.Height / 2));
        }

        drawing.DrawText(Text("x₂", 12, TextInk, true), new Point(frame.Left - 18, frame.Top - 3));
        drawing.DrawText(Text("x₁", 12, TextInk, true), new Point(frame.Right - 11, frame.Bottom + 5));

        void DrawBoundary(double a, double b, double c, Pen pen)
        {
            if (PlotGeometry.ClipLine(world, a, b, c, out var start, out var end))
                drawing.DrawLine(pen, ToScreen(start), ToScreen(end));
        }

        void DrawVertex(Point2 vertex, string name, bool isBest)
        {
            var p = ToScreen(new Point(vertex.X, vertex.Y));
            if (!frame.Contains(p))
                return;

            if (isBest)
                drawing.DrawEllipse(Brushes.White, BestRing, p, 8, 8);
            drawing.DrawEllipse(isBest ? Teal : TextInk, VertexRing, p, 4.5, 4.5);

            var text = Text(name, 11, isBest ? Brushes.White : TextInk, true);
            var labelX = Math.Min(p.X + 10, frame.Right - text.Width - 12);
            var labelY = Math.Max(frame.Top + 4, p.Y - 24);
            var label = new Rect(labelX, labelY, text.Width + 10, 19);
            if (labels.Any(other => other.IntersectsWith(label)))
                return;
            labels.Add(label);

            drawing.DrawRoundedRectangle(isBest ? Teal : Brushes.White, isBest ? null : BorderLine, label, 4, 4);
            drawing.DrawText(text, new Point(label.X + 5, label.Y + 2));
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        if (_fitScale == 0 || !Frame.Contains(e.GetPosition(this)))
            return;

        var frame = Frame;
        var position = e.GetPosition(this);
        var offset = new Vector(position.X - frame.Left - frame.Width / 2,
            frame.Top + frame.Height / 2 - position.Y);
        var next = Math.Clamp(_unitsPerDip * Math.Pow(1.2, -e.Delta / 120.0), _fitScale / 1000, _fitScale * 1000);
        _center += offset * (_unitsPerDip - next);
        _unitsPerDip = next;
        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (_fitScale == 0 || !Frame.Contains(e.GetPosition(this)) || !CaptureMouse())
            return;
        _dragOrigin = e.GetPosition(this);
        Cursor = Cursors.SizeAll;
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_dragOrigin is not { } previous)
            return;
        var position = e.GetPosition(this);
        _center += new Vector(previous.X - position.X, position.Y - previous.Y) * _unitsPerDip;
        _dragOrigin = position;
        InvalidateVisual();
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        if (IsMouseCaptured)
            ReleaseMouseCapture();
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        base.OnLostMouseCapture(e);
        _dragOrigin = null;
        ClearValue(CursorProperty);
    }

    private void SetInitialCamera(PlotData data, Rect frame)
    {
        var maxX = 8.0;
        var maxY = 5.0;
        foreach (var vertex in data.Solution.Vertices)
        {
            maxX = Math.Max(maxX, vertex.X);
            maxY = Math.Max(maxY, vertex.Y);
        }

        var interceptLimitX = 1.6 * maxX;
        var interceptLimitY = 1.6 * maxY;
        foreach (var row in data.Constraints)
        {
            var x = row.A == 0 ? 0 : row.C / row.A;
            var y = row.B == 0 ? 0 : row.C / row.B;
            if (x <= interceptLimitX)
                maxX = Math.Max(maxX, x);
            if (y <= interceptLimitY)
                maxY = Math.Max(maxY, y);
        }

        _fitScale = _unitsPerDip = 1.16 * Math.Max(maxX / frame.Width, maxY / frame.Height);
        _center = new Point(frame.Width * _fitScale / 2 - maxX * 0.07, frame.Height * _fitScale / 2 - maxY * 0.07);
    }

    private static double GridStep(double target)
    {
        var magnitude = Math.Pow(10, Math.Floor(Math.Log10(target)));
        var normalized = target / magnitude;
        return magnitude * (normalized < 1.45 ? 1 : normalized < 3.1 ? 2 : normalized < 7 ? 5 : 10);
    }

    private static double[] Ticks(double min, double max, double step)
    {
        var first = Math.Ceiling(min / step);
        var count = Math.Clamp((int)(Math.Floor(max / step) - first + 1), 0, 256);
        var ticks = new double[count];
        for (var i = 0; i < count; i++)
            ticks[i] = (first + i) * step;
        return ticks;
    }

    private FormattedText Text(string text, double size, Brush brush, bool bold = false) =>
        new(text, CultureInfo.GetCultureInfo("ru-RU"), FlowDirection.LeftToRight,
            bold ? BoldFace : TextFace, size, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
}
