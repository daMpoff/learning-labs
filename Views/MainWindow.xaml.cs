using System.Windows;
using System.Windows.Controls;
using WpfMath.Controls;

namespace SNE.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Очистить панель при смене режима
            DynamicPanel?.Children.Clear();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModeComboBox.SelectedIndex == 0)
            {
                AddMode1Row();
            }
            else if (ModeComboBox.SelectedIndex == 1)
            {
                AddMode2Row();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DynamicPanel.Children.Count > 0)
            {
                DynamicPanel.Children.RemoveAt(DynamicPanel.Children.Count - 1);
                if (ModeComboBox.SelectedIndex == 0)
                {
                }
                else if (ModeComboBox.SelectedIndex == 1)
                {
                }
            }
        }

        private void AddMode1Row()
        {
            StackPanel row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 5)
            };

            FormulaControl formulaBlock = new FormulaControl
            {
                Formula = $"x_{{{mode1Counter}}} = ",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 5, 0)
            };

            TextBox textBox = new TextBox
            {
                Width = 200
            };

            row.Children.Add(formulaBlock);
            row.Children.Add(textBox);

            DynamicPanel.Children.Add(row);
            mode1Counter++;
        }

        private void AddMode2Row()
        {
            StackPanel row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 5)
            };

            FormulaControl formalaBlock = new FormulaControl
            {
                Formula = $"f_{{{mode2Counter}}}(x) = ",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 5, 0)
            };

            TextBox textBox = new TextBox
            {
                Width = 200
            };

            row.Children.Add(formalaBlock);
            row.Children.Add(textBox);

            DynamicPanel.Children.Add(row);
            mode2Counter++;
        }
        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = "Результаты:\n";

            foreach (var row in DynamicPanel.Children)
            {
                if (row is StackPanel stackPanel)
                {
                    foreach (var child in stackPanel.Children)
                    {
                        if (child is TextBox textBox)
                        {
                            ResultTextBox.Text += textBox.Text + "\n";
                        }
                    }
                }
            }
        }
    }
}
