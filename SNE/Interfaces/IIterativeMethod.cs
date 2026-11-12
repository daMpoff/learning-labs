using MathNet.Symbolics;

namespace SNE.Interfaces
{
    public interface IIterativeMethod
    {
        string Name { get; }
        bool IsTwoFormNeed { get; set; }

        (Dictionary<string, FloatingPoint>, int) Solve(IEnumerable<SymbolicExpression> equations, Dictionary<string, FloatingPoint> initialValues, double tolerance, int maxIterations);
    }
}