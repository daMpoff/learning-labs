using System.ComponentModel;

namespace SLAE.ViewModels
{
    public class Coefficient : INotifyPropertyChanged
    {
        private double value;
        private string formula; // Добавлено свойство для формулы

        public double Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public string Formula // Свойство для формулы
        {
            get => formula;
            set
            {
                if (this.formula != value)
                {
                    this.formula = value;
                    OnPropertyChanged(nameof(Formula));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Coefficient(double initialValue = 0, string formula = "")
        {
            Value = initialValue;
            Formula = formula; // Инициализация формулы
        }
    }
}
