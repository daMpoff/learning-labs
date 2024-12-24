using System.ComponentModel;

namespace SLAE.ViewModels
{
    public class CoefficientVariablePair : INotifyPropertyChanged
    {
        private double value;
        private string variable;
        public bool IsLast { get; set; }

        public double Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
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

        public CoefficientVariablePair(double value, string variable)
        {
            Value = value;
            Variable = variable;
        }
    }
}
