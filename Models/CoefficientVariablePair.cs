using System.ComponentModel;

namespace SLAE.ViewModels
{
    public class CoefficientVariablePair : INotifyPropertyChanged
    {
        private Coefficient coefficient;
        private string variable;
        public bool IsLast { get; set; }

        public Coefficient Coefficient
        {
            get => coefficient;
            set
            {
                coefficient = value;
                OnPropertyChanged(nameof(Coefficient));
            }
        }

        public string Variable
        {
            get => variable;
            set
            {
                variable = value;
                OnPropertyChanged(nameof(Variable));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CoefficientVariablePair(Coefficient coefficient, string variable)
        {
            Coefficient = coefficient;
            Variable = variable;
        }
    }
}
