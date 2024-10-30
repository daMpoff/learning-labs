namespace SLAE.ViewModels
{
    public class Equation
    {
        public List<CoefficientVariablePair> CoefficientVariablePairs { get; set; }
        public double Result { get; set; }

        public Equation(int variablesCount)
        {
            CoefficientVariablePairs = new List<CoefficientVariablePair>();
            for (int i = 0; i < variablesCount; i++)
            {
                var coefficient = new Coefficient(); // Создаем коэффициент
                var variable = $"x_{{{i + 1}}}";
                CoefficientVariablePairs.Add(new CoefficientVariablePair(coefficient, variable)); // Создаем пару и добавляем в список
            }
        }
    }
}
