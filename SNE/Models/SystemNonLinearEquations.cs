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
                .Select(expression => { return SymbolicExpression.Parse(expression); })
                .ToList();
        }

        public IEnumerable<string> Expressions { get; set; }
        public IEnumerable<SymbolicExpression> SymbolicExpressions { get; set; }

        private void ValidateExpressions(IEnumerable<string> expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("Выражения не были инициализированы!");
            }

            foreach (var expression in expressions)
            {
                if (string.IsNullOrEmpty(expression) || string.IsNullOrWhiteSpace(expression))
                {
                    throw new ArgumentException("Одно не может быть пустым, либо состоять из пробелов!");
                }
            }
        }
    }
}