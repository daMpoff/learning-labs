using SLAE.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SLAE.Views
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
        }

        private void ResultWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ResultsDataGrid.Columns.Clear();

            // Добавляем колонку для "Название метода"
            var methodColumn = new DataGridTemplateColumn
            {
                Header = "Название метода",
                CellTemplate = CreateTextCellTemplate("MethodName")
            };
            ResultsDataGrid.Columns.Add(methodColumn);

            // Определяем максимальное количество корней в коллекции Results
            if (DataContext is ResultViewModel viewModel && viewModel.Results.Any())
            {
                var maxRootCount = viewModel.Results.Max(r => r.Roots.Count);

                for (int i = 0; i < maxRootCount; i++)
                {
                    // Создаем колонку для каждого корня
                    var rootColumn = new DataGridTemplateColumn
                    {
                        Header = new WpfMath.Controls.FormulaControl
                        {
                            Formula = $"x_{{{i + 1}}}", // Корректное отображение нижнего индекса
                            Margin = new Thickness(3, 5, 0, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        CellTemplate = CreateTextCellTemplate($"Roots[{i}]")
                    };
                    ResultsDataGrid.Columns.Add(rootColumn);
                }
            }
        }

        private DataTemplate CreateTextCellTemplate(string bindingPath)
        {
            // Создаем шаблон ячейки с TextBlock, привязанным к указанному пути
            var xamlTemplate = $@"
        <DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
            <TextBlock Text='{{Binding {bindingPath}, StringFormat=F6}}'/>
        </DataTemplate>";

            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xamlTemplate);
        }
    }
}