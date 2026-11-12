using System.Windows.Media;

namespace lpp.Views;

internal static class PlotPalette
{
    private static readonly Brush[] Constraints =
    [
        Brush(0x2865A7), Brush(0xAC571F), Brush(0x7457A0), Brush(0xA03D66),
        Brush(0x3C6A59), Brush(0x775831), Brush(0x496689), Brush(0x994B47)
    ];

    private static readonly Pen[] Boundaries = Constraints.Select(brush => Pen(brush, 2)).ToArray();

    public static Brush ConstraintBrush(int index) => Constraints[index % Constraints.Length];
    public static Pen ConstraintPen(int index) => Boundaries[index % Boundaries.Length];

    public static Brush Brush(uint rgb) =>
        Freeze(new SolidColorBrush(Color.FromRgb((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb)));

    public static Pen Pen(Brush brush, double thickness, DashStyle? dash = null)
    {
        var pen = new Pen(brush, thickness);
        if (dash is not null)
            pen.DashStyle = dash;
        return Freeze(pen);
    }

    private static T Freeze<T>(T resource) where T : System.Windows.Freezable
    {
        resource.Freeze();
        return resource;
    }
}
