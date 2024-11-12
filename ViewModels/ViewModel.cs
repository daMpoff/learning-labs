using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SLAE.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int equationsCount = 2;
        private int variablesCount = 2;
        private bool isAllSelected;
        private ObservableCollection<Equation> equations;
        // Коллекция для состояния каждого CheckBox
        public ObservableCollection<bool> Methods { get; } = new ObservableCollection<bool>();
        public List<int> Numbers { get; } = Enumerable.Range(1, 100).ToList(); // Устанавливаем диапазон от 2 до 10

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
        // Свойство для управления выбором всех CheckBox
        public bool IsAllSelected
        {
            get => isAllSelected;
            set
            {
                if (isAllSelected != value)
                {
                    isAllSelected = value;
                    OnPropertyChanged(nameof(IsAllSelected));
                    // Устанавливаем состояние всех CheckBox в Methods
                    for (int i = 0; i < Methods.Count; i++)
                    {
                        Methods[i] = isAllSelected;
                    }
                }
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
            // Инициализация коллекции с состоянием для каждого CheckBox
            for (int i = 0; i < 9; i++)
            {
                Methods.Add(false); // Изначально все элементы не выбраны
            }
        }

        private void UpdateEquations()
        {
            Equations.Clear();
            for (int i = 0; i < EquationsCount; i++)
            {
                var equation = new Equation(VariablesCount);
                // Устанавливаем IsLast для каждого коэффициента
                for (int j = 0; j < equation.CoefficientVariablePairs.Count; j++)
                {
                    equation.CoefficientVariablePairs[j].IsLast = (j == equation.CoefficientVariablePairs.Count - 1);
                }

                Equations.Add(equation);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
