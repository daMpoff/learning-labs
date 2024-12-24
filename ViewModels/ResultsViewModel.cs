using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathNet.Symbolics;
using SNE.Models;

namespace SNE.ViewModels;

public class ResultsViewModel : INotifyPropertyChanged
{
    private int iterationsCount = 0;
    private string methodName;
    public ObservableCollection<VariableValuePair> Results { get; set; }

    public int IterationsCount
    {
        get => iterationsCount;
        set
        {
            if (iterationsCount != value)
            {
                iterationsCount = value;
                OnPropertyChanged();
            }
        }
    }

    public string MethodName
    {
        get => methodName;
        set
        {
            if (value != methodName)
            {
                methodName = value;
                OnPropertyChanged();
            }
        }
    }

    public ResultsViewModel(Dictionary<string, FloatingPoint> results, int iterations, string methodName)
    {
        Results = new ObservableCollection<VariableValuePair>();
        IterationsCount = iterations;
        MethodName = methodName;
        LoadData(results);
    }

    private void LoadData(Dictionary<string, FloatingPoint> resultValues)
    {
        foreach (var result in resultValues)
        {
            Results.Add(new VariableValuePair
            {
                DisplayVariableName = result.Key,
                Value = result.Value.RealValue
            });
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}