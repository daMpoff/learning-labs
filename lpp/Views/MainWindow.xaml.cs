using System.Windows;
using lpp.ViewModels;

namespace lpp;

public partial class MainWindow : Window
{
    private readonly ViewModel _model = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _model;
    }

    private void Solve_Click(object sender, RoutedEventArgs e) => _model.Solve();
    private void Example_Click(object sender, RoutedEventArgs e) => _model.LoadExample();
    private void Fit_Click(object sender, RoutedEventArgs e) => Plot.Fit();
    private void Add_Click(object sender, RoutedEventArgs e) => _model.Rows.Add(new ConstraintInput());

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ConstraintInput row })
            _model.Rows.Remove(row);
    }
}
