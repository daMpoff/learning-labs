using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using MathNet.Symbolics;

namespace Differentiation
{
    public partial class MainWindow : Window
    {
        private List<System.Windows.Controls.TextBox> _initConditionBoxes = new();

        public MainWindow()
        {
            InitializeComponent();

            // Список всех методов (Name будет в ComboBox)
            MethodComboBox.ItemsSource = ODESolversRegistry.AllSolvers;
            MethodComboBox.SelectedIndex = 0;

            GenerateInitialConditionFields();
            OrderTextBox.TextChanged += (s, e) => GenerateInitialConditionFields();
        }

        private void GenerateInitialConditionFields()
        {
            if (!int.TryParse(OrderTextBox.Text, out int order) || order < 1) order = 1;
            InitialConditionsPanel.Children.Clear();
            _initConditionBoxes.Clear();

            for (int i = 0; i < order; i++)
            {
                var panel = new System.Windows.Controls.StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    Margin = new Thickness(4, 2, 4, 2),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                };

                var label = new System.Windows.Controls.TextBlock
                {
                    Text = $"y{i}₀",
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Width = 28,
                    TextAlignment = System.Windows.TextAlignment.Right
                };
                var tb = new System.Windows.Controls.TextBox
                {
                    Width = 60,
                    Text = (i == 0 ? "1" : "0"),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                };
                _initConditionBoxes.Add(tb);
                panel.Children.Add(label);
                panel.Children.Add(tb);
                InitialConditionsPanel.Children.Add(panel);
            }
        }

        private void SolveODE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int order = int.Parse(OrderTextBox.Text);
                double start = double.Parse(StartTextBox.Text, CultureInfo.InvariantCulture);
                double end = double.Parse(EndTextBox.Text, CultureInfo.InvariantCulture);
                double step = double.Parse(StepTextBox.Text, CultureInfo.InvariantCulture);

                List<double> init = new();
                foreach (var tb in _initConditionBoxes)
                    init.Add(double.Parse(tb.Text, CultureInfo.InvariantCulture));

                var rhs = SymbolicExpression.Parse(RhsTextBox.Text);

                var solver = (IODESolver)MethodComboBox.SelectedItem;

                solver.Solve(order, start, end, step, init, rhs, out var tList, out var yLists);
                
                for (int i = 0; i < order; i++)
                {
                    var curve = Graphic.Plot.Add.Markers(
                        tList.ToArray(),
                        yLists[i].ToArray()
                    );
                    curve.LegendText = $"{solver.Name} y{i}";
                }
                Graphic.Plot.XLabel("t");
                Graphic.Plot.YLabel("y");
                Graphic.Plot.Title("Решение ОДУ");
                Graphic.Plot.ShowLegend();
                Graphic.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
