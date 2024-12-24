using SLAE.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SLAE.ViewModels
{
    public class ResultViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RootResult> Results { get; set; }

        public ResultViewModel(ObservableCollection<RootResult> results)
        {
            Results = results;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
