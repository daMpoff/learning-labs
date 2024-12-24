namespace SLAE.Models
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
                var value = 0;
                var variable = $"x_{{{i + 1}}}";
                CoefficientVariablePairs.Add(new CoefficientVariablePair(value, variable)); // Создаем пару и добавляем в список
            }
        }
    }
}
