using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SNE.Models;

public class VariableValuePair : INotifyPropertyChanged
{
    private string variableName;
    private double value;
    private string displayVariableName;

    public string VariableName
    {
        get => variableName;
        set
        {
            if (variableName != value)
            {
                variableName = value;
                OnPropertyChanged();
            }
        }
    }

    public string DisplayVariableName
    {
        get => displayVariableName;
        set
        {
            if (displayVariableName != value)
            {
                displayVariableName = value;
                OnPropertyChanged();
            }
        }
    }

    public double Value
    {
        get => value;
        set
        {
            if (this.value != value)
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}