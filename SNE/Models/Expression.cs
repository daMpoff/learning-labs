using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SNE.Models;

public class Expression : INotifyPropertyChanged
{
    private string displayFormula;
    private string value;

    public string DisplayFormula
    {
        get => displayFormula;
        set
        {
            if (displayFormula != value)
            {
                displayFormula = value;
                OnPropertyChanged();
            }
        }
    }

    public string Value
    {
        get => this.value;
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