using SLAE.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SLAE.ViewModels
{
    public class ResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<RootResult> Results { get; set; }

        public ResultViewModel(ObservableCollection<RootResult> results)
        {
            Results = results;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
