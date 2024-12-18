using System.Windows;
using MathNet.Symbolics;

namespace SNE.Models
{
    public class SystemNonLinearEquations
    {
        public SystemNonLinearEquations(IEnumerable<string> expressions)
        {
            ValidateExpressions(expressions);
            Expressions = expressions;
            SymbolicExpressions = expressions
                .Select(expression =>
                {
                    return SymbolicExpression.Parse(expression);
                })
                .ToList();
            //CorrectCheck();
        }

        public IEnumerable<string> Expressions { get; set; }
        public IEnumerable<SymbolicExpression> SymbolicExpressions { get; set; }

        private void CorrectCheck()
        {
            if (SymbolicExpressions.Any(expression => expression.CollectVariables().Count() != SymbolicExpressions.Count()))
            {
                throw new InvalidOperationException("Все уравнения должны содержать одинаковое количество переменных!");
            }
        }
        private void ValidateExpressions(IEnumerable<string> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Выражения не были инициализированы!");
            }

            foreach (var function in expression)
            {
                if (string.IsNullOrEmpty(function) || string.IsNullOrWhiteSpace(function))
                {
                    throw new ArgumentException(nameof(function), "Выражение не может быть пустым, либо состоять из пробелов!");
                }
            }
        }
    }
}
