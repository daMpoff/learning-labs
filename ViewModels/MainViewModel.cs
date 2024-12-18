using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MathNet.Symbolics;
using SNE.Commands;
using SNE.Interfaces;
using SNE.Models;
using Expression = SNE.Models.Expression;

namespace SNE.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int expressionCount = 0;
        private bool isTwoFormNeed = false;
        private double tolerance = 0.001; // Значение точности по умолчанию
        private IIterativeMethod selectedMethod;
        private ObservableCollection<Expression> expressions = new ObservableCollection<Expression>();

        public ObservableCollection<VariableValuePair> InitialValues { get; set; } =
            new ObservableCollection<VariableValuePair>();

        public ObservableCollection<IIterativeMethod> SolvingMethods { get; set; } =
            new ObservableCollection<IIterativeMethod>();

        private Dictionary<string, FloatingPoint> initialValues = new Dictionary<string, FloatingPoint>();

        public ObservableCollection<Expression> Expressions
        {
            get => expressions;
            set
            {
                if (expressions != value)
                {
                    expressions = value;
                    OnPropertyChanged();
                }
            }
        }

        public IIterativeMethod SelectedMethod
        {
            get => selectedMethod;
            set
            {
                if (selectedMethod != value)
                {
                    selectedMethod = value;
                    OnPropertyChanged();
                    OnMethodComboBoxSelectionChanged();
                }
            }
        }

        public bool IsTwoFormNeed
        {
            get => isTwoFormNeed;
            set
            {
                if (isTwoFormNeed != value)
                {
                    isTwoFormNeed = value;
                    OnPropertyChanged();
                    // Очистить панель при смене режима
                    Expressions.Clear();
                }
            }
        }

        public double Tolerance
        {
            get => tolerance;
            set
            {
                if (tolerance != value)
                {
                    tolerance = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SolveCommand { get; set; }
        public ICommand ComboBoxSelectionChangedCommand { get; set; }
        public ICommand AddEquationCommand { get; set; }
        public ICommand RemoveEquationCommand { get; set; }

        public MainViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            BuildSolvers();
            SolveCommand = new RelayCommand(Solve);
            ComboBoxSelectionChangedCommand = new RelayCommand(OnMethodComboBoxSelectionChanged);
            AddEquationCommand = new RelayCommand(AddExpression);
            RemoveEquationCommand = new RelayCommand(RemoveExpression);
        }

        private void AddExpression()
        {
            if (IsTwoFormNeed)
            {
                AddMode1Row();
            }
            else
            {
                AddMode2Row();
            }

            UpdateInitialValues();
        }

        private void RemoveExpression()
        {
            if (Expressions.Count > 0)
            {
                Expressions.RemoveAt(Expressions.Count - 1);
                UpdateInitialValues();
            }
            else
            {
                MessageBox.Show("Вы не можете удалять элементы. \nСписок выражений пуст!");
            }
        }

        private void AddMode1Row()
        {
            Expressions.Add(new Expression { DisplayFormula = $"x_{{{Expressions.Count + 1}}} = " });
        }

        private void AddMode2Row()
        {
            Expressions.Add(new Expression { DisplayFormula = $"f_{{{Expressions.Count + 1}}}(x) = " });
        }

        private void OnMethodComboBoxSelectionChanged()
        {
            if (selectedMethod != null)
            {
                IsTwoFormNeed = SelectedMethod.IsTwoFormNeed;
            }
        }

        private void UpdateInitialValues()
        {
            InitialValues.Clear();
            for (int i = 0; i < Expressions.Count(); i++)
            {
                InitialValues.Add(new VariableValuePair()
                    { VariableName = $"x_{i + 1}", Value = 0.0, DisplayVariableName = $"x_{{{i + 1}}} = " });
            }
        }

        private void BuildSolvers()
        {
            SolvingMethods = new ObservableCollection<IIterativeMethod>(SolversBuilder.BuildSolvers());
        }

        private void Solve()
        {
            try
            {
                initialValues = ConvertInitialValues();
                SystemNonLinearEquations systemNonLinearEquations =
                    new SystemNonLinearEquations(expressions.Select(p => p.Value).ToList());
                var (results, iterationsCount) = SelectedMethod.Solve(systemNonLinearEquations.SymbolicExpressions, initialValues, tolerance, 10000);
                MessageBox.Show($"{iterationsCount}");
                foreach (var result in results)
                {
                    MessageBox.Show($"{result.Key}: {result.Value.RealValue:F6}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Dictionary<string, FloatingPoint> ConvertInitialValues()
        {
            Dictionary<string, FloatingPoint> values = new Dictionary<string, FloatingPoint>();
            for(int i = 0; i < InitialValues.Count();i++)
            {
                values.Add(InitialValues[i].VariableName, InitialValues[i].Value);
            }
            return values;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}