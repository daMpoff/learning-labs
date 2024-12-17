using MathNet.Symbolics;

namespace SNE.Interfaces
{
    public interface IIterativeMethod
    {
        string Name { get; }
        Dictionary<string, FloatingPoint> Solve(IEnumerable<SymbolicExpression> equations, Dictionary<string, FloatingPoint> initialValues, double tolerance, int maxIterations);
    }
}
