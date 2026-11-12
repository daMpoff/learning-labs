using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using lpp.Solver;
using lpp.Views;

namespace lpp.ViewModels;

public sealed class ViewModel : INotifyPropertyChanged
{
    private static readonly CultureInfo Russian = CultureInfo.GetCultureInfo("ru-RU");
    private string _objectiveX = "";
    private string _objectiveY = "";
    private int _goalIndex;
    private string _error = "";

    public ObservableCollection<ConstraintInput> Rows { get; } = [];
    public string ObjectiveX { get => _objectiveX; set => Set(ref _objectiveX, value); }
    public string ObjectiveY { get => _objectiveY; set => Set(ref _objectiveY, value); }
    public int GoalIndex { get => _goalIndex; set => Set(ref _goalIndex, value); }
    public string Error { get => _error; private set => Set(ref _error, value); }
    public PlotData? Data { get; private set; }
    public IReadOnlyList<BoundaryLabel> Boundaries { get; private set; } = [];
    public IReadOnlyList<VertexLabel> Vertices { get; private set; } = [];
    public string ResultTitle { get; private set; } = "";
    public string ResultValue { get; private set; } = "";
    public string ResultPoint { get; private set; } = "";
    public string VertexCount => Vertices.Count.ToString(Russian);
    public Visibility LevelLegendVisibility => Data is { Solution.Status: SolutionStatus.Optimal, Objective: var objective }
        && (objective.A != 0 || objective.B != 0) ? Visibility.Visible : Visibility.Collapsed;

    public ViewModel() => LoadExample();

    public void LoadExample()
    {
        ObjectiveX = "3";
        ObjectiveY = "4";
        GoalIndex = 0;
        Rows.Clear();
        Rows.Add(new("4", "9", 0, "36"));
        Rows.Add(new("2", "1", 0, "11"));
        Rows.Add(new("0", "1", 0, "5"));
        Solve();
    }

    public void Solve()
    {
        if (!TryRead(ObjectiveX, out var cx) || !TryRead(ObjectiveY, out var cy))
        {
            Error = "Проверьте коэффициенты целевой функции.";
            return;
        }

        var constraints = new Constraint[Rows.Count];
        for (var i = 0; i < Rows.Count; i++)
        {
            var row = Rows[i];
            if (!TryRead(row.A, out var a) || !TryRead(row.B, out var b) || !TryRead(row.C, out var c))
            {
                Error = $"Проверьте числа в ограничении {i + 1}.";
                return;
            }

            constraints[i] = new Constraint(a, b, (Relation)row.RelationIndex, c);
        }

        var objective = new Objective(cx, cy, GoalIndex == 0 ? Goal.Maximize : Goal.Minimize);
        Solution result;
        try
        {
            result = LinearProgram.Solve(objective, constraints);
        }
        catch (ArgumentException)
        {
            Error = "Коэффициенты выходят за пределы численного диапазона.";
            return;
        }

        Error = "";
        Data = new PlotData(objective, constraints, result);
        Boundaries = constraints.Select((row, i) =>
            new BoundaryLabel($"R{i + 1}   {Equation(row.A, row.B)} {RelationText(row.Relation)} {Number(row.C)}",
                PlotPalette.ConstraintBrush(i))).ToArray();
        Vertices = result.Vertices.Select((point, i) =>
            new VertexLabel(VertexName(i), Coordinates(point), Number(objective.Evaluate(point)),
                point == result.Optimum)).ToArray();

        (ResultTitle, ResultValue, ResultPoint) = result.Status switch
        {
            SolutionStatus.Optimal => (
                objective.Goal == Goal.Maximize ? "МАКСИМУМ" : "МИНИМУМ",
                $"F* = {Number(result.Value!.Value)}",
                $"x* = {Coordinates(result.Optimum!.Value)}" +
                    (result.HasAlternatives ? " · есть другие оптимумы" : "")),
            SolutionStatus.Unbounded => ("РЕЗУЛЬТАТ", "Функция не ограничена",
                objective.Goal == Goal.Maximize ? "Значение F может расти без границы." : "Значение F может убывать без границы."),
            _ => ("РЕЗУЛЬТАТ", "Допустимых точек нет", "Ограничения несовместны.")
        };

        Notify(nameof(Data));
        Notify(nameof(Boundaries));
        Notify(nameof(Vertices));
        Notify(nameof(VertexCount));
        Notify(nameof(LevelLegendVisibility));
        Notify(nameof(ResultTitle));
        Notify(nameof(ResultValue));
        Notify(nameof(ResultPoint));
    }

    public static string Number(double number) => (number == 0 ? 0 : number).ToString("G7", Russian);

    public static string VertexName(int index) => index < 26 ? ((char)('A' + index)).ToString() : $"P{index + 1}";

    private static string Coordinates(Point2 point) => $"({Number(point.X)}  |  {Number(point.Y)})";

    private static string Equation(double a, double b)
    {
        if (a == 0 && b == 0)
            return "0";

        var first = a == 0 ? "" : Term(a, "x₁");
        if (b == 0)
            return first;
        if (a == 0)
            return Term(b, "x₂");
        return first + (b < 0 ? " − " : " + ") + Term(Math.Abs(b), "x₂");
    }

    private static string Term(double coefficient, string variable) => coefficient switch
    {
        1 => variable,
        -1 => "−" + variable,
        _ => Number(coefficient) + variable
    };

    private static string RelationText(Relation relation) => relation switch
    {
        Relation.LessOrEqual => "≤",
        Relation.GreaterOrEqual => "≥",
        _ => "="
    };

    private static bool TryRead(string text, out double number)
    {
        var parsed = double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out number);
        var magnitude = Math.Abs(number);
        return parsed && double.IsFinite(number) && (magnitude == 0 || magnitude is >= 1e-9 and <= 1e9);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Notify([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;
        field = value;
        Notify(propertyName);
    }
}

public sealed class ConstraintInput(string a = "1", string b = "1", int relationIndex = 0, string c = "10")
{
    public string A { get; set; } = a;
    public string B { get; set; } = b;
    public string C { get; set; } = c;
    public int RelationIndex { get; set; } = relationIndex;
}

public sealed record BoundaryLabel(string Text, Brush Color);
public sealed record VertexLabel(string Name, string Coordinates, string Value, bool IsOptimal);
