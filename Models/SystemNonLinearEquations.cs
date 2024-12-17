using MathNet.Symbolics;

namespace SNE.Models
{
    public class SystemNonLinearEquations
    {
        public SystemNonLinearEquations(IEnumerable<string> functions)
        {
            ValidateFunctions(functions);

            Functions = functions;
            FunctionExpressions = functions
                .Select(function =>
                {
                    if (string.IsNullOrEmpty(function) || string.IsNullOrWhiteSpace(function))
                    {
                        throw new ArgumentNullException(nameof(function), "Выражение не может быть пустым, либо состоять из пробелов!");
                    }
                    return SymbolicExpression.Parse(function);
                })
                .ToList();
            CorrectCheck();
        }

        public IEnumerable<string> Functions { get; set; }
        public List<SymbolicExpression> FunctionExpressions { get; set; }

        private void CorrectCheck()
        {

        }
        private void ValidateFunctions(IEnumerable<string> functions)
        {
            if (functions == null)
            {
                throw new ArgumentNullException(nameof(functions), "Выражения не были инициализированы!");
            }

            foreach (var function in functions)
            {
                if (string.IsNullOrEmpty(function) || string.IsNullOrWhiteSpace(function))
                {
                    throw new ArgumentNullException(nameof(function), "Выражение не может быть пустым, либо состоять из пробелов!");
                }
            }
        }
    }
}
