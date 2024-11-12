using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers
{
    public class GaussianSolver : ISLAESolver
    {
        public string Name { get => "Метод Гаусса"; }
        public bool IsEnable { get; set; }

        public double[] Solve(SLAEModel slae)
        {
            // Реализация метода Гаусса для решения СЛАУ, тестовая
            return new double[slae.VariablesCount];
        }
    }
}
