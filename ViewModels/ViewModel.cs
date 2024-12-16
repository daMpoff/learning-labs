using SLAE.Commands;
using SLAE.Interfaces;
using SLAE.Models;
using SLAE.Solvers;
using SLAE.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SLAE.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int equationsCount = 2;
        private int variablesCount = 2;
        private bool isAllSelected;
        private ObservableCollection<Equation> equations;
        public ICommand GetResultCommand { get; }
        public ObservableCollection<ISLAESolver> Methods { get; set; }
        public List<int> Numbers { get; } = Enumerable.Range(1, 10).ToList(); // Устанавливаем диапазон от 1 до 10

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<RootResult> Results { get; set; } = new ObservableCollection<RootResult> { };
        public SLAEModel SLAEModel { get; set; }

        public int EquationsCount
        {
            get => equationsCount;
            set
            {
                equationsCount = value;
                OnPropertyChanged(nameof(EquationsCount));
                UpdateEquations(); // Обновляем уравнения, если изменилось количество уравнений
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
                        Methods[i].IsEnable = isAllSelected;
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
            InitializeSolvers();
            UpdateEquations();
            GetResultCommand = new RelayCommand(GetResult);
        }
        private void InitializeSolvers()
        {
            Methods = new ObservableCollection<ISLAESolver>()
            {
                new MatrixSolver(),
                new GaussianSolver(),
                new SeidelSolver(),
                new TridiagonalSolver(),
                new SimpleIterationSolver(),
                new CramerSolver()
            };
        }
        private void GetResult()
        {
            if (Results.Any())
            {
                Results.Clear();
            }
            foreach (var method in Methods)
            {
                CastVariablesToSLAEModel();
                if (method.IsEnable)
                {
                    try
                    {
                        RootResult rootResult = new RootResult()
                        {
                            Roots = new ObservableCollection<double>(method.Solve(SLAEModel).Select(d => d)),
                            MethodName = method.Name,
                        };
                        Results.Add(rootResult);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{method.Name}: Возникла ошибка\n{ex.Message}");
                    }
                }
            }
            ResultViewModel resultViewModel = new ResultViewModel(Results);
            ResultWindow resultWindow = new ResultWindow() { DataContext = resultViewModel };
            resultWindow.Show();
        }

        private void CastVariablesToSLAEModel()
        {
            SLAEModel = new SLAEModel(equationsCount, variablesCount);
            for (int i = 0; i < equations.Count; i++)
            {
                Equation equation = equations[i];
                for (int j = 0; j < equation.CoefficientVariablePairs.Count; j++)
                {
                    CoefficientVariablePair pair = equation.CoefficientVariablePairs[j];
                    SLAEModel.Coefficients[i, j] = pair.Coefficient.Value;
                }
                SLAEModel.Constants[i] = equation.Result;
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
