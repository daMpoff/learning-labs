using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SLAE.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int equationsCount = 1;
        private int variablesCount = 1;
        private ObservableCollection<Equation> equations;
        public List<int> Numbers { get; } = Enumerable.Range(1, 10).ToList(); // Устанавливаем диапазон от 2 до 10

        public event PropertyChangedEventHandler PropertyChanged;

        public int EquationsCount
        {
            get => equationsCount;
            set
            {
                equationsCount = value;
                OnPropertyChanged(nameof(EquationsCount));
                UpdateEquations();
            }
        }

        public int VariablesCount
        {
            get => variablesCount;
            set
            {
                variablesCount = value;
                OnPropertyChanged(nameof(VariablesCount));
                UpdateEquations(); // Обновляем уравнения, если изменилось количество переменных
            }
        }

        public ObservableCollection<Equation> Equations
        {
            get => equations;
            private set
            {
                equations = value;
                OnPropertyChanged(nameof(Equations));
            }
        }
        public ViewModel()
        {
            Equations = new ObservableCollection<Equation>();
            UpdateEquations();
        }

        private void UpdateEquations()
        {
            Equations.Clear();
            for (int i = 0; i < EquationsCount; i++)
            {
                Equations.Add(new Equation(VariablesCount)); // Создаем уравнения с переменным количеством переменных
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
